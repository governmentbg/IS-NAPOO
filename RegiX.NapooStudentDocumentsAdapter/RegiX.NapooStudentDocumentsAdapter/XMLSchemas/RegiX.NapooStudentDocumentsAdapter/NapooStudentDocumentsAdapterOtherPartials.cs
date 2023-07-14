using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoLogica.RegiX.NapooStudentDocumentsAdapter
{
    public partial class DocumentsByStudentResponse
    {
        public bool ShouldSerializeStudentDocument()
        {
            //Here removing the empty elements from the array or collection
            if (this.StudentDocument != null)
            {
                this.StudentDocument = this.StudentDocument.Where(x => x != default(StudentData)).ToList();
            }

            bool haveSomeValue = this.StudentDocument != null &&
                this.StudentDocument.Count() != 0 &&
                (
                this.StudentDocument[0].ClientID != default(Int32) ||
                this.StudentDocument[0].ClientIDSpecified != default(Boolean) ||
                this.StudentDocument[0].LicenceNumber != default(Int32) ||
                this.StudentDocument[0].LicenceNumberSpecified != default(Boolean) ||
                this.StudentDocument[0].DocumentTypeID != default(Int32) ||
                this.StudentDocument[0].DocumentTypeIDSpecified != default(Boolean) ||
                this.StudentDocument[0].EducationTypeID != default(Int32) ||
                this.StudentDocument[0].EducationTypeIDSpecified != default(Boolean) ||
                this.StudentDocument[0].QualificationDegree != default(Int32) ||
                this.StudentDocument[0].QualificationDegreeSpecified != default(Boolean) ||
                this.StudentDocument[0].GraduationYear != default(Int32) ||
                this.StudentDocument[0].GraduationYearSpecified != default(Boolean) ||
                this.StudentDocument[0].DocumentIssueDate != default(DateTime) ||
                this.StudentDocument[0].DocumentIssueDateSpecified != default(Boolean) ||
                this.StudentDocument[0].ShouldSerializeStudentIdentifier() ||
                this.StudentDocument[0].ShouldSerializeFirstName() ||
                this.StudentDocument[0].ShouldSerializeMiddleName() ||
                this.StudentDocument[0].ShouldSerializeLastName() ||
                this.StudentDocument[0].ShouldSerializeProfessionalEduCenter() ||
                this.StudentDocument[0].ShouldSerializeProfessionalEduCenterLocation() ||
                this.StudentDocument[0].ShouldSerializeDocumentType() ||
                this.StudentDocument[0].ShouldSerializeEducationType() ||
                this.StudentDocument[0].ShouldSerializeProfessionCodeAndName() ||
                this.StudentDocument[0].ShouldSerializeSubjectCodeAndName() ||
                this.StudentDocument[0].ShouldSerializeDocumentSeries() ||
                this.StudentDocument[0].ShouldSerializeDocumentSerialNumber()
                );

            if (!haveSomeValue)
            {
                this.StudentDocument = null;
            }

            return haveSomeValue;
        }

        public bool ShouldSerializeMessage()
        {
            bool haveSomeValue = !string.IsNullOrEmpty(this.Message);

            if (!haveSomeValue)
            {
                this.Message = null;
            }

            return haveSomeValue;
        }
    }
    public partial class StudentData
    {
        public bool ShouldSerializeStudentIdentifier()
        {
            bool haveSomeValue = !string.IsNullOrEmpty(this.StudentIdentifier);

            if (!haveSomeValue)
            {
                this.StudentIdentifier = null;
            }

            return haveSomeValue;
        }

        public bool ShouldSerializeFirstName()
        {
            bool haveSomeValue = !string.IsNullOrEmpty(this.FirstName);

            if (!haveSomeValue)
            {
                this.FirstName = null;
            }

            return haveSomeValue;
        }

        public bool ShouldSerializeMiddleName()
        {
            bool haveSomeValue = !string.IsNullOrEmpty(this.MiddleName);

            if (!haveSomeValue)
            {
                this.MiddleName = null;
            }

            return haveSomeValue;
        }

        public bool ShouldSerializeLastName()
        {
            bool haveSomeValue = !string.IsNullOrEmpty(this.LastName);

            if (!haveSomeValue)
            {
                this.LastName = null;
            }

            return haveSomeValue;
        }

        public bool ShouldSerializeProfessionalEduCenter()
        {
            bool haveSomeValue = !string.IsNullOrEmpty(this.ProfessionalEduCenter);

            if (!haveSomeValue)
            {
                this.ProfessionalEduCenter = null;
            }

            return haveSomeValue;
        }

        public bool ShouldSerializeProfessionalEduCenterLocation()
        {
            bool haveSomeValue = !string.IsNullOrEmpty(this.ProfessionalEduCenterLocation);

            if (!haveSomeValue)
            {
                this.ProfessionalEduCenterLocation = null;
            }

            return haveSomeValue;
        }

        public bool ShouldSerializeDocumentType()
        {
            bool haveSomeValue = !string.IsNullOrEmpty(this.DocumentType);

            if (!haveSomeValue)
            {
                this.DocumentType = null;
            }

            return haveSomeValue;
        }

        public bool ShouldSerializeEducationType()
        {
            bool haveSomeValue = !string.IsNullOrEmpty(this.EducationType);

            if (!haveSomeValue)
            {
                this.EducationType = null;
            }

            return haveSomeValue;
        }

        public bool ShouldSerializeProfessionCodeAndName()
        {
            bool haveSomeValue = !string.IsNullOrEmpty(this.ProfessionCodeAndName);

            if (!haveSomeValue)
            {
                this.ProfessionCodeAndName = null;
            }

            return haveSomeValue;
        }

        public bool ShouldSerializeSubjectCodeAndName()
        {
            bool haveSomeValue = !string.IsNullOrEmpty(this.SubjectCodeAndName);

            if (!haveSomeValue)
            {
                this.SubjectCodeAndName = null;
            }

            return haveSomeValue;
        }

        public bool ShouldSerializeDocumentSeries()
        {
            bool haveSomeValue = !string.IsNullOrEmpty(this.DocumentSeries);

            if (!haveSomeValue)
            {
                this.DocumentSeries = null;
            }

            return haveSomeValue;
        }

        public bool ShouldSerializeDocumentSerialNumber()
        {
            bool haveSomeValue = !string.IsNullOrEmpty(this.DocumentSerialNumber);

            if (!haveSomeValue)
            {
                this.DocumentSerialNumber = null;
            }

            return haveSomeValue;
        }
    }
}
