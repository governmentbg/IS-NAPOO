using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface IPersonService : IBaseService
    {
        Task<int> CreatePerson(PersonVM model);

        Task<IEnumerable<PersonVM>> GetAllPersonsAsync(PersonVM model);

        Task<IEnumerable<PersonVM>> GetAllPersonsAsync();

        Task<List<PersonVM>> GetPersonsByIdsAsync(List<int> ids);

        Task<PersonVM> GetPersonWithouAnythingIncludedByIdAsync(int id);

        Task<PersonVM> GetPersonByIdAsync(int id);

        Task<PersonVM> GetPersonByIdWithIncludeAsync(int idPerson);


        Task<ResultContext<PersonVM>> UpdatePersonAsync(ResultContext<PersonVM> resultContext);

        Task<ResultContext<PersonVM>> CreatePersonAndExpert(ResultContext<PersonVM> resultContext);

        Task<int> DeletePersonAsync(List<PersonVM> models);

        ExpertVM SetExpertTypeFieldsToModel(string expertType);
        Task<bool> CheckPersonIdentIsUniqueForCandidateProvider(string ident, int idCurrentPerson, int idCandidate_Provider);

        Task<List<ApplicationUser>> GetPersonByIdCandidateProvider(int idCandidateProvider, int idKvActiveUser);
    }
}
