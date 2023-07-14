using System.Xml.Serialization;

namespace ISNAPOO.Core.XML.Course
{
    [XmlRoot(ElementName = "data")]
    public class CourseCollection
    {
        [XmlElement("generalInfo")]
        public GeneralInfo GeneralInfo { get; set; }

        [XmlElement("courseGroup")]
        public CourseGroup CourseGroup { get; set; }

        [XmlElement("curriculumGroup")]
        public CurriculumGroup CurriculumGroup { get; set; }

        [XmlElement("students")]
        public Students Students { get; set; }

        [XmlElement("trainers")]
        public Trainers Trainers { get; set; }
    }
}
