////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.TelegramMetadata
{
    [DataContract]
    public class SerialiserJSON
    {
        public string GetAsString()
        {
            MemoryStream m_stream = new MemoryStream();
            DataContractJsonSerializer ser;
            StreamReader sr;
            string s;
            //
            ser = new DataContractJsonSerializer(GetType());
            ser.WriteObject(m_stream, this);
            m_stream.Position = 0;
            sr = new StreamReader(m_stream);
            s = sr.ReadToEnd();
            return s;
        }

        public byte[] GetAsBytes()
        {
            return Encoding.UTF8.GetBytes(GetAsString());
        }

        public async static Task<object> ReadObject(Type t, string json)
        {
            if (string.IsNullOrEmpty(json))
                return new object();

            using (MemoryStream ms = await GetStream(json))
            {
                return new DataContractJsonSerializer(t).ReadObject(ms);
            }
        }

        public async static Task<MemoryStream> GetStream(string data)
        {
            MemoryStream my_stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(my_stream);
            await writer.WriteAsync(data);
            writer.Flush();
            my_stream.Position = 0;
            return my_stream;
        }
    }
}
