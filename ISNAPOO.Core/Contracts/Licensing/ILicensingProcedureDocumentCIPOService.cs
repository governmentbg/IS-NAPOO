using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Licensing
{
    public interface ILicensingProcedureDocumentCIPOService : IBaseService
    {
        Task<MemoryStream> GenerateLicensingApplication(CandidateProviderVM candidateProvider, IEnumerable<KeyValueVM> kvApplicationFilingSource, IEnumerable<KeyValueVM> kvReceiveLicenseSource, IEnumerable<KeyValueVM> kvVQSSource);

        Task<long> GenerateLicenseNumberAsync(CandidateProviderVM candidateProvider);

        Task<MemoryStream> GenerateFirstLicensingAsync(CandidateProviderVM candidateProvider, ProcedureDocumentVM procedureDocument);
        Task<MemoryStream> GenerateApplication_1(CPOLicensingApplication1 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_2(CPOLicensingApplication2 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_3(CPOLicensingApplication3 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_4(CPOLicensingApplication4 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_5(CPOLicensingApplication5 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_6(CPOLicensingApplication6 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_7(CPOLicensingApplication7 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_8(CPOLicensingApplication8 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_9(CPOLicensingApplication9 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_10(CPOLicensingApplication10 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_11(CPOLicensingApplication11 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_13(CPOLicensingApplication13 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_14(CPOLicensingApplication14 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_15(CPOLicensingApplication15 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_16(CPOLicensingApplication16 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_17(CPOLicensingApplication17 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_18(CPOLicensingApplication18 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_19(CPOLicensingApplication19 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_20(CPOLicensingApplication20 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_21(CPOLicensingApplication21 application, TemplateDocumentVM templateVM);
    }
}
