using Data.Models.Common;
using Data.Models.Data.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.EKATTE;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.EKATTE
{
    public class MunicipalityService : BaseService, IMunicipalityService
    {
        private readonly IRepository repository;

        public MunicipalityService(IRepository repository)
            : base (repository)
        {
            this.repository = repository;
        }

        public async Task<int> CreateMunicipalityAsync(MunicipalityVM municipalityVM)
        {
            Municipality municipality = municipalityVM.To<Municipality>();
            await this.repository.AddAsync<Municipality>(municipality);

            return await this.repository.SaveChangesAsync();
        }

        public async Task<int> DeleteMunicipalityAsync(int id)
        {
            Municipality municipality = await this.repository.GetByIdAsync<Municipality>(id);

            if (municipality != null)
            {
                await this.repository.HardDeleteAsync<Municipality>(id);
                return await this.repository.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<IEnumerable<MunicipalityVM>> GetAllMunicipalitiesAsync()
        {
            IQueryable<Municipality> municipalities = this.repository.All<Municipality>();

            return await municipalities.To<MunicipalityVM>().ToListAsync();
        }

        public async Task<IEnumerable<MunicipalityVM>> GetAllMunicipalitiesByDistrictIdAsync(int? id)
        {
            IQueryable<Municipality> municipalities = this.repository.All<Municipality>(x => x.idDistrict == id);

            return await municipalities.To<MunicipalityVM>().ToListAsync();
        }

        public async Task<MunicipalityVM> GetMunicipalityByIdAsync(int id)
        {
            Municipality municipality = await this.repository.GetByIdAsync<Municipality>(id);

            if (municipality != null)
            {
                this.repository.Detach<Municipality>(municipality);

                return municipality.To<MunicipalityVM>();
            }

            return null;
        }

        public async Task<int> UpdateMunicipalityAsync(MunicipalityVM municipalityVM)
        {
            Municipality municipality = municipalityVM.To<Municipality>();
            this.repository.Update<Municipality>(municipality);

            return await this.repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<int>> GetAllMunicipalitiesWithRegionsAsync()
        {
            var data = await this.repository.AllReadonly<Municipality>(x => x.MunicipalityName == "Варна" || x.MunicipalityName == "Пловдив" || x.MunicipalityName == "Столична").ToListAsync();

            return data.Select(x => x.idMunicipality).ToList();
        }
    }
}
