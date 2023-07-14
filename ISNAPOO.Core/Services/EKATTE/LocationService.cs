using Data.Models.Common;
using Data.Models.Data.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.Services;
using ISNAPOO.Core.ViewModels.EKATTE;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.EKATTE
{
    public class LocationService : BaseService, ILocationService
    {
        private readonly IRepository repository;

        public LocationService(IRepository repository)
            : base(repository)
        {
            this.repository = repository;
        }

        public async Task<int> CreateLocationAsync(LocationVM locationVM)
        {
            Location location = locationVM.To<Location>();
            await this.repository.AddAsync<Location>(location);

            return await this.repository.SaveChangesAsync();
        }

        public async Task<int> DeleteLocationAsync(int id)
        {
            Location location = await this.repository.GetByIdAsync<Location>(id);

            if (location != null)
            {
                await this.repository.HardDeleteAsync<Location>(id);
                return await this.repository.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<IEnumerable<LocationVM>> GetAllLocationsAsync()
        {
            IQueryable<Location> locations = this.repository.AllReadonly<Location>();

            return await locations.To<LocationVM>().ToListAsync();
        }

        public async Task<LocationVM> GetLocationByLocationIdAsync(int? id)
        {
            IQueryable<Location> locations = this.repository.All<Location>(x => x.idLocation == id);
            var result = locations.FirstOrDefault();
            return result.To<LocationVM>();
        }

        public async Task<IEnumerable<LocationVM>> GetAllLocationsAsync(LocationVM filterModel)
        {
            IQueryable<Location> locations = this.repository.All<Location>(FilterLocationValue(filterModel));

            return await locations.To<LocationVM>().ToListAsync();
        }

        public async Task<IEnumerable<LocationVM>> GetAllLocationsByMunicipalityIdAsync(int? id)
        {
            IQueryable<Location> locations = this.repository.All<Location>(x => x.idMunicipality == id);

            return await locations.To<LocationVM>().ToListAsync();
        }

        public async Task<IEnumerable<LocationVM>> GetAllLocationsJoinedAsync(LocationVM locationVM)
        {   
            var locations = this.repository.AllReadonly<Location>().Include(l => l.Municipality).OrderBy(o => o.kati).AsNoTracking();

            var locationVMs = await locations.To<LocationVM>(l => l.Municipality).ToListAsync();

            return locationVMs;
        }

        // филтрира нас. места по кати от филтър текст
        public async Task<IEnumerable<LocationVM>> GetAllLocationsByKatiAsync(string kati)
        {
            var locations = this.repository.AllReadonly<Location>(x => x.kati.Contains(kati)).OrderBy(o => o.kati).AsNoTracking();

            var locationVMs = await locations.To<LocationVM>(l => l.Municipality).ToListAsync();

            return locationVMs;
        }

        public async Task<LocationVM> GetLocationByIdAsync(int id)
        {
            Location location = await this.repository.GetByIdAsync<Location>(id);

            if (location != null)
            {
                this.repository.Detach<Location>(location);

                return location.To<LocationVM>();
            }

            return null;
        }

        public async Task<LocationVM> GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(int id)
        {
            var location = await this.repository.AllReadonly<Location>(x => x.idLocation == id).Include(x => x.Municipality).ThenInclude(x => x.District).AsNoTracking().FirstOrDefaultAsync();

            return location.To<LocationVM>();
        }

        protected Expression<Func<Location, bool>> FilterLocationValue(LocationVM model)
        {
            var predicate = PredicateBuilder.True<Location>();

            if (!string.IsNullOrEmpty(model.LocationJoinedFilter))
            {
                predicate = predicate.And(p => p.LocationName.Contains(model.LocationJoinedFilter)
                                               || (p.Municipality != null && p.Municipality.MunicipalityName.Contains(model.LocationJoinedFilter))
                                               || (p.Municipality != null && p.Municipality.District != null && p.Municipality.District.DistrictName.Contains(model.LocationJoinedFilter))
                                          );
            }
            else if (model.idLocation != 0)
            {
                //Task<LocationVM> taskLoc = Task.Run(async () => await GetLocationByIdAsync(model.idLocation));
                //LocationVM locVM = taskLoc.Result;

                predicate = predicate.And(p => p.idLocation == model.idLocation
                                               || p.idMunicipality == model.idMunicipality
                                          );
            }
            else if (!string.IsNullOrEmpty(model.LocationCode))
            {
                predicate = predicate.And(p => p.LocationCode == model.LocationCode);
            }

            return predicate;
        }        

        public async Task<int> UpdateLocationAsync(LocationVM locationVM)
        {
            Location location = locationVM.To<Location>();
            this.repository.Update<Location>(location);

            return await this.repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<LocationVM>> GetAllLocationsByDistrictIdAsync(int idDistrict)
        {
            return await this.repository
                .All<Location>()
                .To<LocationVM>(x => x.Municipality.District)
                .Where(x => x.Municipality.District.idDistrict == idDistrict)
                .ToListAsync();
        }
    }
}
