
namespace ISNAPOO.Core.Contracts.Licensing
{
    using System.IO;
    using System.Threading.Tasks;
    using ISNAPOO.Core.ViewModels.Common;
    using ISNAPOO.Core.ViewModels.CPO.LicensingChangeProcedureDoc;

    public interface ILicensingChangeProcedureCPOService
    {
        //public MemoryStream GenerateApplication_1(CPOLicenseChangeApplication1 application);

        //public MemoryStream GenerateApplication_2(CPOLicenseChangeApplication2 application);

        //public MemoryStream GenerateApplication_3(CPOLicenseChangeApplication3 application);

        //public MemoryStream GenerateApplication_4(CPOLicenseChangeApplication4 application);

        //public MemoryStream GenerateApplication_6(CPOLicenseChangeApplication6 application);

        //public MemoryStream GenerateApplication_7(CPOLicenseChangeApplication7 application);

        //public MemoryStream GenerateApplication_8(CPOLicenseChangeApplication8 application);

        //public MemoryStream GenerateApplication_9(CPOLicenseChangeApplication9 application);

        //public MemoryStream GenerateApplication_10(CPOLicenseChangeApplication10 application);

        //public MemoryStream GenerateApplication_11(CPOLicenseChangeApplication11 application);

        //public MemoryStream GenerateApplication_13(CPOLicenseChangeApplication13 application);

        //public MemoryStream GenerateApplication_14(CPOLicenseChangeApplication14 application);

        //public MemoryStream GenerateApplication_15(CPOLicenseChangeApplication15 application);

        //public MemoryStream GenerateApplication_16(CPOLicenseChangeApplication16 application);

        //public MemoryStream GenerateApplication_17(CPOLicenseChangeApplication17 application);

        //public MemoryStream GenerateApplication_18(CPOLicenseChangeApplication18 application);

        //public MemoryStream GenerateApplication_19(CPOLicenseChangeApplication19 application);

        //public MemoryStream GenerateApplication_20(CPOLicenseChangeApplication20 application);

        //public MemoryStream GenerateApplication_21(CPOLicenseChangeApplication21 application);

        Task<MemoryStream> GenerateApplication_1(CPOLicenseChangeApplication1 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_2(CPOLicenseChangeApplication2 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_3(CPOLicenseChangeApplication3 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_4(CPOLicenseChangeApplication4 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_6(CPOLicenseChangeApplication6 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_7(CPOLicenseChangeApplication7 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_8(CPOLicenseChangeApplication8 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_9(CPOLicenseChangeApplication9 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_10(CPOLicenseChangeApplication10 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_11(CPOLicenseChangeApplication11 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_13(CPOLicenseChangeApplication13 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_14(CPOLicenseChangeApplication14 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_15(CPOLicenseChangeApplication15 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_16(CPOLicenseChangeApplication16 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_17(CPOLicenseChangeApplication17 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_18(CPOLicenseChangeApplication18 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_19(CPOLicenseChangeApplication19 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_20(CPOLicenseChangeApplication20 application, TemplateDocumentVM templateVM);
        Task<MemoryStream> GenerateApplication_21(CPOLicenseChangeApplication21 application, TemplateDocumentVM templateVM);
    }
}
