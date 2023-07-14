using System;
using System.Collections.Generic;
using System.Text;


namespace ISNAPOO.Common.HelperClasses
{

    /// <summary>
    /// Настройки на e-mail 
    /// </summary>
    public class EmailConfiguration 
    {
        public string MailServer { get; set; }
        public int Port { get; set; }

        public string TargetName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool UseSSL { get; set; }
        public string BccName { get; set; }
        public string BccEmail { get; set; }

        public string CcName { get; set; }
        public string CcEmail { get; set; }

        public bool AllowSendMail { get; set; }

        

        public void ToCopy(EmailConfiguration emailConfiguration)
        {
            this.MailServer = emailConfiguration.MailServer;
            this.Port = emailConfiguration.Port;
            this.TargetName = emailConfiguration.TargetName;
            this.Email = emailConfiguration.Email;
            this.Password = emailConfiguration.Password;
            this.UseSSL = emailConfiguration.UseSSL;
            this.CcName = emailConfiguration.CcName;
            this.CcEmail = emailConfiguration.CcEmail;
            this.BccEmail = emailConfiguration.BccEmail;
            this.BccName = emailConfiguration.BccName;
            this.AllowSendMail = emailConfiguration.AllowSendMail;
        }

        public string DebugEmailConfiguration()
        {
            string res = string.Empty;
            res += $"MailServer:{this.MailServer}\n";
            res += $"Port:{this.Port}\n";
            res += $"Email:{this.Email}\n";
            res += $"UseSSL:{this.UseSSL}\n";
            res += $"AllowSendMail:{this.AllowSendMail}\n";
            res += $"BccEmail:{this.BccEmail}\n";
            res += $"CcName:{this.CcName}\n";
            


            return res;
            
        }
    }
}
