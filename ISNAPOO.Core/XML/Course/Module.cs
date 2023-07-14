using System.Xml.Serialization;

namespace ISNAPOO.Core.XML.Course
{
    public class Module
    {
        /* Номенклатура: 
        3	А1 - Обща
        4	А2 - Теория
        6	А2 - Практика
        5	А3 - Теория
        7	А3 - Практика
        8	Б - Разширена */
        [XmlElement("curricType")]
        public int CurricType { get; set; } // Вид.

        [XmlElement("name")]
        public string Name { get; set; } // Наименование на предмета.

        [XmlElement("duration")]
        public int Duration { get; set; } // Продължителност в часове.
    }
}
