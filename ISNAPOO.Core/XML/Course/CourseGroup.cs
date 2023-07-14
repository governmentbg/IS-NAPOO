using System;
using System.Xml.Serialization;

namespace ISNAPOO.Core.XML.Course
{
    public class CourseGroup
    {
        [XmlElement("courseName")]
        public string CourseName { get; set; } // Наименование на курса.

        [XmlElement("specialityID")]
        public int SpecialityId { get; set; } // Код на специалността по СППОО. 

        /* Номенклатура: 
        1   професионално обучение за придобиване на СПК
        2	професионално обучение по част от професия
        3	мотивационно обучение
        4	обучение за ключови компетентности
        5	професионално ориентиране и консултиране
        6	курс за правоспособност
        7	обучение по здравословни и безопасни условия на труд
        8	обучение по системи за управление на качеството(ISO)
        21	курс за правоспособност по Наредба № 1
        22	курс за правоспособност по Наредба № 7
        99	други */
        [XmlElement("courseType")]
        public int CourseType { get; set; } // Вид на курса

        [XmlElement("courseNotes")]
        public string CourseNotes { get; set; } // Допълнителни бележки към курса.

        /* Номенклатура:
        1	А
        2	Б
        3	Г
        4	Д
        5	Е
        11	неприложимо
        20	A10
        21	A11
        22	A12
        30	Б13
        31	Б14
        32	Б15
        33	Б16
        34	Б17
        35	Б18
        40	Д3
        41	Д4
        42	Д5
        43	Д6
        44	Д7
        45	Д8
        50	Е1
        51	Е2
        52	Е3
        53	Е4
        54	Е5
        55	Е6
        56	Е7
        57	Е8
        58	Е9
        59	Е10
        60	Е11
        61	Е12
        62	Е13 */
        [XmlElement("courseFrameProgram")]
        public string CourseFrameProgram { get; set; } // Рамкова програма на курса.

        // незадължително поле
        [XmlIgnore]
        public int? _CourseFrameProgram 
            => !string.IsNullOrEmpty(this.CourseFrameProgram)
                ? int.Parse(this.CourseFrameProgram)
                : null;

        /* Номенклатура:
        1	висше - магистър
        2	висше - бакалавър
        3	висше - професионален бакалавър
        11	средно със степен на професионална квалификация
        12	средно общо
        21	основно
        22	завършен начален етап или курс за ограмотяване
        23	завършен клас от средно образование
        24	придобито право за явяване на държавни зрелостни изпити за завършване на средно образование
        25	завършен първи гимназиален етап (X клас)
        26	завършен VII клас (за лица с увреждания)
        99	друго */
        [XmlElement("courseEducationalRequirements")]
        public string CourseEducationalRequirements { get; set; } // Минимални образователни изисквания.

        // незадължително поле
        [XmlIgnore]
        public int? _CourseEducationalRequirements
            => !string.IsNullOrEmpty(this.CourseEducationalRequirements)
                ? int.Parse(this.CourseEducationalRequirements)
                : null;

        [XmlElement("groupName")]
        public string GroupName { get; set; } // Наименование на групата.

        [XmlElement("groupStartDate")]
        public string GroupStartDate { get; set; } // Дата на започване.

        // незадължително поле
        [XmlIgnore]
        public DateTime? _GroupStartDate
            => !string.IsNullOrEmpty(this.GroupStartDate)
                ? DateTime.Parse(this.GroupStartDate)
                : null;

        [XmlElement("groupEndDate")]
        public string GroupEndDate { get; set; } // Дата на приключване.

        // незадължително поле
        [XmlIgnore]
        public DateTime? _GroupEndDate
            => !string.IsNullOrEmpty(this.GroupEndDate)
                ? DateTime.Parse(this.GroupEndDate)
                : null;

        [XmlElement("groupSubscribeDate")]
        public string GroupSubscribeDate { get; set; } // Крайна дата за записване.

        // незадължително поле
        [XmlIgnore]
        public DateTime? _GroupSubscribeDate
            => !string.IsNullOrEmpty(this.GroupSubscribeDate)
                ? DateTime.Parse(this.GroupSubscribeDate)
                : null;

        [XmlElement("groupAddressZipCode")]
        public int GroupAddressZipCode { get; set; } // Пощенски код на населеното място, в което се провежда обучението. За София - 1000.

        /* Номенклатура:
        1	предстоящ
        2	текущ
        3	приключил */
        [XmlElement("groupStatus")]
        public int GroupStatus { get; set; } // Статус на групата

        /* Номенклатура:
        1	дневна - с откъсване от работа (невалидно според ЗПОО, чл. 17, ал. 3)
        2	дневна - без откъсване от работа (невалидно според ЗПОО, чл. 17, ал. 3)
        3	вечерна
        4	дистанционна
        5	самостоятелна
        6	друга
        7	индивидуална
        8	обучение чрез работа (дуална система на обучение) 
        9	дневна
        10	задочна */
        [XmlElement("groupEducationForm")]
        public int GroupEducationForm { get; set; } // Форма на обучение

        /* Номенклатура:
        1	по програми и мерки за заетост
        2	по реда на Глава 7 от ЗНЗ */
        [XmlElement("groupMeasureType")]
        public string GroupMeasureType { get; set; } // Вид

        // незадължително поле
        [XmlIgnore]
        public int? _GroupMeasureType
            => !string.IsNullOrEmpty(this.GroupMeasureType)
                ? int.Parse(this.GroupMeasureType)
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
        [XmlElement("groupAssignType")]
        public int GroupAssignType { get; set; } // Основен източник на финансиране

        [XmlElement("groupCost")]
        public decimal GroupCost { get; set; } // Цена в лева.

        [XmlElement("groupDurationInHours")]
        public int GroupDurationInHours { get; set; } // Продължителност в часове.

        [XmlElement("groupExamTheoryDate")]
        public string GroupExamTheoryDate { get; set; } // Дата на провеждане на изпита по теория.

        // незадължително поле
        [XmlIgnore]
        public DateTime? _GroupExamTheoryDate
            => !string.IsNullOrEmpty(this.GroupExamTheoryDate)
                ? DateTime.Parse(this.GroupExamTheoryDate)
                : null;

        [XmlElement("groupExamPracticeDate")]
        public string GroupExamPracticeDate { get; set; } // Дата на провеждане на изпита по практика.

        // незадължително поле
        [XmlIgnore]
        public DateTime? _GroupExamPracticeDate
            => !string.IsNullOrEmpty(this.GroupExamPracticeDate)
                ? DateTime.Parse(this.GroupExamPracticeDate)
                : null;

        [XmlElement("groupExamCommissionMembers")]
        public string GroupExamCommissionMembers { get; set; } // Членове на изпитна комисия.
    }
}
