using System.Xml.Serialization;

namespace ISNAPOO.Core.XML.Course
{
    public class Students
    {
        //[XmlArray("student")]
        //[XmlArrayItem("student", typeof(Student))]
        [XmlElement("student")]
        public Student[] StudentList { get; set; }
    }
}
