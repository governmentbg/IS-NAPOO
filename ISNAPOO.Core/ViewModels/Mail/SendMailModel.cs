using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ISNAPOO.Core.ViewModels.Mail
{
    public class SendMailModel
    {
        [Required(ErrorMessage = "Полето \"Заглавие\" е задължително!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Полето \"Съдържание\" е задължително!")]
        public string body { get; set; }

        public string FileName { get; set; }

        public MemoryStream? binaryFile { get; set; }
    }
}

