using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace COD_Teamspeak.Helpers
{
    public class Serializer
    {
        public static T Deserialize<T>(string path) where T : class
        {
            var ser = new XmlSerializer(typeof(T));
            using(var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fileStream))
            {

                return (T)ser.Deserialize(reader);
            }
        }

        public static string Serialize<T>(T ObjectToSerialize)
        {
            var xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }
    }
}
