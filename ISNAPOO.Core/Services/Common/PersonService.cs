using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.ExternalExpertCommission;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Common
{
    public class PersonService : BaseService, IPersonService
    {
        private readonly IRepository repository;

        public PersonService(IRepository repository) : base(repository)
        {
            this.repository = repository;
        }

        public async Task<PersonVM> GetPersonByIdAsync(int id)
        {
            Person person = await this.repository.GetByIdAsync<Person>(id);
            this.repository.Detach<Person>(person);

            PersonVM result = person.To<PersonVM>();

            if (person.IdLocation.HasValue)
            {
                Location location = await this.repository.GetByIdAsync<Location>(person.IdLocation.Value);
                this.repository.Detach<Location>(location);

                result.Location = location.To<LocationVM>();
            }

            return result;
        }

        public async Task<PersonVM> GetPersonWithouAnythingIncludedByIdAsync(int id)
        {
            Person person = await this.repository.GetByIdAsync<Person>(id);

            PersonVM result = person.To<PersonVM>();

            return result;
        }

        public async Task<PersonVM> GetPersonByIdWithIncludeAsync(int idPerson)
        {
            IQueryable<Person> data = this.repository.AllReadonly<Person>(x=>x.IdPerson == idPerson);

            return data.To<PersonVM>(x=>x.CandidateProviderPersons).First();

        }

        public async Task<List<PersonVM>> GetPersonsByIdsAsync(List<int> ids)
        {
            IQueryable<Person> experts = this.repository.All<Person>(this.FilterPersonByIds(ids));

            return experts.To<PersonVM>().ToList();
        }

        public async Task<int> CreatePerson(PersonVM model)
        {
            var newPerson = model.To<Person>();

     

            await this.repository.AddAsync<Person>(newPerson);
            var result = await this.repository.SaveChangesAsync();

            if (result == 1)
            {
                result = newPerson.IdPerson;
            }

            return result;
        }

        public async Task<bool> CheckPersonIdentIsUniqueForCandidateProvider(string ident, int idCurrentPerson, int idCandidate_Provider)
        {
            CandidateProviderPerson candProvPerson = new CandidateProviderPerson();
            bool isUnique = true;

            if (!string.IsNullOrEmpty(ident) && idCandidate_Provider != GlobalConstants.INVALID_ID_ZERO)
            {
                var candidateProvPerson = this.repository.AllReadonly<CandidateProviderPerson>
                    (x => x.IdCandidate_Provider == idCandidate_Provider).Include(x => x.Person);

                if (idCurrentPerson > 0)
                {
                    candProvPerson = candidateProvPerson.FirstOrDefault(x => x.Person.Indent == ident && x.Person.IdPerson != idCurrentPerson);
                }
                else
                {
                    candProvPerson = candidateProvPerson.FirstOrDefault(x => x.Person.Indent == ident);
                }
                if (candProvPerson is not null)
                {
                    isUnique = false;
                    return isUnique;
                }

                //var persons = await GetAllPersonsAsync(new PersonVM());

                //if (idCandidate_Provider != GlobalConstants.INVALID_ID_ZERO)
                //{
                //    //Ако имаме кандидат провайдър проверяваме само хората който са в съощия кандидат провайдъра
                //    var personsToChek = persons.Where(p => p.CandidateProviderPersons.Where(c => c.IdCandidate_Provider == idCandidate_Provider).Count() > 0 && p.IdPerson != idCurrentPerson).ToList();

                //    if (personsToChek.Any(x => x.Indent == ident))
                //    {
                //        isUnique = false;
                //        return isUnique;
                //    }
                //}

                ////Опитваме да намерим този човек за да го проверим дали към кандидат провайдър
                //var currentPerson = persons.FirstOrDefault(x => x.IdPerson == idCurrentPerson);
                //if (currentPerson != null)
                //{
                //    var candidateProviderPerson = currentPerson.CandidateProviderPersons.FirstOrDefault();

                //    //Ако имаме кандидат провайдър проверяваме само хората който са в съощия кандидат провайдъра
                //    if (candidateProviderPerson != null)
                //    {
                //        var personsToChek = persons.Where(p => p.CandidateProviderPersons.Where(c => c.IdCandidate_Provider == candidateProviderPerson.IdCandidate_Provider).Count() > 0 && p.IdPerson != idCurrentPerson).ToList();

                //        if (personsToChek.Any(x => x.Indent == ident))
                //        {
                //            isUnique = false;
                //            return isUnique;
                //        }
                //    }
                //    else
                //    {
                //        //Ако няма кандидат провайдър проверя само хората който нямат кандидат провайдър
                //        var personsToChek = persons.Where(p => (p.CandidateProviderPersons == null || p.CandidateProviderPersons.Count() == 0) && p.IdPerson != idCurrentPerson).ToList();

                //        if (personsToChek.Any(x => x.Indent == ident))
                //        {
                //            isUnique = false;
                //            return isUnique;
                //        }
                //    }
                //}
                //else
                //{
                //    var personsToChek = persons.Where(p => (p.CandidateProviderPersons == null || p.CandidateProviderPersons.Count() == 0) && p.IdPerson != idCurrentPerson).ToList();

                //    if (personsToChek.Any(x => x.Indent == ident))
                //    {
                //        isUnique = false;
                //        return isUnique;
                //    }
                //}
            }
            return isUnique;
        }

        public async Task<IEnumerable<PersonVM>> GetAllPersonsAsync()
        {
            var data = this.repository.All<Person>();

            return await data.To<PersonVM>().ToListAsync();
        }

        public async Task<IEnumerable<PersonVM>> GetAllPersonsAsync(PersonVM model)
        {
            IQueryable<Person> persons = this.repository.All<Person>(this.FilterPersonValue(model));

            return await persons.To<PersonVM>(x => x.CandidateProviderPersons).ToListAsync();
        }

        protected Expression<Func<Person, bool>> FilterPersonValue(PersonVM model)
        {
            var predicate = PredicateBuilder.True<Person>();

            if (!string.IsNullOrEmpty(model.FirstName))
            {
                predicate = predicate.And(p => p.FirstName.Contains(model.FirstName));
            }
            if (!string.IsNullOrEmpty(model.SecondName))
            {
                predicate = predicate.And(p => p.SecondName.Contains(model.SecondName));
            }
            if (!string.IsNullOrEmpty(model.FamilyName))
            {
                predicate = predicate.And(p => p.FamilyName.Contains(model.FamilyName));
            }
            if (model.IdIndentType.HasValue)
            {
                predicate = predicate.And(p => p.IdIndentType.HasValue && p.IdIndentType.Value == model.IdIndentType.Value);
            }
            if (!string.IsNullOrEmpty(model.Indent))
            {
                predicate = predicate.And(p => p.Indent != null && p.Indent.Contains(model.Indent));
            }
            if (model.BirthDate.HasValue)
            {
                predicate = predicate.And(p => p.BirthDate.HasValue && p.BirthDate.Value == model.BirthDate.Value);
            }
            if (model.IdSex.HasValue)
            {
                predicate = predicate.And(p => p.IdSex.HasValue && p.IdSex.Value == model.IdSex.Value);
            }
            if (!string.IsNullOrEmpty(model.PersonalID))
            {
                predicate = predicate.And(p => p.PersonalID != null && p.PersonalID.Contains(model.PersonalID));
            }
            if (!string.IsNullOrEmpty(model.PersonalIDIssueBy))
            {
                predicate = predicate.And(p => p.PersonalIDIssueBy != null && p.PersonalIDIssueBy.Contains(model.PersonalIDIssueBy));
            }
            if (model.IdLocation.HasValue)
            {
                predicate = predicate.And(p => p.IdLocation.HasValue && p.IdLocation.Value == model.IdLocation.Value);
            }
            if (!string.IsNullOrEmpty(model.PostCode))
            {
                predicate = predicate.And(p => p.PostCode != null && p.PostCode.Contains(model.PostCode));
            }
            if (!string.IsNullOrEmpty(model.Address))
            {
                predicate = predicate.And(p => p.Address != null && p.Address.Contains(model.Address));
            }
            if (!string.IsNullOrEmpty(model.Phone))
            {
                predicate = predicate.And(p => p.Phone != null && p.Phone.Contains(model.Phone));
            }
            if (!string.IsNullOrEmpty(model.Email))
            {
                predicate = predicate.And(p => p.Email != null && p.Email.Contains(model.Email));
            }

            return predicate;
        }

        protected Expression<Func<Person, bool>> FilterPersonByIds(List<int> ids)
        {
            var predicate = PredicateBuilder.True<Person>();

            predicate = predicate.And(n => ids.Contains(n.IdPerson));

            return predicate;
        }

        public ExpertVM SetExpertTypeFieldsToModel(string expertType)
        {
            var vm = new ExpertVM();
            if (expertType == GlobalConstants.TOKEN_EXPERT_EXTERNAL_VALUE)
            {
                vm.IsExternalExpert = true;
            }
            else if (expertType == GlobalConstants.TOKEN_EXPERT_COMMISSIONS_VALUE)
            {
                vm.IsCommissionExpert = true;
            }
            else if (expertType == GlobalConstants.TOKEN_EXPERT_NAPOO_EMPLOYEES_VALUE)
            {
                vm.IsNapooExpert = true;
            }
            else if (expertType == GlobalConstants.TOKEN_EXPERT_DOC_WORK_GROUP_VALUE)
            {
                vm.IsDOCExpert = true;
            }

            return vm;
        }

        public async Task<ResultContext<PersonVM>> CreatePersonAndExpert(ResultContext<PersonVM> resultContext)
        {
            try
            {

                var newPerson = resultContext.ResultContextObject.To<Person>();

      

                Expert newExpert = new Expert();

                newExpert.Person = newPerson;
       
                var viewModel = SetExpertTypeFieldsToModel(resultContext.ResultContextObject.TokenExpertType);
                newExpert.IsExternalExpert = viewModel.IsExternalExpert;
                newExpert.IsCommissionExpert = viewModel.IsCommissionExpert;
                newExpert.IsNapooExpert = viewModel.IsNapooExpert;
                newExpert.IsDOCExpert = viewModel.IsDOCExpert;

                await this.repository.AddAsync<Expert>(newExpert);
                int result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
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

        public async Task<ResultContext<PersonVM>> UpdatePersonAsync(ResultContext<PersonVM> resultContext)
        {
            try
            {
                var updatedEnity = await this.GetByIdAsync<Person>(resultContext.ResultContextObject.IdPerson);
                this.repository.Detach<Person>(updatedEnity);

                updatedEnity = resultContext.ResultContextObject.To<Person>();
                 
                updatedEnity.Location = null;
                this.repository.Update(updatedEnity);
                var result = await this.repository.SaveChangesAsync();

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

        public async Task<int> DeletePersonAsync(List<PersonVM> persons)
        {
            foreach (var person in persons)
            {
                await this.repository.HardDeleteAsync<Person>(person.IdPerson);
            }

            return await this.repository.SaveChangesAsync();
        }

        public async Task<List<ApplicationUser>> GetPersonByIdCandidateProvider(int idCandidateProvider, int idKvActiveUser)
        {
            List<CandidateProviderPerson> candProvPerso = new List<CandidateProviderPerson>();
            List<ApplicationUser> appActiveUsers = new List<ApplicationUser>();
            ApplicationUser appUser = new ApplicationUser();

            if (idCandidateProvider != GlobalConstants.INVALID_ID_ZERO)
            {
                var candidateProvPerson = this.repository.AllReadonly<CandidateProviderPerson>
                    (x => x.IdCandidate_Provider == idCandidateProvider);
                if (candidateProvPerson.Any())
                {
                    foreach(var person in candidateProvPerson)
                    {
                        appUser = this.repository.AllReadonly<ApplicationUser>
                            (x => x.IdPerson == person.IdPerson && x.IdUserStatus == idKvActiveUser).FirstOrDefault();
                        if (appUser != null)
                        {
                            appActiveUsers.Add(appUser);
                        }
                    }
                }
            }
            return appActiveUsers;
        }
    }
}
