using Data.Models.Common;
using Data.Models.Data.Common;
using Data.Models.Framework;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Common;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services
{
    public class BaseService : IBaseService
    {

        protected ILogger<BaseService> logger;
        protected IRepository repository;
        protected AuthenticationStateProvider authenticationStateProvider;

        public BaseService(IRepository repository)
        {
            this.repository = repository;
            //this.logger = logger;
        }

        public BaseService(IRepository repository, AuthenticationStateProvider authenticationStateProvider)
        {
            this.repository = repository;
            this.authenticationStateProvider = authenticationStateProvider;

        }

        protected AuthenticationState AuthenticationState
        {
            get
            {
                return this.authenticationStateProvider.GetAuthenticationStateAsync().Result;
            }
        }

        protected UserProps UserProps
        {
            get
            {
                UserProps userProps = new UserProps();

                try
                {
                    AuthenticationState AuthenticationState = this.authenticationStateProvider.GetAuthenticationStateAsync().Result;

                    if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_CANDIDATE_PROVIDER)))
                    {
                        userProps.IdCandidateProvider = Int32.Parse(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_CANDIDATE_PROVIDER));
                    }

                    if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_PERSON)))
                    {
                        userProps.IdPerson = Int32.Parse(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_PERSON));

                    }
                    if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(ClaimTypes.NameIdentifier)))
                    {
                        userProps.ID = AuthenticationState.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    }

                    if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_USER)))
                    {
                        userProps.UserId = Int32.Parse(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_USER));
                    }

                    if (!string.IsNullOrEmpty(AuthenticationState.User.Identity.Name))
                    {
                        userProps.UserName = AuthenticationState.User.Identity.Name;
                    }

                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "remoteIpAddress"))
                    {
                        userProps.IPAddress = AuthenticationState.User.Claims.First(x => x.Type == "remoteIpAddress").Value;
                    }
                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "browserInformation"))
                    {
                        userProps.BrowserInformation = AuthenticationState.User.Claims.First(x => x.Type == "browserInformation").Value;
                    }


                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "currentAction"))
                    {
                        userProps.CurrentAction = AuthenticationState.User.Claims.First(x => x.Type == "currentAction").Value;
                    }

                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "currentActionDescription"))
                    {
                        userProps.CurrentActionDescription = AuthenticationState.User.Claims.First(x => x.Type == "currentActionDescription").Value;
                    }

                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "currentUrl"))
                    {
                        userProps.CurrentUrl = AuthenticationState.User.Claims.First(x => x.Type == "currentUrl").Value;
                    }

                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "currentMenu"))
                    {
                        userProps.CurrentMenu = AuthenticationState.User.Claims.First(x => x.Type == "currentMenu").Value;
                    }
                }
                catch { }


                return userProps;
            }
        }

        protected Task<bool> HasClaim(string claimType)
        {
            if (AuthenticationState == null)
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(AuthenticationState.User.Claims.Any(c => c.Type == claimType));
        }

        protected Task<bool> IsInRole(string role)
        {
            if (AuthenticationState == null)
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(AuthenticationState.User.IsInRole(role));
        }

        protected Task<List<string>> GetUserRoles()
        {
            if (AuthenticationState == null)
            {
                return Task.FromResult(new List<string>());
            }

            var roles = ((ClaimsIdentity)AuthenticationState.User.Identity).Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value);

            return Task.FromResult(roles.ToList());
        }

        public async Task<T> GetByIdObjectAsync<T>(object id) where T : class
        {
            return await repository.GetByIdAsync<T>(id);
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : class
        {
            return await repository.GetByIdAsync<T>(id);
        }

        protected Expression<Func<T, bool>> FilterActive<T>(DateTime? dtNow = null) where T : IDatePeriod
        {
            dtNow = dtNow ?? DateTime.Now;
            return x => x.DateStart <= dtNow && (x.DateEnd ?? DateTime.MaxValue) >= dtNow;
        }



        protected DateTime dtNow = DateTime.Now;


        public async Task<long> GetSequenceNextValue(string resource, int? idResource = 0, int? year = 0)
        {
            long sequenceNextVal = 0;


            SequenceVM filter = new SequenceVM();
            filter.Resource = resource;
            filter.IdResource = idResource;
            filter.Year = year;

            var sequence = await this.repository.All<Sequence>(FilterSequenceValue(filter)).FirstOrDefaultAsync();


            if (sequence != null)
            {
                sequence.NextVal++;
                sequenceNextVal = sequence.NextVal;
                var result = await this.repository.SaveChangesAsync();
            }
            else
            {

                Sequence sequenceToSave = new Sequence();
                sequenceNextVal = 1;
                sequenceToSave.Resource = resource;
                sequenceToSave.IdResource = idResource;
                sequenceToSave.Year = year;
                sequenceToSave.NextVal = sequenceNextVal;

                await this.repository.AddAsync<Sequence>(sequenceToSave);
                var result = await this.repository.SaveChangesAsync();
            }

            return sequenceNextVal;
        }

        protected Expression<Func<Sequence, bool>> FilterSequenceValue(SequenceVM model)
        {
            var predicate = PredicateBuilder.True<Sequence>();


            if (!string.IsNullOrWhiteSpace(model.Resource))
            {
                predicate = predicate.And(p => p.Resource.Contains(model.Resource));
            }

            if (model.IdResource != 0)
            {
                predicate = predicate.And(p => p.IdResource == model.IdResource);
            }

            if (model.Year != 0)
            {
                predicate = predicate.And(p => p.Year == model.Year);
            }

            return predicate;
        }
    }


}
