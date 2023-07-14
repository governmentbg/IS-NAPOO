using Data.Models;
using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.SPPOO
{
    public class ProfessionalDirectionService : BaseService, IProfessionalDirectionService
    {
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;

        public ProfessionalDirectionService(IRepository repository, IDataSourceService dataSourceService)
            : base(repository)
        {
            this.repository = repository;
            this.dataSourceService = dataSourceService;
        }

        public async Task<string> CreateProfessionalDirectionAsync(ProfessionalDirectionVM proffesionalDirectionVM)
        {
            bool checkCodeResult = this.DoesProfessionalDirectionWithCodeExist(proffesionalDirectionVM.Code);

            if (checkCodeResult)
            {
                return "Професионално направление с този код вече съществува!";
            }

            string msg = string.Empty;

           

            if (proffesionalDirectionVM.IdStatus == 0)
            {
                proffesionalDirectionVM.IdStatus = dataSourceService.GetWorkStatusID();
            }

            try
            {
                ProfessionalDirection professionalDirection = proffesionalDirectionVM.To<ProfessionalDirection>();
                await this.repository.AddAsync<ProfessionalDirection>(professionalDirection);
                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    msg = "Записът е успешен!";
                    proffesionalDirectionVM.IdProfessionalDirection = professionalDirection.IdProfessionalDirection;
                }
                else
                {
                    msg = "Грешка при запис в базата данни!";
                }

                foreach (var order in proffesionalDirectionVM.OrderVMs)
                {
                    ProfessionalDirectionOrder professionalDirectionOrder = new ProfessionalDirectionOrder
                    {
                        IdSPPOOOrder = order.IdOrder,
                        IdProfessionalDirection = professionalDirection.IdProfessionalDirection,
                        IdTypeChange = order.IdTypeChange
                    };

                    await this.repository.AddAsync<ProfessionalDirectionOrder>(professionalDirectionOrder);

                    await this.repository.SaveChangesAsync();

                    this.repository.Detach<ProfessionalDirectionOrder>(professionalDirectionOrder);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        public async Task<string> DeleteProfessionalDirectionAsync(int id)
        {
            string msg = string.Empty;

            try
            {
                await this.repository.HardDeleteAsync<ProfessionalDirection>(id);

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

        public async Task<ProfessionalDirectionVM> GetProfessionalDirectionByIdAsync(ProfessionalDirectionVM professionalDirectionVM)
        {
            ProfessionalDirection professionalDirection = await this.repository.AllReadonly<ProfessionalDirection>(FilterValue(professionalDirectionVM))
                .Include(o => o.ProfessionalDirectionOrders)
                    .ThenInclude(or => or.SPPOOOrder)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

            this.repository.Detach<ProfessionalDirection>(professionalDirection);

            return professionalDirection.To<ProfessionalDirectionVM>();
        }

        public async Task<IEnumerable<ProfessionalDirectionVM>> GetProfessionalDirectionsByIdsAsync(List<int> ids)
        {
            IQueryable<ProfessionalDirection> professionalDirections = this.repository.AllReadonly<ProfessionalDirection>(x => ids.Contains(x.IdProfessionalDirection));

            return await professionalDirections.To<ProfessionalDirectionVM>().ToListAsync();
        }

        public async Task<string> UpdateProfessionalDirectionAsync(ProfessionalDirectionVM proffesionalDirectionVM)
        {
            ProfessionalDirection professionalDirectionFromDB = await this.repository.GetByIdAsync<ProfessionalDirection>(proffesionalDirectionVM.IdProfessionalDirection);
            this.repository.Detach<ProfessionalDirection>(professionalDirectionFromDB);

            if (proffesionalDirectionVM.Code != professionalDirectionFromDB.Code)
            {
                bool checkCodeResult = this.DoesProfessionalDirectionWithCodeExist(proffesionalDirectionVM.Code);

                if (checkCodeResult)
                {
                    return "Професионално направление с този код вече съществува!";
                }
            }

            string msg = string.Empty;

            if (proffesionalDirectionVM.IdStatus == 0)
            {
                proffesionalDirectionVM.IdStatus = dataSourceService.GetWorkStatusID();
            }

            try
            {
                professionalDirectionFromDB = proffesionalDirectionVM.To<ProfessionalDirection>();
         

                this.repository.Update(professionalDirectionFromDB);
                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    msg = "Записът е успешен!";
                }
                else
                {
                    msg = "Грешка при запис в базата данни!";
                }

                this.repository.Detach<ProfessionalDirection>(professionalDirectionFromDB);

                List<ProfessionalDirectionOrder> professionalDirectionOrders = this.repository.AllReadonly<ProfessionalDirectionOrder>(x => x.IdProfessionalDirection == proffesionalDirectionVM.IdProfessionalDirection).ToList();

                this.repository.HardDeleteRange<ProfessionalDirectionOrder>(professionalDirectionOrders);
                await this.repository.SaveChangesAsync();

                foreach (var order in proffesionalDirectionVM.OrderVMs)
                {
                    ProfessionalDirectionOrder professionalDirectionOrder = new ProfessionalDirectionOrder
                    {
                        IdSPPOOOrder = order.IdOrder,
                        IdProfessionalDirection = professionalDirectionFromDB.IdProfessionalDirection,
                        IdTypeChange = order.IdTypeChange
                    };

                    await this.repository.AddAsync<ProfessionalDirectionOrder>(professionalDirectionOrder);
                    await this.repository.SaveChangesAsync();

                    if (professionalDirectionOrder.SPPOOOrder != null)
                    {
                        this.repository.Detach<SPPOOOrder>(professionalDirectionOrder.SPPOOOrder);
                    }

                    this.repository.Detach<ProfessionalDirectionOrder>(professionalDirectionOrder);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        private bool DoesProfessionalDirectionWithCodeExist(string code)
        {
            return this.repository.AllReadonly<ProfessionalDirection>(x => x.Code == code && x.IdStatus == dataSourceService.GetActiveStatusID()).Any();
        }

        protected Expression<Func<ProfessionalDirection, bool>> FilterValue(ProfessionalDirectionVM model)
        {
            var predicate = PredicateBuilder.True<ProfessionalDirection>();

            if (model.IdProfessionalDirection > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdProfessionalDirection == model.IdProfessionalDirection);
            }

            return predicate;
        }

        public async Task<IEnumerable<ProfessionalDirectionVM>> GetAllProfessionalDirectionsByCandidateProviderIdAsync(int idCandidateProvider)
        {
            var data = this.repository.All<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == idCandidateProvider);

            var candidateProviderSpecialityVMs = await data.To<CandidateProviderSpecialityVM>(c => c.Speciality.Profession.ProfessionalDirection).ToListAsync();

            var listDirections = new List<ProfessionalDirectionVM>();
            var validationList = new List<int>();
            foreach (var item in candidateProviderSpecialityVMs)
            {
                if (!validationList.Contains(item.Speciality.Profession.IdProfessionalDirection))
                {
                    validationList.Add(item.Speciality.Profession.IdProfessionalDirection);
                    listDirections.Add(item.Speciality.Profession.ProfessionalDirection);
                }
            }

            return listDirections;
        }

        public async Task<IEnumerable<ProfessionalDirectionVM>> GetAllProfessionalDirectionsAsync(ProfessionalDirectionVM modelFilter)
        {
            var data = this.repository.All<ProfessionalDirection>(this.FilterProfessionalDirectionValue(modelFilter));

            return await data.To<ProfessionalDirectionVM>().ToListAsync();
        }
        public async Task<IEnumerable<ProfessionalDirectionVM>> GetCIPOProfessionalDirectionsAsync(ProfessionalDirectionVM modelFilter)
        {
            var data = this.repository.All<ProfessionalDirection>(this.FilterProfessionalDirectionValue(modelFilter));
            var dataVM = data.Where(d => d.Name == "Професионално ориентиране" && d.Code == "999").To<ProfessionalDirectionVM>();
            return dataVM;
        }

        public async Task<IEnumerable<ProfessionalDirectionVM>> GetAllActiveProfessionalDirectionsAsync()
        {
            IQueryable<ProfessionalDirection> data = this.repository.AllReadonly<ProfessionalDirection>(x => x.IdStatus == dataSourceService.GetActiveStatusID());

            return await data.To<ProfessionalDirectionVM>().ToListAsync();
        }
    
        protected Expression<Func<ProfessionalDirection, bool>> FilterProfessionalDirectionValue(ProfessionalDirectionVM modelFilter)
        {
            var predicate = PredicateBuilder.True<ProfessionalDirection>();

            if (!string.IsNullOrWhiteSpace(modelFilter.DisplayNameFilter))
            {
                predicate = predicate.And(p => p.Name.Contains(modelFilter.DisplayNameFilter)
                                               || p.Code.Contains(modelFilter.DisplayNameFilter));
            }

            return predicate;
        }
    }
}
