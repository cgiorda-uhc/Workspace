using Avro;
using Avro.Generic;
using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Net;
using Confluent.SchemaRegistry.Serdes;
using Confluent.SchemaRegistry;
using System.Threading;
using Confluent.Kafka.SyncOverAsync;
using OptumHealth.BoSS.Core.Infrastructure;
using OptumHealth.BoSS.Core.Infrastructure.Logging;

namespace BoSS.Web.EISLogging
{
    public class EISLog
    {
        private static String TOPIC = System.Configuration.ConfigurationManager.AppSettings["Topic"];
        private static string bootstrapServers = System.Configuration.ConfigurationManager.AppSettings["BootstrapServers"];
        private static string askID = System.Configuration.ConfigurationManager.AppSettings["AskID"];
        private static string CI = System.Configuration.ConfigurationManager.AppSettings["CI"];
        private static string appName = System.Configuration.ConfigurationManager.AppSettings["AppName"];
       
        public async Task Produce(LogData ld, string pemPath, string mySchema)
        {
            string clientID = askID;
            using (var producer =
                new ProducerBuilder<Null, GenericRecord>(
                    new ProducerConfig
                    {
                        BootstrapServers = bootstrapServers
                        , ClientId = clientID
                        , SecurityProtocol = SecurityProtocol.Ssl
                        , SslCaLocation = pemPath
                    })
                    .SetValueSerializer(new BossSerializer<GenericRecord>())
                    .Build())
            {
                
                Log.For(this).Info("inside Produce: step 2");
                
                string msg = "";
                var test = CreateMessageObject(ld, pemPath, mySchema);
                await producer.ProduceAsync(TOPIC, test)
                            .ContinueWith(task =>
                            {
                                msg = task.IsFaulted
                                    ? $"error producing message: {task.Exception.Message}"
                                    : $"produced to: {task.Result.TopicPartitionOffset}";
                                Log.For(this).Info(msg);
                                producer.Flush(TimeSpan.FromSeconds(30));
                            });
                
                Log.For(this).Info("Message sent out");
            }
        }
        

        private Message<Null, GenericRecord> CreateMessageObject(LogData ld, string pemPath, string mySchema)
        {
            RecordSchema sm = null;
            try
            {
                sm = (RecordSchema)Avro.Schema.Parse(mySchema);
            }
            catch (Exception ex)
            {
                Log.For(this).Info(ex.Message);
            }
            
            GenericRecord logEvt = new GenericRecord(sm);
            Field dFld = null;
            Field aFld = null;
            Field logClassFld = null;
            Field sFld = null;
            Field destUserFld = null;
            GenericRecord d = null;
            GenericRecord a = null;
            GenericRecord ds = null;

            if (sm.TryGetField("device", out dFld))
            {
                d = new GenericRecord((RecordSchema)dFld.Schema);
                d.Add("vendor", "Optum");
                d.Add("product", "");
                d.Add("ip4", 0L);
                d.Add("hostname", ld.device.hostname);
                d.Add("version", "1");
                d.Add("CI", CI);
                d.Add("pid", 0);
                d.Add("proc", "");
                logEvt.Add("device", d);
            }
            if (sm.TryGetField("application", out aFld))
            {
                a = new GenericRecord((RecordSchema)aFld.Schema);
                a.Add("askId", askID);
                a.Add("name", appName);
                a.Add("CI", CI);
                logEvt.Add("application", a);
            }
            logEvt.Add("receivedTime", CurrentTimeMillis());
            logEvt.Add("msg", ld.msg);
            if (sm.TryGetField("logClass", out logClassFld))
                logEvt.Add("logClass", new GenericEnum((EnumSchema)logClassFld.Schema, ld.logClass.ToString()));
            if (sm.TryGetField("severity", out sFld))
            {
                UnionSchema us = (UnionSchema)sFld.Schema;
                logEvt.Add("severity", new GenericEnum((EnumSchema)us[1], ld.severity.ToString()));
            }

            if (sm.TryGetField("destUser", out destUserFld))
            {
                UnionSchema du = (UnionSchema)destUserFld.Schema;
                ds = new GenericRecord((RecordSchema)du[1]);
                ds.Add("uid", ld.destUser.uid);
                ds.Add("uuid", ld.destUser.uid);
                ds.Add("name", ld.destUser.name);
                ds.Add("firstName", null);
                ds.Add("lastName", null);
                ds.Add("tokenIssuer", null);
                ds.Add("tokenCreated", null);
                ds.Add("tokenExpires", null);
                ds.Add("tokenHash", null);
                ds.Add("priv", ld.destUser.priv);
                ds.Add("role", ld.destUser.role);
                logEvt.Add("destUser", ds);
            }

            logEvt.Add("eventClass", null);
            logEvt.Add("externalId", null);
            logEvt.Add("name", "BOSS");
            logEvt.Add("destHost", null);
            logEvt.Add("sourceHost", null);
            logEvt.Add("sourceUser", null);
            logEvt.Add("request", null);
            logEvt.Add("fileRecord", null);
            logEvt.Add("start", null);
            logEvt.Add("end", null);
            logEvt.Add("act", null);
            logEvt.Add("outcome", null);
            logEvt.Add("reason", null);
            logEvt.Add("appProto", null);
            logEvt.Add("txProto", null);
            logEvt.Add("tags", null);
            logEvt.Add("additionalFields", null);
            Log.For(this).Info("inside Produce: step 3");
            return new Message<Null, GenericRecord> { Value = logEvt };
        }
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }
}
