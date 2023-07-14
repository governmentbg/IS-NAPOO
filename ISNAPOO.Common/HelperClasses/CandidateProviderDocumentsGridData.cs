using System;
using System.IO;

namespace ISNAPOO.Common.HelperClasses
{
    public class CandidateProviderDocumentsGridData
    {
        public int EntityId { get; set; }

        public string EntityType { get; set; }

        public int IdDocumentType { get; set; }

        public string DocumentTypeName { get; set; }

        public string? DocumentTitle { get; set; }

        public string? UploadedFileName { get; set; }

        public string UploadedByName { get; set; }

        public string CreationDate { get; set; }

        public MemoryStream UploadedFileStream { get; set; }

        public string FileName { get; set; }

        public bool IsAdditionalDocument { get; set; }//Допълнителни документи

        public string IsAdditionalDocumentText 
        {
            get
            {
                if (this.IsAdditionalDocument)
                {
                    return "Да";
                }
                else
                {
                    return "Не";
                } 
            }
        }

        public bool HasUploadedFile
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.UploadedFileName) && this.UploadedFileName != "#")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
