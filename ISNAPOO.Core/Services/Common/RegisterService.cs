using Data.Models.Data.Training;
using Data.Models.DB;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.DataViewModels.Registers;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using RegiX.Class.AVBulstat2.GetStateOfPlay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Common
{
    public class RegisterService : IRegisterService
    {
        protected AuthenticationStateProvider authenticationStateProvider;
        private ApplicationDbContext ctx;

        public RegisterService(ApplicationDbContext context, AuthenticationStateProvider authenticationStateProvider)
        {
            this.ctx = context;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<List<ProfessionalCertificateDataView>> GetProfessionalCertificateDataView(ClientCourseDocumentVM model) 
        {
            int returnRows = 1000;

            List<ProfessionalCertificateDataView> result = new List<ProfessionalCertificateDataView>();

            result = await 
                     (
                        from ccd in ctx.ClientCourseDocuments
                        join cc in ctx.ClientCourses on ccd.IdClientCourse equals cc.IdClientCourse
                        join client in ctx.Clients on cc.IdClient equals client.IdClient
                        
                        join kvSex in ctx.KeyValues on client.IdSex equals kvSex.IdKeyValue into grSex
                        from sex in grSex.DefaultIfEmpty()

                        join kvNationality in ctx.KeyValues on client.IdNationality equals kvNationality.IdKeyValue into grNationality
                        from nationality in grNationality.DefaultIfEmpty()

                        join c in ctx.Courses on cc.IdCourse equals c.IdCourse
                        join kvTypeFrameworkProgram in ctx.KeyValues on c.IdTrainingCourseType equals kvTypeFrameworkProgram.IdKeyValue

                        join p in ctx.Programs on c.IdProgram equals p.IdProgram
                        join s in ctx.Specialities on p.IdSpeciality equals s.IdSpeciality
                        join kvVQS in ctx.KeyValues on s.IdVQS equals kvVQS.IdKeyValue
                        join prof in ctx.Professions on s.IdProfession equals prof.IdProfession
                        join cp in ctx.CandidateProviders on p.IdCandidateProvider equals cp.IdCandidate_Provider
                        
                        join l in ctx.Locations on cp.IdLocationCorrespondence equals l.idLocation into grLoc
                        from locCorr in grLoc.DefaultIfEmpty()

                        join m in ctx.Municipalities on (locCorr != null ? locCorr.idMunicipality : GlobalConstants.INVALID_ID) equals m.idMunicipality into gpMunicipality
                        from municipality in gpMunicipality.DefaultIfEmpty()

                        join d in ctx.Districts on (municipality != null ? municipality.idDistrict : GlobalConstants.INVALID_ID) equals d.idDistrict into gpDistrict
                        from district in gpDistrict.DefaultIfEmpty() 

                        select new ProfessionalCertificateDataView() 
                        {
                            IdClientCourseDocument = ccd.IdClientCourseDocument,
                            IdCandidateProvider = cp.IdCandidate_Provider,
                            FirstName = cc.FirstName,
                            SecondName = cc.SecondName,
                            FamilyName = cc.FamilyName,
                            ClientIndent = cc.Indent,
                            LicenceNumber = cp.LicenceNumber,
                            LicenceDate = cp.LicenceDate,
                            ProviderName = cp.ProviderName,
                            ProviderOwner = cp.ProviderOwner,
                            AttorneyName = cp.AttorneyName,
                            ProviderPhone = cp.ProviderPhone,
                            ProviderEmail = cp.ProviderEmail,
                            ProviderAddressCorrespondence = cp.ProviderAddressCorrespondence,
                            ProfessionCodeAndName = $"{prof.Code} {prof.Name}",
                            SpecialityCodeAndNameAndVQS = $"{s.Code} {s.Name} - {kvVQS.Name}",
                            CourseName = c.CourseName,
                            StartDate = c.StartDate,
                            EndDate = c.EndDate,
                            LocationName = (locCorr != null)? locCorr.LocationName : "",
                            CourseTypeName = kvTypeFrameworkProgram.Name,
                            ClientIdSex = client.IdSex,
                            ClientSexName = (sex != null) ? sex.Name : "",
                            ClientIdNationality = client.IdNationality,
                            ClientNationalityName = (nationality != null) ? nationality.Name : "",
                            DocumentRegNo = ccd.DocumentRegNo,
                            DocumentDate = ccd.DocumentDate,


                        }
            ).Take(returnRows).ToListAsync();
            return result;

        }
    }
}
