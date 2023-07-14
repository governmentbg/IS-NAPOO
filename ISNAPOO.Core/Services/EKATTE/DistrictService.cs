using Data.Models.Common;
using Data.Models.Data.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.EKATTE;
using Microsoft.EntityFrameworkCore;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.EKATTE
{
    public class DistrictService : BaseService, IDistrictService
    {
        private readonly IRepository repository;

        public DistrictService(IRepository repository)
            : base(repository)
        {
            this.repository = repository;
        }

        public async Task<int> CreateDistrictAsync(DistrictVM districtVM)
        {
            District district = districtVM.To<District>();
            await this.repository.AddAsync<District>(district);

            return await this.repository.SaveChangesAsync();
        }

        public async Task<int> DeleteDistrictAsync(int id)
        {
            District district = await this.repository.GetByIdAsync<District>(id);

            if (district != null)
            {
                await this.repository.HardDeleteAsync<District>(id);
                return await this.repository.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<IEnumerable<DistrictVM>> GetAllDistrictsAsync()
        {
            IQueryable<District> districts = this.repository.All<District>().OrderBy(d=>d.DistrictName);

            IEnumerable<DistrictVM> result = await districts.To<DistrictVM>(x => x.Municipalities.Select(y => y.Locations), x => x.Municipalities.Select(y => y.Regions)).ToListAsync();

            return result;
        }

        public async Task<DistrictVM> GetDistrictByIdAsync(int id)
        {
            District district = await this.repository.GetByIdAsync<District>(id);
            this.repository.Detach<District>(district);

            if (district != null)
            {
                DistrictVM districtVM = district.To<DistrictVM>();

                return districtVM;
            }

            return null;
        }

        public async Task<int> UpdateDistrictAsync(DistrictVM districtVM)
        {
            District district = districtVM.To<District>();

            this.repository.Update<District>(district);
            return await this.repository.SaveChangesAsync();
        }
    }
}
