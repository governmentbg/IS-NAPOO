using Data.Models.Common;
using Data.Models.Data.Common;
using Data.Models.Data.ExternalExpertCommission;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.ExternalExpertCommission
{
    public class ExpertService : BaseService, IExpertService
    {
        private readonly IRepository repository;
        private readonly IPersonService personService;
        private readonly IDataSourceService dataSourceService;
        private readonly IApplicationUserService applicationUserService;
        private readonly IDOCService docService;

        public ExpertService(IRepository repository, IPersonService personService, IDataSourceService dataSourceService, IApplicationUserService applicationUserService, IDOCService docService) : base(repository)
        {
            this.repository = repository;
            this.personService = personService;
            this.dataSourceService = dataSourceService;
            this.applicationUserService = applicationUserService;
            this.docService = docService;
        }

        public async Task<ExpertVM> GetExpertByIdAsync(int id)
        {
            var expert = await this.repository.AllReadonly<Expert>(e => e.IdExpert == id)
                .Include(x => x.Person)
                .ThenInclude(p => p.Location)
                .ThenInclude(v => v.Municipality)
                .ThenInclude(m => m.District)
                .FirstOrDefaultAsync();

            this.repository.Detach<Expert>(expert);

            ExpertVM result = expert.To<ExpertVM>();

            return result;
        }
        public async Task<ExpertVM> GetExpertByIdPersonAsync(int id)
        {
            var expert = await this.repository.AllReadonly<Expert>(e => e.IdPerson == id)
                .Include(x => x.Person)
                .ThenInclude(p => p.Location)
                .ThenInclude(v => v.Municipality)
                .ThenInclude(m => m.District)
                .FirstOrDefaultAsync();

            this.repository.Detach<Expert>(expert);

            ExpertVM result = expert.To<ExpertVM>();

            return result;
        }

        public async Task ChangeExpertExternalOrCommissionAsync(int idExpert)
        {
            var kvExternalExpert = await dataSourceService.GetKeyValueByIntCodeAsync("ExpertType", "ExternalExpert");
            var kvActive = await dataSourceService.GetKeyValueByIntCodeAsync("ExpertStatus", "ActiveExpert");

            var context = new ResultContext<ExpertVM>();
            var expert = await this.repository.GetByIdAsync<Expert>(idExpert);
            this.repository.Detach<Expert>(expert);

            var listDirections = this.repository.All<ExpertProfessionalDirection>(e => e.IdExpert == idExpert).ToList();

            bool isExternalExpert = listDirections.Any(x => x.IdExpertType == kvExternalExpert.IdKeyValue && x.IdStatus == kvActive.IdKeyValue);

            expert.IsExternalExpert = isExternalExpert;

            this.repository.Update(expert);
            await this.repository.SaveChangesAsync();

        }

        public async Task<ResultContext<PersonVM>> UpdateExpertAsync(ResultContext<PersonVM> resultContext)
        {
            if (resultContext.ResultContextObject.IdPerson == GlobalConstants.INVALID_ID_ZERO)
            {
                resultContext = await CreatePersonAndExpertAsync(resultContext);
            }
            else
            {
                resultContext = await UpdatePersonAsync(resultContext);
            }

            return resultContext;
        }
        public async Task<ResultContext<PersonVM>> UpdatePersonEmailSentDateAsync(ResultContext<PersonVM> resultContext)
        {
            var updatedEnity = await personService.GetPersonByIdAsync(resultContext.ResultContextObject.IdPerson);
            updatedEnity.PasswordResetDate = DateTime.Now;
            var person = updatedEnity.To<Person>();
            this.repository.Detach<Person>(person);
            this.repository.Update(person);
            await this.repository.SaveChangesAsync();
            return resultContext;
        }
        private async Task<ResultContext<PersonVM>> UpdatePersonAsync(ResultContext<PersonVM> resultContext)
        {
            try
            {
                var updatedEnity = await this.GetByIdAsync<Person>(resultContext.ResultContextObject.IdPerson);
                ResultContext<ApplicationUserVM> resultContextAppUser = new ResultContext<ApplicationUserVM>();
                ApplicationUserVM applicationUser = new ApplicationUserVM();
                var expert = await this.GetByIdAsync<Expert>(resultContext.ResultContextObject.IdExpert);
                applicationUser.IdPerson = resultContext.ResultContextObject.IdPerson;
                applicationUser.Email = resultContext.ResultContextObject.Email;
                applicationUser.FirstName = resultContext.ResultContextObject.FirstName;
                applicationUser.FamilyName = resultContext.ResultContextObject.FamilyName;
                applicationUser.IsCommissionExpert = expert.IsCommissionExpert;
                applicationUser.IsExternalExpert = expert.IsExternalExpert;
                applicationUser.IsNapooExpert = expert.IsNapooExpert;
                applicationUser.IsDOCExpert = expert.IsDOCExpert;
                applicationUser.IdUserStatus = dataSourceService.GetKeyValueByIntCodeAsync("UserStatus", "Active").Result.IdKeyValue;
                resultContextAppUser.ResultContextObject = applicationUser;

                var user = await this.repository.AllReadonly<ApplicationUser>().Where(x => x.IdPerson == updatedEnity.IdPerson).FirstOrDefaultAsync();
                if (user == null)
                {
                    await applicationUserService.CreateApplicationExpertUserAsync(resultContextAppUser);
                    user = await this.repository.AllReadonly<ApplicationUser>().Where(x => x.IdPerson == updatedEnity.IdPerson).FirstOrDefaultAsync();
                }
                resultContextAppUser.ResultContextObject.UserName = user.UserName;
                await applicationUserService.UpdateApplicationExpertUserAsync(resultContextAppUser);

                updatedEnity = resultContext.ResultContextObject.To<Person>();

                updatedEnity.Location = null;
                this.repository.Detach<Person>(updatedEnity);
                this.repository.Update<Expert>(expert);
                this.repository.Update(updatedEnity);
                var result = await this.repository.SaveChangesAsync();
                resultContext.ResultContextObject.IdModifyUser = updatedEnity.IdModifyUser;
                resultContext.ResultContextObject.ModifyDate = updatedEnity.ModifyDate;
                if (result > 0)
                {
                    resultContext.AddMessage("Записът e успешeн!");
                }
                else
                {
                    resultContext.AddErrorMessage("Грешка при запис в базата!");
                }

                return resultContext;
            }
            catch (Exception е)
            {
                return resultContext;

            }
        }
        private async Task<ResultContext<PersonVM>> CreatePersonAndExpertAsync(ResultContext<PersonVM> resultContext)
        {
            try
            {

                var newPerson = resultContext.ResultContextObject.To<Person>();



                Expert newExpert = new Expert();

                newExpert.Person = newPerson;

                var viewModel = personService.SetExpertTypeFieldsToModel(resultContext.ResultContextObject.TokenExpertType);
                newExpert.IsExternalExpert = viewModel.IsExternalExpert;
                newExpert.IsCommissionExpert = viewModel.IsCommissionExpert;
                newExpert.IsNapooExpert = viewModel.IsNapooExpert;
                newExpert.IsDOCExpert = viewModel.IsDOCExpert;

                await this.repository.AddAsync<Expert>(newExpert);
                int result = await this.repository.SaveChangesAsync();


                if (result > 0)
                {
                    ResultContext<ApplicationUserVM> resultContextAppUser = new ResultContext<ApplicationUserVM>();
                    ApplicationUserVM applicationUser = new ApplicationUserVM();
                    applicationUser.IdPerson = newPerson.IdPerson;
                    applicationUser.Email = newExpert.Person.Email;
                    applicationUser.FirstName = newExpert.Person.FirstName;
                    applicationUser.FamilyName = newExpert.Person.FamilyName;
                    applicationUser.IsCommissionExpert = newExpert.IsCommissionExpert;
                    applicationUser.IsExternalExpert = newExpert.IsExternalExpert;
                    applicationUser.IsNapooExpert = newExpert.IsNapooExpert;
                    applicationUser.IsDOCExpert = newExpert.IsDOCExpert;
                    applicationUser.IdUserStatus = dataSourceService.GetKeyValueByIntCodeAsync("UserStatus", "Active").Result.IdKeyValue;

                    resultContextAppUser.ResultContextObject = applicationUser;

                    resultContextAppUser = await applicationUserService.CreateApplicationExpertUserAsync(resultContextAppUser);

                    resultContext.AddMessage("Записът e успешeн!");
                    resultContext.ResultContextObject.IdPerson = newPerson.IdPerson;
                    resultContext.ResultContextObject.IdExpert = newExpert.IdExpert;
                }
                else
                {
                    resultContext.AddErrorMessage("Грешка при запис в базата!");
                }

                return resultContext;

            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
                return resultContext;
            }
        }


        public async Task<IEnumerable<ExpertVM>> GetAllExpertsByIdProfessionalDirectionAsync(ExpertVM filterExpertVM)
        {
            IQueryable<Expert> experts = this.repository.All<Expert>(this.FilterExpertValue(filterExpertVM));
            List<ExpertVM> listExpertVMs = await experts.To<ExpertVM>(e => e.ExpertProfessionalDirections.Select(s => s.ProfessionalDirection), e => e.Person, e => e.ProcedureExternalExperts.Select(p => p.StartedProcedure)).ToListAsync();
            var kvExpertStatusActive = await this.dataSourceService.GetKeyValueByIntCodeAsync("ExpertStatus", "ActiveExpert");

            listExpertVMs = listExpertVMs.Where(e => e.ExpertProfessionalDirections.All(p => p.IdStatus == kvExpertStatusActive.IdKeyValue)).ToList();
            return listExpertVMs;
        }
        public async Task<IEnumerable<ExpertVM>> GetAllExpertsAsync(ExpertVM filterExpertVM)
        {
            IQueryable<Expert> experts = this.repository.All<Expert>(this.FilterExpertValue(filterExpertVM));

            List<ExpertVM> listExpertVMs = await experts.To<ExpertVM>(e => e.ExpertProfessionalDirections.Select(s => s.ProfessionalDirection),
                p => p.ExpertDOCs.Select(s => s.DOC),
                x => x.ExpertExpertCommissions.Select(s => s.ExpertCommissionName),
                e => e.Person,
                e => e.ProcedureExternalExperts.Select(p => p.StartedProcedure),
                d => d.ProcedureDocuments).ToListAsync();

            var dataKvExpertType = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertType");
            var dataExpertCommisisonRole = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertRoleCommission");
            var dataExpertCommisisons = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertCommission");

            var kvExpertStatusActive = await this.dataSourceService.GetKeyValueByIntCodeAsync("ExpertStatus", "ActiveExpert");
            var kvCommissionStatusActive = await this.dataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderTrainerStatus", "Active");

            List<string> listProfessionalDirectionText = new List<string>();
            List<string> listExpertExpertCommissionsText = new List<string>();
            List<string> listExpertExpertDOCsText = new List<string>();

            var docSource = await docService.GetAllDocAsync();
            //var keyValues = dataSourceService.GetAllKeyValueList();
            foreach (ExpertVM expertVM in listExpertVMs.Where(w => w.ExpertProfessionalDirections.Count > 0 || w.ExpertExpertCommissions.Count > 0 || w.ExpertDOCs.Count > 0))
            {
                listProfessionalDirectionText = new List<string>();
                listExpertExpertCommissionsText = new List<string>();
                listExpertExpertDOCsText = new List<string>();

                var activeExpertProfessionalDirections = expertVM.ExpertProfessionalDirections.Where(d => d.IdStatus == kvExpertStatusActive.IdKeyValue);
                foreach (ExpertProfessionalDirectionVM expertProfessionalDirectionVM in activeExpertProfessionalDirections)
                {
                    var kvExpertType = dataKvExpertType.FirstOrDefault(kv => kv.IdKeyValue == expertProfessionalDirectionVM.IdExpertType);

                    //listProfessionalDirectionText.Add(kvExpertType.Name + " - " + expertProfessionalDirectionVM.ProfessionalDirectionName);
                    expertVM.ProfessionalDirectionsInfo += $"{kvExpertType.Name} {expertProfessionalDirectionVM.ProfessionalDirectionName} <br />";
                    expertVM.ProfessionalDirectionsInfoTrim += $"{expertProfessionalDirectionVM.ProfessionalDirectionName} <br />";

                }

                var activeExpertExpertCommissions = expertVM.ExpertExpertCommissions.Where(c => c.IdStatus == kvCommissionStatusActive.IdKeyValue);
                foreach (ExpertExpertCommissionVM expertExpertCommissionVM in activeExpertExpertCommissions)
                {
                    var kvExpertRole = dataExpertCommisisonRole.FirstOrDefault(kv => kv.IdKeyValue == expertExpertCommissionVM.IdRole);
                    var commisisonName = dataExpertCommisisons.FirstOrDefault(k => k.IdKeyValue == expertExpertCommissionVM.IdExpertCommission).Name;

                    //listExpertExpertCommissionsText.Add(kvExpertRole.Name + " на ЕК  - " + keyValues.FirstOrDefault(k => k.IdKeyValue == expertExpertCommissionVM.IdExpertCommission).Name);
                    expertVM.CommissionsInfo += $"{kvExpertRole.Name} на ЕК  - {commisisonName} <br />";

                }

                var activeExpertDOCs = expertVM.ExpertDOCs.Where(d => d.IdStatus == kvExpertStatusActive.IdKeyValue);
                foreach (ExpertDOCVM expertDOCVM in activeExpertDOCs)
                {
                    //listExpertExpertDOCsText.Add("Рецензент на ДОС - " + docSource.FirstOrDefault(d => d.IdDOC == expertDOCVM.IdDOC).Name);
                    expertVM.DOCsInfo += $"Рецензент на ДОС - {docSource.FirstOrDefault(d => d.IdDOC == expertDOCVM.IdDOC).Name} <br />";

                }
                //expertVM.DOCsInfo = string.Join("<br />", listExpertExpertDOCsText);
                //expertVM.ProfessionalDirectionsInfo = string.Join("<br />", listProfessionalDirectionText);
                //expertVM.CommissionsInfo = string.Join("<br />", listExpertExpertCommissionsText);
            }

            return listExpertVMs.OrderBy(x => x.Person.FirstName).ThenBy(x => x.Person.SecondName).ThenBy(x => x.Person.FamilyName);
        }

        protected Expression<Func<Expert, bool>> FilterExpertValue(ExpertVM model)
        {
            var predicate = PredicateBuilder.True<Expert>();

            if (model.IsExternalExpert)
            {
                predicate = predicate.And(p => p.IsExternalExpert);
            }
            if (model.IsCommissionExpert)
            {
                predicate = predicate.And(p => p.IsCommissionExpert);
            }
            if (model.IsNapooExpert)
            {
                predicate = predicate.And(p => p.IsNapooExpert);
            }
            if (model.IsDOCExpert)
            {
                predicate = predicate.And(p => p.IsDOCExpert);
            }
            if (!string.IsNullOrEmpty(model.Person.FirstName))
            {
                predicate = predicate.And(p => p.Person.FirstName.Contains(model.Person.FirstName));
            }
            if (!string.IsNullOrEmpty(model.Person.SecondName))
            {
                predicate = predicate.And(p => p.Person.SecondName.Contains(model.Person.SecondName));
            }
            if (!string.IsNullOrEmpty(model.Person.FamilyName))
            {
                predicate = predicate.And(p => p.Person.FamilyName.Contains(model.Person.FamilyName));
            }
            if (model.Person.IdIndentType.HasValue)
            {
                predicate = predicate.And(p => p.Person.IdIndentType.HasValue && p.Person.IdIndentType.Value == model.Person.IdIndentType.Value);
            }
            if (!string.IsNullOrEmpty(model.Person.Indent))
            {
                predicate = predicate.And(p => p.Person.Indent != null && p.Person.Indent.Contains(model.Person.Indent));
            }
            if (model.Person.BirthDate.HasValue)
            {
                predicate = predicate.And(p => p.Person.BirthDate.HasValue && p.Person.BirthDate.Value == model.Person.BirthDate.Value);
            }
            if (model.Person.IdSex.HasValue)
            {
                predicate = predicate.And(p => p.Person.IdSex.HasValue && p.Person.IdSex.Value == model.Person.IdSex.Value);
            }
            if (!string.IsNullOrEmpty(model.Person.PersonalID))
            {
                predicate = predicate.And(p => p.Person.PersonalID != null && p.Person.PersonalID.Contains(model.Person.PersonalID));
            }
            if (!string.IsNullOrEmpty(model.Person.PersonalIDIssueBy))
            {
                predicate = predicate.And(p => p.Person.PersonalIDIssueBy != null && p.Person.PersonalIDIssueBy.Contains(model.Person.PersonalIDIssueBy));
            }
            if (model.Person.IdLocation.HasValue)
            {
                predicate = predicate.And(p => p.Person.IdLocation.HasValue && p.Person.IdLocation.Value == model.Person.IdLocation.Value);
            }
            if (!string.IsNullOrEmpty(model.Person.PostCode))
            {
                predicate = predicate.And(p => p.Person.PostCode != null && p.Person.PostCode.Contains(model.Person.PostCode));
            }
            if (!string.IsNullOrEmpty(model.Person.Address))
            {
                predicate = predicate.And(p => p.Person.Address != null && p.Person.Address.Contains(model.Person.Address));
            }
            if (!string.IsNullOrEmpty(model.Person.Phone))
            {
                predicate = predicate.And(p => p.Person.Phone != null && p.Person.Phone.Contains(model.Person.Phone));
            }
            if (!string.IsNullOrEmpty(model.Person.Email))
            {
                predicate = predicate.And(p => p.Person.Email != null && p.Person.Email.Contains(model.Person.Email));
            }
            if (model.IdExpertTypeFilter != GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.ExpertProfessionalDirections.Any(epd => epd.IdExpertType == model.IdExpertTypeFilter));
            }
            if (model.IdProfessionalDirectionFilter != GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.ExpertProfessionalDirections.Any(epd => epd.IdProfessionalDirection == model.IdProfessionalDirectionFilter));
            }
            if (model.DateApprovalExternalExpertFilter.HasValue)
            {
                predicate = predicate.And(p => p.ExpertProfessionalDirections.Any(epd => epd.DateApprovalExternalExpert.HasValue && epd.DateApprovalExternalExpert.Value == model.DateApprovalExternalExpertFilter.Value));
            }
            if (!string.IsNullOrEmpty(model.OrderNumberFilter))
            {
                predicate = predicate.And(p => p.ExpertProfessionalDirections.Any(epd => epd.OrderNumber != null && epd.OrderNumber == model.OrderNumberFilter));
            }
            if (model.DateOrderIncludedExpertCommissionFilter.HasValue)
            {
                predicate = predicate.And(p => p.ExpertProfessionalDirections.Any(epd => epd.DateOrderIncludedExpertCommission.HasValue && epd.DateOrderIncludedExpertCommission.Value == model.DateOrderIncludedExpertCommissionFilter.Value));
            }

            return predicate;
        }


        public async Task<int> DeleteExpertAsync(List<ExpertVM> experts)
        {
            foreach (var expert in experts)
            {
                await this.repository.HardDeleteAsync<Expert>(expert.IdExpert);
            }

            return await this.repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ExpertExpertCommissionVM>> GetAllExpertExpertCommissionsAsync(ExpertExpertCommissionVM filterVM)
        {
            try
            {
                var kvExpertCommissions = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertCommission");
                var kvExpertRoleCommissions = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertRoleCommission");
                var kvMemberTypeECSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MemberTypeECHolderAlternate");
                var keyValues = this.dataSourceService.GetAllKeyValueList();
                IQueryable<ExpertExpertCommission> experts = this.repository.All<ExpertExpertCommission>(this.FilterExpertExpertCommissionVMValue(filterVM));

                List<ExpertExpertCommissionVM> listExpertVMs = await experts.To<ExpertExpertCommissionVM>(e => e.Expert.Person).ToListAsync();


                foreach (var item in listExpertVMs)
                {
                    item.ExpertCommissionName = kvExpertCommissions.FirstOrDefault(r => r.IdKeyValue == item.IdExpertCommission)?.Name;
                    item.RoleName = kvExpertRoleCommissions.FirstOrDefault(r => r.IdKeyValue == item.IdRole)?.Name;
                    item.StatusName = keyValues.FirstOrDefault(r => r.IdKeyValue == item.IdStatus).Name;
                    item.MemberTypeString = kvMemberTypeECSource.FirstOrDefault(r => r.IdKeyValue == item.IdMemberType)?.Name;
                }

                return listExpertVMs;
            }
            catch (Exception ex)
            {

                return new List<ExpertExpertCommissionVM>();
            }
        }

        protected Expression<Func<ExpertExpertCommission, bool>> FilterExpertExpertCommissionVMValue(ExpertExpertCommissionVM model)
        {
            var predicate = PredicateBuilder.True<ExpertExpertCommission>();

            if (model.IdExpert > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdExpert == model.IdExpert);
            }
            if (model.IdExpertCommission > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdExpertCommission == model.IdExpertCommission);
            }
            if (model.IdStatus > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdStatus == model.IdStatus);
            }

            return predicate;
        }

        public async Task<ExpertExpertCommissionVM> GetExpertExpertCommissionByIdAsync(int id)
        {
            var expert = await this.repository.All<ExpertExpertCommission>(e => e.IdExpertExpertCommission == id)
                .Include(x => x.Expert)
                .FirstOrDefaultAsync();

            this.repository.Detach<ExpertExpertCommission>(expert);

            ExpertExpertCommissionVM result = expert.To<ExpertExpertCommissionVM>();

            return result;
        }
        public async Task<IEnumerable<KeyValueVM>> GetExpertExpertCommissionByExpertIdAsync(int id)
        {
            var expert = this.repository.All<ExpertExpertCommission>(e => e.IdExpert == id);
            var keyValues = dataSourceService.GetAllKeyValueList();
            List<KeyValueVM> commission = new List<KeyValueVM>();
            foreach (var item in expert)
            {
                commission.Add(keyValues.FirstOrDefault(c => c.IdKeyValue == item.IdExpertCommission));
            }

            var result = commission;

            return result;
        }

        public async Task<ResultContext<ExpertExpertCommissionVM>> SaveExpertExpertCommissionAsync(ResultContext<ExpertExpertCommissionVM> resultContext)
        {

            try
            {
                int result = 0;
                if (resultContext.ResultContextObject.IdExpertExpertCommission > GlobalConstants.INVALID_ID_ZERO)
                {
                    var updatedEnity = await this.GetByIdAsync<ExpertExpertCommission>(resultContext.ResultContextObject.IdExpertExpertCommission);

                    updatedEnity = resultContext.ResultContextObject.To<ExpertExpertCommission>();

                    updatedEnity.Expert = null;

                    this.repository.Update(updatedEnity);

                    result = await this.repository.SaveChangesAsync();
                }
                else
                {
                    var data = resultContext.ResultContextObject.To<ExpertExpertCommission>();
                    await this.repository.AddAsync<ExpertExpertCommission>(data);

                    result = await this.repository.SaveChangesAsync();
                    resultContext.ResultContextObject.IdExpertExpertCommission = data.IdExpertExpertCommission;

                }

                if (result > 0)
                {
                    var list = this.repository.All<ExpertExpertCommission>(e => e.IdExpert == resultContext.ResultContextObject.IdExpert).ToList();

                    var kvActive = await dataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderTrainerStatus", "Active");

                    bool isCommissionExpert = list.Any(x => x.IdStatus == kvActive.IdKeyValue);

                    var expert = await this.repository.GetByIdAsync<Expert>(resultContext.ResultContextObject.IdExpert);


                    expert.IsCommissionExpert = isCommissionExpert;
                    resultContext.ResultContextObject = list.FirstOrDefault(c => c.IdExpertExpertCommission == resultContext.ResultContextObject.IdExpertExpertCommission).To<ExpertExpertCommissionVM>();

                    this.repository.Update(expert);
                    await this.repository.SaveChangesAsync();

                    resultContext.AddMessage("Записът e успешeн!");

                }
                else
                {
                    resultContext.AddErrorMessage("Грешка при запис в базата!");
                }

                return resultContext;
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
                return resultContext;
            }
        }

        public async Task ChangeExpertDOCAsync(int idExpert)
        {
            var kvActive = await dataSourceService.GetKeyValueByIntCodeAsync("ExpertStatus", "ActiveExpert");
            var context = new ResultContext<ExpertDOCVM>();
            var expert = await this.repository.GetByIdAsync<Expert>(idExpert);
            this.repository.Detach<Expert>(expert);
            var listDOCs = this.repository.All<ExpertDOC>(e => e.IdExpert == idExpert).ToList();

            bool isExpertDOC = listDOCs.Any(x => x.IdStatus == kvActive.IdKeyValue);
            expert.IsDOCExpert = isExpertDOC;

            this.repository.Update(expert);
            await this.repository.SaveChangesAsync();
        }
        public async Task ChangeExpertNAPOOAsync(int idExpert)
        {
            var kvActive = await dataSourceService.GetKeyValueByIntCodeAsync("ExpertStatus", "ActiveExpert");
            var context = new ResultContext<ExpertNapooVM>();
            var expert = await this.repository.GetByIdAsync<Expert>(idExpert);
            this.repository.Detach<Expert>(expert);
            var listExpertsNAPOO = this.repository.All<ExpertNapoo>(e => e.IdExpert == idExpert).ToList();

            bool isExpertNAPOO = listExpertsNAPOO.Any(x => x.IdStatus == kvActive.IdKeyValue);
            expert.IsNapooExpert = isExpertNAPOO;

            this.repository.Update(expert);
            await this.repository.SaveChangesAsync();
        }
        public async Task<IEnumerable<ExpertDOCVM>> GetAllExpertExpertDOCsAsync(int expertId)
        {
            var expertDOCs = this.repository.All<ExpertDOC>(e => e.IdExpert == expertId);
            var dataVM = await expertDOCs.To<ExpertDOCVM>().ToListAsync();

            foreach (var expertDOC in dataVM)
            {
                expertDOC.Expert = await GetExpertByIdAsync(expertId);
                expertDOC.DOC = await docService.GetDOCByIdAsync(new DocVM() { IdDOC = expertDOC.IdDOC });
                expertDOC.StatusName = dataSourceService.GetKeyValueByIdAsync(expertDOC.IdStatus).Result.Name;
            }

            return dataVM;
        }
        public async Task<IEnumerable<ExpertNapooVM>> GetAllExpertsNAPOOAsync(int expertId)
        {
            var expertsNAPOO = this.repository.All<ExpertNapoo>(e => e.IdExpert == expertId);
            var dataVM = await expertsNAPOO.To<ExpertNapooVM>().ToListAsync();

            foreach (var expertNAPOO in dataVM)
            {
                expertNAPOO.Expert = await GetExpertByIdAsync(expertId);
                expertNAPOO.StatusName = dataSourceService.GetKeyValueByIdAsync(expertNAPOO.IdStatus).Result.Name;
            }

            return dataVM;
        }

        public async Task<IEnumerable<ExpertVM>> GetAllNAPOOExpertsWithPersonIncludedAsync()
        {
            var experts = await this.repository.AllReadonly<Expert>(x => x.IsNapooExpert).To<ExpertVM>(x => x.Person).ToListAsync();
            return experts.OrderBy(x => x.Person.FullName).ToList();
        }

        public async Task<ExpertDOCVM> GetExpertDOCByIdAsync(int id)
        {
            var expertDOCs = await this.repository.AllReadonly<ExpertDOC>(e => e.IdExpertDOC == id)
                .Include(x => x.Expert)
                .Include(x => x.DOC)
                .FirstOrDefaultAsync();

            this.repository.Detach<ExpertDOC>(expertDOCs);

            ExpertDOCVM result = expertDOCs.To<ExpertDOCVM>();

            return result;
        }
        public async Task<ExpertNapooVM> GetExpertNAPOOByIdAsync(int id)
        {
            var expertsNAPOO = await this.repository.AllReadonly<ExpertNapoo>(e => e.IdExpertNapoo == id)
                .Include(x => x.Expert)
                .FirstOrDefaultAsync();

            this.repository.Detach<ExpertNapoo>(expertsNAPOO);

            ExpertNapooVM result = expertsNAPOO.To<ExpertNapooVM>();

            return result;
        }
        public async Task<int> UpdateExpertDOCAsync(ExpertDOCVM model)
        {
            int result = GlobalConstants.INVALID_ID_ZERO;
            var context = new ResultContext<ExpertDOCVM>();

            if (model.IdExpertDOC == 0)
            {
                result = await this.CreateExpertDOCAsync(model);
            }
            else
            {
                var updatedEnity = await this.GetByIdAsync<ExpertDOC>(model.IdExpertDOC);
                model.DOC = null;
                model.Expert = null;

                updatedEnity = model.To<ExpertDOC>();

                this.repository.Update<ExpertDOC>(updatedEnity);
                result = await this.repository.SaveChangesAsync();
            }

            if (result > 0)
            {
                await this.ChangeExpertDOCAsync(model.IdExpert);
            }

            return result;
        }
        public async Task<ResultContext<ExpertNapooVM>> UpdateExpertNAPOOAsync(ExpertNapooVM model)
        {
            int result = GlobalConstants.INVALID_ID_ZERO;
            var context = new ResultContext<ExpertNapooVM>();
            if (model.IdExpertNapoo == 0)
            {
                var newExpertNAPOO = model.To<ExpertNapoo>();

                await this.repository.AddAsync<ExpertNapoo>(newExpertNAPOO);
                result = await this.repository.SaveChangesAsync();
                model = newExpertNAPOO.To<ExpertNapooVM>();
            }
            else
            {
                var updatedEnity = await this.GetByIdAsync<ExpertNapoo>(model.IdExpertNapoo);

                updatedEnity = model.To<ExpertNapoo>();

                this.repository.Update<ExpertNapoo>(updatedEnity);
                result = await this.repository.SaveChangesAsync();
            }

            if (result > 0)
            {
                await this.ChangeExpertNAPOOAsync(model.IdExpert);
                context.ResultContextObject = model;
                context.AddMessage("Записът е успешен!");
            }

            return context;
        }
        public async Task<int> CreateExpertDOCAsync(ExpertDOCVM model)
        {
            var newExpertDOC = model.To<ExpertDOC>();

            await this.repository.AddAsync<ExpertDOC>(newExpertDOC);
            var result = await this.repository.SaveChangesAsync();

            return result;
        }
    }
}
