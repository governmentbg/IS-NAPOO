using Data.Models.Common;
using Data.Models.Data.DOC;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.DOC;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.DOC
{
    public class ERUSpecialityService : BaseService, IERUSpecialityService
    {
        private readonly IRepository repository;

        public ERUSpecialityService(IRepository repository)
            : base(repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<ERUSpecialityVM>> GetAllERUsBySpecialityIdAsync(ERUSpecialityVM eruSpeciality)
        {
            IQueryable<ERUSpeciality> eRUSpecialities = this.repository.AllReadonly<ERUSpeciality>(x => x.IdSpeciality == eruSpeciality.IdSpeciality);

            return await eRUSpecialities.To<ERUSpecialityVM>().ToListAsync();
        }
    }
}
