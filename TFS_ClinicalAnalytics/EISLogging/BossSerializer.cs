using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry;

namespace EISLogging
{
    class BossSerializer<T> : IAsyncSerializer<T>
    {
        private bool autoRegisterSchema = true;
        private int initialBufferSize = DefaultInitialBufferSize;

        private IBossSerializerImpl<T> serializerImpl;

        /// <summary>
        ///     The default initial size (in bytes) of buffers used for message 
        ///     serialization.
        /// </summary>
        public const int DefaultInitialBufferSize = 1024;
        public BossSerializer(IEnumerable<KeyValuePair<string, string>> config = null)
        {
            if (config == null) { return; }
        }

        /// <summary>
        ///     Serialize an instance of type <typeparamref name="T"/> to a byte array in Avro format. The serialized
        ///     data is preceeded by a "magic byte" (1 byte) and the id of the schema as registered
        ///     in Confluent's Schema Registry (4 bytes, network byte order). This call may block or throw 
        ///     on first use for a particular topic during schema registration.
        /// </summary>
        /// <param name="value">
        ///     The value to serialize.
        /// </param>
        /// <param name="context">
        ///     Context relevant to the serialize operation.
        /// </param>
        /// <returns>
        ///     A <see cref="System.Threading.Tasks.Task" /> that completes with 
        ///     <paramref name="value" /> serialized as a byte array.
        /// </returns>
        public Task<byte[]> SerializeAsync(T value, SerializationContext context)
        {
            try
            {
                if (serializerImpl == null)
                {
                    serializerImpl = (IBossSerializerImpl<T>)new BossSerializerImpl(autoRegisterSchema, initialBufferSize);
                }

                return serializerImpl.Serialize(context.Topic, value, context.Component == MessageComponentType.Key);
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }
    }
}