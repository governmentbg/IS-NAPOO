using Data.Models.Common;
using Data.Models.Data.SPPOO;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.SPPOO
{
    public class SpecialityNKPDService : BaseService, ISpecialityNKPDService
    {
        private readonly IRepository repository;

        public SpecialityNKPDService(IRepository repository)
            : base(repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<SpecialityNKPDVM>> GetAllSpecialityNKPDAsync()
        {
            IQueryable<SpecialityNKPD> data = this.repository.AllReadonly<SpecialityNKPD>();

            return await data.To<SpecialityNKPDVM>(x => x.NKPD).ToListAsync();

        }
    }
}
