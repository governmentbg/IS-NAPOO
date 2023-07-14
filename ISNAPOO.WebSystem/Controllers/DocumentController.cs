using System.Linq;
using System.Text;
using System.Web;
using Data.Models.DB;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Blazor.RichTextEditor;

namespace ISNAPOO.WebSystem.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IDataSourceService _dataSourceService;
        public ILogger<DocumentController> _logger { get; set; }

        private readonly ApplicationDbContext context;

        public DocumentController(  ILogger<DocumentController> logger, IDataSourceService dataSourceService, ApplicationDbContext applicationDbContext)
        {
            this.context = applicationDbContext;
            this._dataSourceService = dataSourceService;
            this._logger = logger;
        }
  
        public ActionResult DownloadClientDocument(string TokenString)
        {

            ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
            tokenContext.ResultContextObject.Token = TokenString;

            try
            {
                tokenContext = BaseHelper.GetDecodeToken(tokenContext);
            }
            catch (Exception)
            {  
                throw new Exception(String.Format("Не е намерен файл."));
            }


            var resourcesFolderName = this.context.Settings.Where(x => x.SettingIntCode == "ResourcesFolderName").First().SettingValue;
          
            var linkSetting = this.context.Settings.Where(x => x.SettingIntCode == "NapooOldISDocsLink").First().SettingValue;


            var IsMigrate = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "IsMigrate").Value.ToString();

            if (IsMigrate == "true")
            {
                var document_1_file_name = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "document_1_file_name").Value.ToString();

                string fileName = $"{resourcesFolderName}\\{document_1_file_name}";

                if (System.IO.File.Exists(fileName))
                {
                    var fileNameOnly = Path.GetFileName(fileName);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(fileName);

                    fileName = HttpUtility.UrlEncode(fileNameOnly, Encoding.UTF8);

                    return File(fileBytes, MimeTypeMap.GetMimeType(fileNameOnly), fileName);
                }
                else
                {
                    return NotFound();
                }
            }
            else 
            {
                var oid = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "oid").Value.ToString();

               

                System.Net.HttpWebRequest request = null;
                System.Net.HttpWebResponse response = null;
                request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create($"{linkSetting}{oid}");
                request.Timeout = 30000;
                request.UserAgent = ".NET Client";
                response = (System.Net.HttpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();

                var fileNameFromOldIS = response.Headers["Content-Disposition"];

                fileNameFromOldIS = response.Headers["Content-Disposition"].Replace("attachment; filename=", String.Empty).Replace("\"", String.Empty);

                var bytes = Encoding.UTF8.GetBytes(fileNameFromOldIS);

                var encoded = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("ISO-8859-1"), bytes);

                fileNameFromOldIS = Encoding.UTF8.GetString(encoded);

                MemoryStream ms = new MemoryStream();
                responseStream.CopyTo(ms);
                 
                responseStream.Close();

                fileNameFromOldIS = HttpUtility.UrlEncode(fileNameFromOldIS, Encoding.UTF8);


                return File(ms.ToArray(), MimeTypeMap.GetMimeType(fileNameFromOldIS), fileNameFromOldIS);
            }
             
        }
    }
}
