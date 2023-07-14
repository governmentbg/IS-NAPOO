using Data.Models.Common;
using Data.Models.Data.Common;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Common
{
    public class PolicyService : BaseService, IPolicyService
    {
        private readonly IRepository repository;

        public PolicyService(IRepository repository) : base(repository)
        {
            this.repository = repository;
        }

        public async Task MergePoliciesAsync()
        {
            IEnumerable<Policy> policiesDB = await this.repository.All<Policy>().ToListAsync();

            var policiesFormCode = IPolicyService.GetPolicies();
            bool hasNewPolicies = false;

            foreach (var policy in policiesFormCode) 
            {

                if (!policiesDB.Any(c => c.PolicyCode == policy.PolicyCode)) 
                {
                    Policy newPolicy = new Policy() { PolicyCode = policy.PolicyCode, PolicyDescription = policy.PolicyDescription };
                    this.repository.AddAsync<Policy>(newPolicy);
                    hasNewPolicies = true;
                }                
            }

            if (hasNewPolicies) { await this.repository.SaveChangesAsync(); }
            
        }

        public async Task<IEnumerable<PolicyVM>> GetAllPolicyAsync(PolicyVM policyVM)
        {
            var data = this.repository.All<Policy>(FilterPolicy(policyVM));
            var dataVM = await data.To<PolicyVM>().ToListAsync();
            return dataVM.OrderBy(p => p.PolicyDescription).ToList();
        }

        public async Task<IEnumerable<PolicyVM>> GetAllPolicyExceptAsync(List<RoleClaim> RoleClaims)
        {
            var data = this.repository.All<Policy>().Where(p=> !RoleClaims.Select(r=>r.Type).Contains(p.PolicyCode));

                       
            return await data.To<PolicyVM>().ToListAsync();
        }

        protected Expression<Func<Policy, bool>> FilterPolicy(PolicyVM policyVM )
        {
            var predicate = PredicateBuilder.True<Policy>();
           

            return predicate;
        }

        public async Task UpdatePolicy(PolicyVM policyVM)
        {
            var DBPolicy = await this.repository.GetByIdAsync<Policy>(policyVM.idPolicy);

            DBPolicy.PolicyDescription = policyVM.PolicyDescription;

             repository.Update<Policy>(DBPolicy);
            await repository.SaveChangesAsync();
        }

        public async Task<PolicyVM> getById(int idPolicy)
        {
            Policy policy = await this.repository.GetByIdAsync<Policy>(idPolicy);

            return policy.To<PolicyVM>();
        }
    }

    

}
