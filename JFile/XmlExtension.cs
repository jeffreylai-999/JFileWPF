using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace JFile
{
    class XmlExtension
    {
        public static void WriteToXmlFile<T>(string filePath, List<T> objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(List<T>));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public static List<T> ReadFromXmlFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(List<T>));
                reader = new StreamReader(filePath);
                return (List<T>)serializer.Deserialize(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
