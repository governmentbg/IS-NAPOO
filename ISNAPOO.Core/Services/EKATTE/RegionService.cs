using Data.Models.Common;
using Data.Models.Data.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.EKATTE;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.EKATTE
{
    public class RegionService : BaseService, IRegionService
    {
        private readonly IRepository repository;

        public RegionService(IRepository repository)
            : base(repository)
        {
            this.repository = repository;
        }

        public async Task<int> CreateRegionAsync(RegionVM regionVM)
        {
            Region region = regionVM.To<Region>();
            await this.repository.AddAsync<Region>(region);

            return await this.repository.SaveChangesAsync();
        }

        public async Task<int> DeleteRegionAsync(int id)
        {
            Region region = await this.repository.GetByIdAsync<Region>(id);

            if (region != null)
            {
                await this.repository.HardDeleteAsync<Region>(id);
                return await this.repository.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<IEnumerable<RegionVM>> GetAllRegionsAsync()
        {
            IQueryable<Region> regions = this.repository.All<Region>();

            return await regions.To<RegionVM>().ToListAsync();
        }

        public async Task<RegionVM> GetRegionByIdAsync(int id)
        {
            Region region = await this.repository.GetByIdAsync<Region>(id);

            if (region != null)
            {
                this.repository.Detach<Region>(region);

                return region.To<RegionVM>();
            }

            return null;
        }

        public async Task<int> UpdateRegionAsync(RegionVM regionVM)
        {
            Region region = regionVM.To<Region>();
            this.repository.Update<Region>(region);

            return await this.repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<RegionVM>> GetAllRegionsByIdMunicipalityAsync(int idMunicipality)
        {
            var data = this.repository.AllReadonly<Region>(x => x.idMunicipality == idMunicipality);

            return await data.To<RegionVM>().OrderBy(x => x.RegionName).ToListAsync();
        }
    }
}
