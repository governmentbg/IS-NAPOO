using DocuServiceReference;
using DocuWorkService.ViewModel;
using ISNAPOO.Common.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuWorkService
{
    public interface IDocuService
    {
        public Task<ResultContext<RegisterDocumentResponse>> RegisterDocumentAsync(RegisterDocumentParams Params, DocData Data);

        public Task<ResultContext<GetDocumentResponse>> GetDocumentAsync(int DocID, string GUID);

        public Task<GetDocOfficialByWorkResponse> GetDocOfficialByWorkAsync(int WorkDocID, string WorkGUID);

        public Task<GetFileResponse> GetFileAsync(int FileID, string GUID);

        public Task<IdentDocumentResponse> GetIdentDocument(string DocNumber, int DeloSerial, DateTime DocDate);

        public Task<ResultContext<DocData>> CheckAndGetDocument(GetDocumentVM documentVM);

    }
}
