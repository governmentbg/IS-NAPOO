using Data.Models.Common;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Common;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Tsp;
using System;
using Org.BouncyCastle.Asn1.Ocsp;

namespace ISNAPOO.Core.Services.Common
{
    public class TimeStampService : BaseService, ITimeStampService
    {
        private readonly IRepository repository;
        private readonly IUploadFileService uploadFileService;
        private readonly ISettingService settingService;
        private readonly IDataSourceService dataSourceService;

        public TimeStampService(IRepository repository, IUploadFileService uploadFileService, ISettingService settingService, IDataSourceService dataSourceService)
            : base(repository)
        {
            this.repository = repository;
            this.uploadFileService = uploadFileService;
            this.settingService = settingService;
            this.dataSourceService = dataSourceService;

        }

        public async Task GenerateTimeStampFilesAsync(NotificationVM notification, bool generateForCPO)
        {
            var notificationInfoArr = this.GenerateNotificationFilesAndTextNames(notification, generateForCPO);
            var txtFileName = notificationInfoArr[0];
            var tsrFileName = notificationInfoArr[1];
            var notificationText = notificationInfoArr[2].Trim();
            var timeStampResponseAsBytes = await this.GenerateTimeStampRequestAndResponseFiles(notificationText);
            var memoryStreamFromTimeStampResponse = new MemoryStream(timeStampResponseAsBytes);

            await this.uploadFileService.UploadTimeStampFilesAsync(memoryStreamFromTimeStampResponse, tsrFileName, txtFileName, notificationText, notification.IdNotification);
        }

        private async Task<byte[]> GenerateTimeStampRequestAndResponseFiles(string notificationText)
        {
           // SHA256 sha256 = SHA256CryptoServiceProvider.Create();
            SHA256 sha256 = SHA256.Create();

            byte[] hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(notificationText));

            TimeStampRequestGenerator reqGen = new TimeStampRequestGenerator();
            reqGen.SetCertReq(true);

            TimeStampRequest tsReq = reqGen.Generate(TspAlgorithms.Sha256, hash, Org.BouncyCastle.Math.BigInteger.ValueOf(100));
            byte[] tsData = tsReq.GetEncoded();

            var tsaValue = (await this.dataSourceService.GetSettingByIntCodeAsync("TSA")).SettingValue;
            var tsaUser = (await this.dataSourceService.GetSettingByIntCodeAsync("TSAUser")).SettingValue;
            var tsaPassword = (await this.dataSourceService.GetSettingByIntCodeAsync("TSAPassword")).SettingValue;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(tsaValue);


            string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{tsaUser}:{tsaPassword}"));

            req.Headers.Add("Authorization", $"Basic {encoded}");
            req.Method = "POST";
            //req.ContentType = "application/timestamp-query";
            req.ContentType = "timestamp/query";
            req.ContentLength = tsData.Length;

            Stream reqStream = req.GetRequestStream();
            reqStream.Write(tsData, 0, tsData.Length);
            reqStream.Close();

            try
            {
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                if (res != null)
                {
                    Stream resStream = new BufferedStream(res.GetResponseStream());

                    //StreamReader reader = new StreamReader(resStream);
                    //string text = reader.ReadToEnd();


                    TimeStampResponse tsRes = new TimeStampResponse(resStream);
                    byte[] tsResData = tsRes.GetEncoded();
                    resStream.Close();

                    return tsResData;
                }
            }
            catch (Exception e)
            {

                throw e;
            }
           

            return new byte[0];
        }

        private string[] GenerateNotificationFilesAndTextNames(NotificationVM notification, bool generateForCPO)
        {
            string text = string.Empty;
            string txtName = string.Empty;
            string tsrName = string.Empty;
            if (generateForCPO)
            {
                var timeStr = notification.SendDate.Value.AddHours(-3);
                txtName = "NotificationForCPO.txt";
                tsrName = "TimeStampResponseForCPO.tsr";
                text = $"{timeStr.ToString("dd.MM.yyyy hh:mm:ss")} GMT";
            }
            else
            {
                var timeStr = notification.ReviewDate.Value.AddHours(-3);
                txtName = "NotificationForNAPOO.txt";
                tsrName = "TimeStampResponseForNAPOO.tsr";
                text = $"{timeStr.ToString("dd.MM.yyyy hh:mm:ss")} GMT";
            }

            return new string[] { txtName, tsrName, text };
        } 
    }
}
