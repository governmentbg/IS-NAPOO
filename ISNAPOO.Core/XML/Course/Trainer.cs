using System.Xml.Serialization;

namespace ISNAPOO.Core.XML.Course
{
    public class Trainer
    {
        [XmlElement("trainerID")]
        public string TrainerId { get; set; } // ЕГН на преподавателя.

        [XmlElement("trainerIDtype")]
        public int TrainerIdType { get; set; } // Вид на идентификатора.  0 = ЕГН, 1 = ЛНЧ.

        [XmlElement("trainerBirthYear")]
        public string TrainerBirthYear { get; set; } // Година на раждане на преподавателя

        // незадължително поле
        [XmlIgnore]
        public int? _TrainerBirthYear
            => !string.IsNullOrEmpty(this.TrainerBirthYear)
                ? int.Parse(this.TrainerBirthYear)
                : null;

        [XmlElement("trainerGender")]
        public string TrainerGender { get; set; } // Пол на преподавателя. 1 = мъж, 2 = жена.

        // незадължително поле
        [XmlIgnore]
        public int? _TrainerGender
            => !string.IsNullOrEmpty(this.TrainerGender)
                ? int.Parse(this.TrainerGender)
                : null;

        [XmlElement("trainerFirstName")]
        public string TrainerFirstName { get; set; } // Име на преподавателя.

        [XmlElement("trainerMiddleName")]
        public string TrainerMiddleName { get; set; } // Презиме на преподавателя.

        [XmlElement("trainerLastName")]
        public string TrainerLastName { get; set; } // Фамилия на преподавателя.

        [XmlElement("trainerNationality")]
        public string TrainerNationality { get; set; } // Гражданство на преподавателя. Виж номенклатурата.

        // незадължително поле
        [XmlIgnore]
        public int? _TrainerNationality
            => !string.IsNullOrEmpty(this.TrainerNationality)
                ? int.Parse(this.TrainerNationality)
                : null;

        [XmlElement("trainerIsTheory")]
        public string TrainerIsTheory { get; set; } // Преподава ли по теория. 1  = да.

        // незадължително поле
        [XmlIgnore]
        public int? _TrainerIsTheory
            => !string.IsNullOrEmpty(this.TrainerIsTheory)
                ? int.Parse(this.TrainerIsTheory)
                : null;

        [XmlElement("trainerIsPractice")]
        public string TrainerIsPractice { get; set; } // Преподава ли по практика. 1  = да.

        // незадължително поле
        [XmlIgnore]
        public int? _TrainerIsPractice
            => !string.IsNullOrEmpty(this.TrainerIsPractice)
                ? int.Parse(this.TrainerIsPractice)
                : null;
    }
}
