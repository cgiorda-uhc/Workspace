using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EISLogging
{
    public interface IBossSerializerImpl<T>
    {
        Task<byte[]> Serialize(string topic, T data, bool isKey);
    }
}
