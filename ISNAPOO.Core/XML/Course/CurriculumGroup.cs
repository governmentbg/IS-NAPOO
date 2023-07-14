using System.Xml.Serialization;

namespace ISNAPOO.Core.XML.Course
{
    public class CurriculumGroup
    {
        [XmlArray("module")]
        [XmlArrayItem("module", typeof(Module))]
        public Module[] Modules { get; set; }
    }
}
