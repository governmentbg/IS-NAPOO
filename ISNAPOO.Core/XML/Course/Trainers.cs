using System.Xml.Serialization;

namespace ISNAPOO.Core.XML.Course
{
    public class Trainers
    {
        //[XmlArray("trainer")]
        //[XmlArrayItem("trainer", typeof(Trainer))]
        [XmlElement("trainer")]
        public Trainer[] TrainerList { get; set; }
    }
}
