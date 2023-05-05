using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Avro.Generic;
using Avro.IO;
using System.IO;

namespace EISLogging
{
    class BossSerializerImpl : IBossSerializerImpl<GenericRecord>
    {
        private bool autoRegisterSchema;
        private int initialBufferSize;

        private Dictionary<global::Avro.RecordSchema, string> knownSchemas = new Dictionary<global::Avro.RecordSchema, string>();
        private HashSet<KeyValuePair<string, string>> registeredSchemas = new HashSet<KeyValuePair<string, string>>();
        private Dictionary<string, int> schemaIds = new Dictionary<string, int>();

        public BossSerializerImpl(
            bool autoRegisterSchema,
            int initialBufferSize)
        {
            this.autoRegisterSchema = autoRegisterSchema;
            this.initialBufferSize = initialBufferSize;
        }

        /// <summary>
        ///     Serialize GenericRecord instance to a byte array in Avro format. The serialized
        ///     data is preceeded by a "magic byte" (1 byte) and the id of the schema as registered
        ///     in Confluent's Schema Registry (4 bytes, network byte order). This call may block or throw 
        ///     on first use for a particular topic during schema registration.
        /// </summary>
        /// <param name="topic">
        ///     The topic associated wih the data.
        /// </param>
        /// <param name="data">
        ///     The object to serialize.
        /// </param>
        /// <param name="isKey">
        ///     whether or not the data represents a message key.
        /// </param>
        /// <returns>
        ///     <paramref name="data" /> serialized as a byte array.
        /// </returns>
        public Task<byte[]> Serialize(string topic, GenericRecord data, bool isKey)
        {
            try
            {
                global::Avro.RecordSchema writerSchema;
                writerSchema = data.Schema;
                using (var stream = new MemoryStream(initialBufferSize))
                using (var writer = new BinaryWriter(stream))
                {
                    //stream.WriteByte(Constants.MagicByte);
                    //writer.Write(IPAddress.HostToNetworkOrder(schemaId));
                    new GenericWriter<GenericRecord>(writerSchema)
                        .Write(data, new BinaryEncoder(stream));
                    return Task.FromResult(stream.ToArray());
                }
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }
    }
}
