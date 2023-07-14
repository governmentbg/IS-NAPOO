using Data.Models.Common;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Common
{
    public class ConcurrencyService : BaseService, IConcurrencyService
    {
        private static readonly SemaphoreLocker _locker = new SemaphoreLocker();
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;
        private readonly IPersonService personService;
        private readonly AuthenticationStateProvider authenticationStateProvider;

        public ConcurrencyService(IRepository repository, IDataSourceService dataSourceService, IPersonService personService, AuthenticationStateProvider authenticationStateProvider)
            : base (repository, authenticationStateProvider)
        {
            this.repository = repository;
            this.dataSourceService = dataSourceService;
            this.personService = personService;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        private int IdPerson => this.UserProps.IdPerson;

        private static IDictionary<Guid, ConcurrencyInfo> CurrentlyOpenedModals { get; set; } = new ConcurrentDictionary<Guid, ConcurrencyInfo>();

        public List<ConcurrencyInfo> GetAllCurrentlyOpenedModals()
        {
            return CurrentlyOpenedModals.Values.ToList();
        }

        public async Task AddEntityIdAsCurrentlyOpened(int idEntity, string type)
        {
            var data = await this.SetConcurrencyData(idEntity, type);
            Guid key = Guid.NewGuid();
            CurrentlyOpenedModals.Add(key, data);
        }

        public void RemoveEntityIdAsCurrentlyOpened(int idEntity, string type)
        {
            var entry = CurrentlyOpenedModals.FirstOrDefault(x => x.Value.IdEntity == idEntity && x.Value.EntityType == type && x.Value.IdPerson == this.IdPerson);
            CurrentlyOpenedModals.Remove(entry);
        }

        private async Task<ConcurrencyInfo> SetConcurrencyData(int idEntity, string type)
        {
            var person = await this.personService.GetPersonByIdAsync(this.IdPerson);
            ConcurrencyInfo concurrencyInfo = new ConcurrencyInfo()
            {
                IdPerson = person.IdPerson,
                IdEntity = idEntity,
                EntityType = type,
                PersonFirstName = person.FirstName,
                PersonFamilyName = person.FamilyName,
                TimeOpened = DateTime.Now
            };

            return concurrencyInfo;
        }
    }
}
