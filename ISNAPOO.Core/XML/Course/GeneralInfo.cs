using System.Xml.Serialization;

namespace ISNAPOO.Core.XML.Course
{
    public class GeneralInfo
    {
        [XmlElement("companyID")]
        public int CompanyId { get; set; } // Номер на лицензията на центъра.

        [XmlElement("companyName")] // Наименование на юридическото лице.
        public string CompanyName { get; set; }

        [XmlElement("companyBulstat")] 
        public string CompanyBulstat { get; set; } // Булстат на юридическото лице

        [XmlElement("groupNo")]
        public string GroupNo { get; set; } // № на групата при генериране на файла от ИС на АЗ. Ако файлът се генерира от ИС на центъра, полето следва да бъде пропуснато.

        // незадължително поле
        [XmlIgnore]
        public int? _GroupNo
            => !string.IsNullOrEmpty(this.GroupNo)
            ? int.Parse(this.GroupNo)
            : null;
    }
}
