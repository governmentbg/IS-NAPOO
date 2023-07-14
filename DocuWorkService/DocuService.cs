using DocuServiceReference;
using DocuWorkService.ViewModel;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DocuWorkService
{
    public class DocuService : IDocuService
    {
        private readonly ApplicationSetting applicationSetting;

        private readonly DocuServiceSOAPClient client;
        public DocuService(IOptions<ApplicationSetting> _applicationSetting)
        {
            applicationSetting = _applicationSetting.Value;

            if (applicationSetting.EndpointConfigurationDocuService == "DocuServicePortHTTP")
            {
                this.client = new DocuServiceSOAPClient(DocuServiceSOAPClient.EndpointConfiguration.DocuServicePortHTTP, applicationSetting.DocuServiceURL);
            }
            else if (applicationSetting.EndpointConfigurationDocuService == "DocuServicePort")
            {
                this.client = new DocuServiceSOAPClient(DocuServiceSOAPClient.EndpointConfiguration.DocuServicePort, applicationSetting.DocuServiceURL);
            }
            else
            {
                throw new Exception("НЕ Е НАСТРОЕН ПРАВИЛНАТА КОНФИГУРАЦИЯ ЗА ВРЪЗКАТА С ДЕЛОВОДНАТА СИСТЕМА. Виж ");
            }

        }
        public async Task<ResultContext<RegisterDocumentResponse>> RegisterDocumentAsync(RegisterDocumentParams Params, DocData Data)
        {
            ResultContext<RegisterDocumentResponse> context = new ResultContext<RegisterDocumentResponse>();

            try
            {
                foreach(var file in Data.File)
                {
                    file.Official = true;
                }

                var registerDocumentResponse = await client.RegisterDocumentAsync(Params, Data);

                context.ResultContextObject = registerDocumentResponse;
            }
            catch (System.ServiceModel.EndpointNotFoundException e)
            {
                context.AddErrorMessage("Няма връзка с деловодната система.");
            }
            catch (Exception e)
            {
                context.AddErrorMessage("Няма намерен документ в деловодната система!");
            }

            return context;
        }

        public async Task<ResultContext<GetDocumentResponse>> GetDocumentAsync(int DocID, string GUID)
        {
            var context = new ResultContext<GetDocumentResponse>();

            try
            {
                var doc = await client.GetDocumentAsync(DocID, GUID);

                context.ResultContextObject = doc;

            }
            catch (System.ServiceModel.EndpointNotFoundException e)
            {
                context.AddErrorMessage("Няма връзка с деловодната система.");
            }
            catch (Exception e)
            {
                context.AddErrorMessage("Няма намерен документ в деловодната система!");
            }

            return context;
        }

        public async Task<GetDocOfficialByWorkResponse> GetDocOfficialByWorkAsync(int WorkDocID, string WorkGUID)
        {


            return await client.GetDocOfficialByWorkAsync(WorkDocID, WorkGUID);
        }

        public async Task<GetFileResponse> GetFileAsync(int FileID, string GUID)
        {


            return await client.GetFileAsync(FileID, GUID);
        }

        public async Task<IdentDocumentResponse> GetIdentDocument(string DocNumber, int DeloSerial, DateTime DocDate)
        {

            var result = await client.IdentDocumentAsync(DocNumber, DeloSerial, DocDate);

            return result;
        }

        public async Task<ResultContext<DocData>> CheckAndGetDocument(GetDocumentVM documentVM)
        {
            var context = new ResultContext<DocData>();

            try
            {
                if (documentVM.OfficialDocID.HasValue && !string.IsNullOrEmpty(documentVM.OfficialGUID))
                {
                    var doc = (await GetDocumentAsync(documentVM.OfficialDocID.Value, documentVM.OfficialGUID));

                    if (doc is not null)
                    {
                        context.ResultContextObject = doc.ResultContextObject.Doc;
                    }
                }
                else
                 if (documentVM.DocID.HasValue && !string.IsNullOrEmpty(documentVM.GUID))
                {
                    context.ResultContextObject = (await GetDocumentAsync(documentVM.DocID.Value, documentVM.GUID)).ResultContextObject.Doc;
                }
                else
                if (!string.IsNullOrEmpty(documentVM.OfficialDocNumber) && documentVM.OfficialDocDate.HasValue)
                {
                    var documents = await this.GetIdentDocument(documentVM.OfficialDocNumber, documentVM.DeloSerial, documentVM.OfficialDocDate.Value);

                    bool IsEmpty = true;

                    foreach (var document in documents.DocIdent)
                    {
                        var response = await this.GetDocumentAsync(document.DocID, document.GUID);

                        if  ((documentVM.VidCode != 0 && response.ResultContextObject.Doc.DocVidCode == documentVM.VidCode) 
                            || (documentVM.VidCodes is not null && documentVM.VidCodes.Contains(response.ResultContextObject.Doc.DocVidCode)))
                        {
                            context.ResultContextObject = response.ResultContextObject.Doc;
                            IsEmpty = false;
                        }
                    }

                    if (IsEmpty)
                    {
                        context.AddErrorMessage("Няма намерен документ в деловодната система!");
                    }
                }

                if (context.ResultContextObject.File.Any(x => x.Official))
                {
                    context.ResultContextObject.File = context.ResultContextObject.File.Where(x => x.Official).ToArray();
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException e)
            {
                context.AddErrorMessage("Няма връзка с деловодната система.");
            }
            catch (Exception e)
            {
                context.AddErrorMessage("Няма намерен документ в деловодната система!");
            }

            return context;
        }

    }
}
