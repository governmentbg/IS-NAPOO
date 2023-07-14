using System.Xml.Linq;
using System.Xml.Serialization;

namespace RegiX
{
    public class RegiXSerializerService
    {
        public static T
           GetXDocument<T>(XDocument xDocument)
        {
            using (var ms = new MemoryStream())
            {
                xDocument.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var serializer = new XmlSerializer(typeof(T));
#pragma warning disable CS8603 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                return (T)serializer.Deserialize(ms);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8603 // Converting null literal or possible null value to non-nullable type.
            }
        }
    }
}