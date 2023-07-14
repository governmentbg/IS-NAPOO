using System;
using System.Xml.Serialization;

namespace ISNAPOO.Core.XML.Course
{
    public class Student
    {
        [XmlElement("studentID")]
        public string StudentId { get; set; } // Стойност на идентификатора на курсиста.

        [XmlElement("studentIDtype")]
        public int StudentIdType { get; set; } // Вид на идентификатора. 0 = ЕГН, 1 = ИДН.

        [XmlElement("studentGender")]
        public string StudentGender { get; set; } // Пол на курсиста. 1 = мъж, 2 = жена.

        // незадължително поле
        [XmlIgnore]
        public int? _StudentGender
            => !string.IsNullOrEmpty(this.StudentGender)
                ? int.Parse(this.StudentGender)
                : null;

        [XmlElement("studentFirstName")]
        public string StudentFirstName { get; set; } // Име на курсиста.

        [XmlElement("studentMiddleName")]
        public string StudentMiddleName { get; set; } // Презиме на курсиста.

        [XmlElement("studentLastName")]
        public string StudentLastName { get; set; } // Фамилия на курсиста.

        [XmlElement("studentBirthDate")]
        public string StudentBirthDate { get; set; } // Рождена дата на курсиста.

        // незадължително поле
        [XmlIgnore]
        public DateTime? _StudentBirthDate
            => !string.IsNullOrEmpty(this.StudentBirthDate)
                ? DateTime.Parse(this.StudentBirthDate)
                : null;

        [XmlElement("studentNationality")]
        public string StudentNationality { get; set; } // Гражданство на курсиста. Номенклатура.

        // незадължително поле
        [XmlIgnore]
        public int? _StudentNationality
            => !string.IsNullOrEmpty(this.StudentNationality)
                ? int.Parse(this.StudentNationality)
                : null;

        /* Номенклатура: 
        1	завършил с документ
        2	прекъснал по уважителни причини
        3	прекъснал по неуважителни причини
        4	завършил курса, но не положил успешно изпита
        5	придобил СПК по реда на чл.40 от ЗПОО */
        [XmlElement("studentGraduationStatus")]
        public string StudentGraduationStatus { get; set; } // Допълнителни данни за завършване на курсиста.

        // незадължително поле
        [XmlIgnore]
        public int? _StudentGraduationStatus
            => !string.IsNullOrEmpty(this.StudentGraduationStatus)
                ? int.Parse(this.StudentGraduationStatus)
                : null;

        [XmlElement("studentEndDate")]
        public string StudentEndDate { get; set; } // Дата на приключване на курса от курсиста.

        // незадължително поле
        [XmlIgnore]
        public DateTime? _StudentEndDate
            => !string.IsNullOrEmpty(this.StudentEndDate)
                ? DateTime.Parse(this.StudentEndDate)
                : null;

        /* Номенклатура:
        1	по заявка на АЗ - ДБТ
        2	По заявка от работодател
        3	Заплащане на такса от обучаемите
        4	други програми - фондове от ЕС (не се прилага сред
        5	други програми - национални фондове (не се прилага
        6	програма "Аз мога" (не се прилага сред 1.1.2016)
        7	Други
        8	ОП Развитие на човешките ресурси (по проекти)
        9	ОП Развитие на човешките ресурси (с ваучери)
        10	Активни мерки на пазара на труда (от държавния бюджет) */
        [XmlElement("studentAssignType")]
        public string StudentAssingType { get; set; } // Финансиране

        // незадължително поле
        [XmlIgnore]
        public int? _StudentAssingType
            => !string.IsNullOrEmpty(this.StudentAssingType)
                ? int.Parse(this.StudentAssingType)
                : null;

        /* Номенклатура:
        1	Свидетелство за придобита СПК
        2	Удостоверение за професионално обучение
        3	Свидетелство за правоспособност
        4	Удостоверение за правоспособност
        5	Удостоверение за правоспособност (първа степен)
        6	Удостоверение за правоспособност (втора степен)
        7	Удостоверение за правоспособност (трета степен)
        8	Удостоверение / Сертификат
        9	Свидетелство за валидиране на ПК
        10	Удостоверение за валидиране на ПК по част от проф.
        11	Дубликат на свидетелство за ПК
        12	Дубликат на свидетелство за валидиране на ПК */
        [XmlElement("documentType")]
        public string DocumentType { get; set; } // Вид на документа

        // незадължително поле
        [XmlIgnore]
        public int? _DocumentType
            => !string.IsNullOrEmpty(this.DocumentType)
                ? int.Parse(this.DocumentType)
                : null;

        [XmlElement("documentGraduationYear")]
        public string DocumentGraduationYear { get; set; } // Година на завършване на курсиста.

        // незадължително поле
        [XmlIgnore]
        public int? _DocumentGraduationYear
            => !string.IsNullOrEmpty(this.DocumentGraduationYear)
                ? int.Parse(this.DocumentGraduationYear)
                : null;

        [XmlElement("documentSeries")]
        public string DocumentSeries { get; set; } // Серия на документа.

        [XmlElement("documentSerialNumber")]
        public string DocumentSerialNumber { get; set; } // Сериен номер на документа.

        [XmlElement("documentRegistrationNumber")]
        public string DocumentRegistrationNumber { get; set; } // Регистрационен номер на документа.

        [XmlElement("documentPublishingDate")]
        public string DocumentPublishingDate { get; set; } // Дата на издаване на документа.

        // незадължително поле
        [XmlIgnore]
        public DateTime? _DocumentPublishingDate
            => !string.IsNullOrEmpty(this.DocumentPublishingDate)
                ? DateTime.Parse(this.DocumentPublishingDate)
                : null;

        [XmlElement("documentProtocolNumber")]
        public string DocumentProtocolNumber { get; set; } // Номер на протокола.

        [XmlElement("documentTheoryGrade")]
        public string DocumentTheoryGrade { get; set; } // Оценка по теория.

        // незадължително поле
        [XmlIgnore]
        public double? _DocumentTheoryGrade
            => !string.IsNullOrEmpty(this.DocumentTheoryGrade)
                ? double.Parse(this.DocumentTheoryGrade)
                : null;

        [XmlElement("documentPracticeGrade")]
        public string DocumentPracticeGrade { get; set; } // Оценка по практика.

        // незадължително поле
        [XmlIgnore]
        public double? _DocumentPracticeGrade
            => !string.IsNullOrEmpty(this.DocumentPracticeGrade)
                ? double.Parse(this.DocumentPracticeGrade)
                : null;
    }
}
