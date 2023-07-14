using Data.Models;
using Data.Models.Common;
using Data.Models.Data.SPPOO;
using Data.Models.DB;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.SPPOO
{
    public class ProfessionService : BaseService, IProfessionService
    {
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;
        public ProfessionService(IRepository repository,IDataSourceService dataSourceService)
            : base(repository)
        {
            this.repository = repository;
            this.dataSourceService = dataSourceService;
        }

        public async Task<IEnumerable<ProfessionVM>> GetAllActiveProfessionsByProfessionalDirectionIdAsync(ProfessionalDirectionVM professionalDirectionVM)
        {
            IQueryable<Profession> professions = this.repository.AllReadonly<Profession>(FilterByProfessionalDirectionId(professionalDirectionVM))
                .Include(o => o.ProfessionOrders)
                    .ThenInclude(or => or.SPPOOOrder)
                .AsNoTracking();

            return await professions.To<ProfessionVM>().AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<ProfessionVM>> GetAllActiveProfessionsAsync()
        {
            IQueryable<Profession> professions = this.repository.AllReadonly<Profession>(x => x.IdStatus == dataSourceService.GetActiveStatusID());

            return await professions.To<ProfessionVM>().AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<ProfessionalDirectionVM>> GetAllActiveProfessionalDirectionsAsync()
        {
            IQueryable<ProfessionalDirection> professions = this.repository.AllReadonly<ProfessionalDirection>(x => x.IdStatus == dataSourceService.GetActiveStatusID());

            return await professions.To<ProfessionalDirectionVM>().AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<ProfessionVM>> GetAllAsync(ProfessionVM professionVM)
        {
            var doc = this.repository.All<Data.Models.Data.SPPOO.Profession>(FilterSettingValue(professionVM));

            IEnumerable<ProfessionVM> result = await doc.To<ProfessionVM>().ToListAsync();

            return result;
        }

        public async  Task<IEnumerable<ProfessionVM>> GetAllAsync()
        {
            var doc = this.repository.All<Data.Models.Data.SPPOO.Profession>();

            IEnumerable<ProfessionVM> result = await doc.To<ProfessionVM>().ToListAsync();

            return result;
        }
        public IEnumerable<ProfessionVM> GetAll(ProfessionVM professionVM)
        {
            var doc = this.repository.All<Data.Models.Data.SPPOO.Profession>(FilterSettingValue(professionVM));

            IEnumerable<ProfessionVM> result = doc.To<ProfessionVM>().ToList();

            return result;
        }


        public async Task<IEnumerable<ProfessionalDirectionVM>> GetAllProfessionalDirections()
        {
            var doc = this.repository.All<Data.Models.Data.SPPOO.ProfessionalDirection>();

            IEnumerable<ProfessionalDirectionVM> result = doc.To<ProfessionalDirectionVM>().ToList();

            return result;
        }

        public async Task<string> CreateProfessionAsync(ProfessionVM professionVM)
        {
            bool checkCodeResult = this.DoesProfessionWithCodeExist(professionVM.Code);

            if (checkCodeResult)
            {
                return "Професия с този код вече съществува!";
            }

            string msg = string.Empty;


            try
            {
                Data.Models.Data.SPPOO.Profession profession = professionVM.To<Data.Models.Data.SPPOO.Profession>();
                profession.IdProfessionalDirection = professionVM.IdProfessionalDirection;

                await this.repository.AddAsync<Data.Models.Data.SPPOO.Profession>(profession);
                var result = await this.repository.SaveChangesAsync();

                professionVM.IdProfession = profession.IdProfession;

                if (result > 0)
                {
                    professionVM.IdProfession = profession.IdProfession;
                    msg = "Записът е успешен!";
                }
                else
                {
                    msg = "Грешка при запис в базата данни!";
                }

                foreach (var order in professionVM.OrderVMs)
                {
                    ProfessionOrder professionOrder = new ProfessionOrder
                    {
                        IdSPPOOOrder = order.IdOrder,
                        IdProfession = profession.IdProfession,
                        IdTypeChange = order.IdTypeChange
                    };

                    await this.repository.AddAsync<ProfessionOrder>(professionOrder);
                    await this.repository.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        public async Task<string> DeleteProfessionAsync(int id)
        {
            string msg = string.Empty;

            try
            {
                await this.repository.HardDeleteAsync<Profession>(id);

                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    msg = "Записът е изтрит успешно!";
                }
                else
                {
                    msg = "Грешка при изтриване в базата данни!";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        public async Task<ProfessionVM> GetProfessionByIdAsync(ProfessionVM professionVM)
        {
            Profession profession = await this.repository.AllReadonly<Profession>(FilterValue(professionVM))
                .Include(o => o.ProfessionOrders)
                    .ThenInclude(or => or.SPPOOOrder)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

            this.repository.Detach<Profession>(profession);

            return profession.To<ProfessionVM>();
        }

        public async Task<ProfessionVM> GetOnlyProfessionByIdAsync(int idProfession)
        {
            Profession profession = await this.repository.GetByIdAsync<Profession>(idProfession);

            if (profession is not null)
            {
                return profession.To<ProfessionVM>();
            }
            else
            {
                return new ProfessionVM();
            }
        }

        public async Task UpdateProfessionOrdersAsync(ProfessionVM professionVM)
        {
            Profession professionFromDB = await this.repository.GetByIdAsync<Profession>(professionVM.IdProfession);
            this.repository.Detach<Profession>(professionFromDB);

            if (professionFromDB.IdStatus != professionVM.IdStatus)
            {
                professionFromDB = professionVM.To<Profession>();
                this.repository.Update(professionFromDB);
                await this.repository.SaveChangesAsync();

                this.repository.Detach<Profession>(professionFromDB);
            }

            List<ProfessionOrder> professionOrders = this.repository.AllReadonly<ProfessionOrder>(x => x.IdProfession == professionVM.IdProfession).ToList();
            this.repository.HardDeleteRange<ProfessionOrder>(professionOrders);

            await this.repository.SaveChangesAsync();

            foreach (var order in professionVM.OrderVMs)
            {
                ProfessionOrder professionOrder = new ProfessionOrder
                {
                    IdSPPOOOrder = order.IdOrder,
                    IdProfession = professionVM.IdProfession,
                    IdTypeChange = order.IdTypeChange
                };

                await this.repository.AddAsync<ProfessionOrder>(professionOrder);

                await this.repository.SaveChangesAsync();

                if (professionOrder.SPPOOOrder != null)
                {
                    this.repository.Detach<SPPOOOrder>(professionOrder.SPPOOOrder);
                }

                this.repository.Detach<ProfessionOrder>(professionOrder);
            }
        }

        public async Task<string> UpdateProfessionAsync(ProfessionVM professionVM)
        {
            professionVM.ProfessionOrders = null;
            Profession professionFromDB = await this.repository.GetByIdAsync<Profession>(professionVM.IdProfession);
            this.repository.Detach<Profession>(professionFromDB);

            if (professionVM.Code != professionFromDB.Code)
            {
                bool checkCodeResult = this.DoesProfessionWithCodeExist(professionVM.Code);

                if (checkCodeResult)
                {
                    return "Професия с този код вече съществува!";
                }
            }

            string msg = string.Empty;

            try
            {
                professionFromDB = professionVM.To<Profession>();

                this.repository.Update(professionFromDB);
                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    msg = "Записът е успешен!";
                }
                else
                {
                    msg = "Грешка при запис в базата данни!";
                }

                this.repository.Detach<Profession>(professionFromDB);

                List<ProfessionOrder> professionOrders = this.repository.AllReadonly<ProfessionOrder>(x => x.IdProfession == professionVM.IdProfession).ToList();
                this.repository.HardDeleteRange<ProfessionOrder>(professionOrders);
                await this.repository.SaveChangesAsync();

                foreach (var order in professionVM.OrderVMs)
                {
                    ProfessionOrder professionOrder = new ProfessionOrder
                    {
                        IdSPPOOOrder = order.IdOrder,
                        IdProfession = professionVM.IdProfession,
                        IdTypeChange = order.IdTypeChange
                    };

                    await this.repository.AddAsync<ProfessionOrder>(professionOrder);
                    await this.repository.SaveChangesAsync();

                    if (professionOrder.SPPOOOrder != null)
                    {
                        this.repository.Detach<SPPOOOrder>(professionOrder.SPPOOOrder);
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        public async Task<IEnumerable<ProfessionVM>> GetProfessionsByIdsAsync(List<int> ids)
        {
            IQueryable<Profession> professions = this.repository.AllReadonly<Profession>(x => ids.Contains(x.IdProfession));

            return await professions.To<ProfessionVM>().ToListAsync();
        }

        private bool DoesProfessionWithCodeExist(string code)
        {
            return this.repository.AllReadonly<Profession>(x => x.Code == code && x.IdStatus == dataSourceService.GetActiveStatusID()).Any();
        }

        protected Expression<Func<Data.Models.Data.SPPOO.Profession, bool>> FilterSettingValue(ProfessionVM model)
        {
            var predicate = PredicateBuilder.True<Data.Models.Data.SPPOO.Profession>();

            if (!string.IsNullOrEmpty(model.ProfessionComboFilter))
            {
                predicate = predicate.And(p => p.Name.Contains(model.ProfessionComboFilter) || p.Code.Contains(model.ProfessionComboFilter));
            }

            if (model.IdProfession > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdProfession == model.IdProfession);
            }

            if (model.IdStatus > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdStatus == model.IdStatus);
            }

            return predicate;
        }

        protected Expression<Func<Profession, bool>> FilterValue(ProfessionVM model)
        {
            var predicate = PredicateBuilder.True<Profession>();

            if (model.IdProfession > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdProfession == model.IdProfession);
            }

            return predicate;
        }

        protected Expression<Func<Profession, bool>> FilterByProfessionalDirectionId(ProfessionalDirectionVM model)
        {
            var predicate = PredicateBuilder.True<Profession>();

            predicate = predicate.And(p => p.IdProfessionalDirection == model.IdProfessionalDirection && p.IdStatus != dataSourceService.GetRemoveStatusID());

            return predicate;
        }
    }
}
