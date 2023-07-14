using Data.Models;
using Data.Models.Common;
using Data.Models.Data.DOC;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.SPPOO
{
    public class SpecialityService : BaseService, ISpecialityService
    {
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;

        public SpecialityService(IRepository repository, IDataSourceService dataSourceService)
            : base(repository)
        {
            this.repository = repository;
            this.dataSourceService = dataSourceService;
        }

        public async Task<IEnumerable<SpecialityVM>> GetAllActiveSpecialitiesByProfessionIdAsync(ProfessionVM professionVM)
        {
            IQueryable<Speciality> specialities = this.repository.AllReadonly<Speciality>(FilterByProfessionId(professionVM))
                .Include(o => o.SpecialityOrders)
                    .ThenInclude(or => or.SPPOOOrder)
                .AsNoTracking();

            return await specialities.To<SpecialityVM>().AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<SpecialityVM>> GetAllActiveSpecialitiesAsync()
        {
           try
            {
                var specialities = this.repository.AllReadonly<Speciality>(this.FilterByActiveId()).AsNoTracking();

                return specialities.To<SpecialityVM>().ToList();
            }catch(Exception e)
            {
                return null;
            }
        }
        public async Task<List<SpecialityVM>> GetAllSpecialitiesIncludeAsync(SpecialityVM specialityVM)
        {
            try
            {
                var specialities = this.repository.AllReadonly<Speciality>(this.FilterByActiveId()).AsNoTracking();

                return await specialities.To<SpecialityVM>(x => x.Profession).ToListAsync();
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task UpdateSpecialityOrdersAsync(SpecialityVM specialityVM)
        {
            specialityVM.Doc = null;
            Speciality specialtyFromDB = await this.repository.GetByIdAsync<Speciality>(specialityVM.IdSpeciality);
            this.repository.Detach<Speciality>(specialtyFromDB);

            if (specialtyFromDB.IdStatus != specialityVM.IdStatus)
            {
                specialtyFromDB = specialityVM.To<Speciality>();

                this.repository.Update<Speciality>(specialtyFromDB);
                await this.repository.SaveChangesAsync();

                this.repository.Detach<Speciality>(specialtyFromDB);
            }

            List<SpecialityOrder> specialityOrders = this.repository.AllReadonly<SpecialityOrder>(x => x.IdSpeciality == specialityVM.IdSpeciality).ToList();
            this.repository.HardDeleteRange<SpecialityOrder>(specialityOrders);

            await this.repository.SaveChangesAsync();

            foreach (var order in specialityVM.OrderVMs)
            {
                SpecialityOrder specialityOrder = new SpecialityOrder
                {
                    IdSPPOOOrder = order.IdOrder,
                    IdSpeciality = specialityVM.IdSpeciality,
                    IdTypeChange = order.IdTypeChange
                };

                await this.repository.AddAsync<SpecialityOrder>(specialityOrder);

                await this.repository.SaveChangesAsync();

                if (specialityOrder.SPPOOOrder != null)
                {
                    this.repository.Detach<SPPOOOrder>(specialityOrder.SPPOOOrder);
                }

                this.repository.Detach<SpecialityOrder>(specialityOrder);
            }
        }

        public async Task<string> CreateSpecialityAsync(SpecialityVM specialityVM)
        {
            specialityVM.Doc = null;

            bool checkCodeResult = this.DoesSpecialityWithCodeExist(specialityVM.Code);

            if (checkCodeResult)
            {
                return "Специалност с този код вече съществува!";
            }

            string msg = string.Empty;
 
 

            specialityVM.LinkMON = specialityVM.LinkMON == null ? string.Empty : specialityVM.LinkMON;
            specialityVM.LinkNIP = specialityVM.LinkNIP == null ? string.Empty : specialityVM.LinkNIP;

            try
            {
                Speciality speciality = specialityVM.To<Speciality>();
                speciality.IdProfession = specialityVM.IdProfession;
                speciality.Profession = null;

                await this.repository.AddAsync<Speciality>(speciality);
                var result = await this.repository.SaveChangesAsync();

                this.repository.Detach<Speciality>(speciality);

                specialityVM.IdSpeciality = speciality.IdSpeciality;

                if (result > 0)
                {
                    specialityVM.IdSpeciality = speciality.IdSpeciality;
                    msg = "Записът е успешен!";
                }
                else
                {
                    msg = "Грешка при запис в базата данни!";
                }

                foreach (var id in specialityVM.IdsNkpd)
                {
                    SpecialityNKPD specialityNKPD = new SpecialityNKPD
                    {
                        IdNKPD = id,
                        IdSpeciality = speciality.IdSpeciality
                    };

                    await this.repository.AddAsync<SpecialityNKPD>(specialityNKPD);
                    await this.repository.SaveChangesAsync();

                    this.repository.Detach<SpecialityNKPD>(specialityNKPD);
                }

                foreach (var order in specialityVM.OrderVMs)
                {
                    SpecialityOrder specialityOrder = new SpecialityOrder
                    {
                        IdSPPOOOrder = order.IdOrder,
                        IdSpeciality = speciality.IdSpeciality,
                        IdTypeChange = order.IdTypeChange
                    };

                    await this.repository.AddAsync<SpecialityOrder>(specialityOrder);
                    await this.repository.SaveChangesAsync();

                    this.repository.Detach<SpecialityOrder>(specialityOrder);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        public async Task<string> DeleteSpecialityAsync(int id)
        {
            string msg = string.Empty;

            try
            {
                await this.repository.HardDeleteAsync<Speciality>(id);

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

        public async Task<SpecialityVM> GetSpecialityByIdAsync(SpecialityVM specialityVM)
        {
            Speciality speciality = await this.repository.AllReadonly<Speciality>(FilterValue(specialityVM))
                .Include(o => o.SpecialityOrders)
                    .ThenInclude(or => or.SPPOOOrder)
                .AsNoTracking()
                .Include(n => n.SpecialityNKPDs)
                    .ThenInclude(nk => nk.NKPD)
                .AsNoTracking()
                .Include(doc => doc.DOC)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            this.repository.Detach<Speciality>(speciality);

            return speciality.To<SpecialityVM>();
        }

        public async Task<SpecialityVM> GetSpecialityWithProfessionIncludedByIdSpecialityAsync(int idSpeciality)
        {
            var data = this.repository.AllReadonly<Speciality>(x => x.IdSpeciality == idSpeciality);

            return await data.To<SpecialityVM>(x => x.Profession).FirstOrDefaultAsync();
        }

        public async Task<string> UpdateSpecialityAsync(SpecialityVM specialityVM)
        {
            specialityVM.SpecialityOrders = null;
            specialityVM.SpecialityNKPDs = null;
            Speciality specialtyFromDB = await this.repository.GetByIdAsync<Speciality>(specialityVM.IdSpeciality);
            this.repository.Detach<Speciality>(specialtyFromDB);

            if (specialityVM.Code != specialtyFromDB.Code)
            {
                bool checkCodeResult = this.DoesSpecialityWithCodeExist(specialityVM.Code);

                if (checkCodeResult)
                {
                    return "Специалност с този код вече съществува!";
                }
            }

            string msg = string.Empty;

            try
            {
                specialtyFromDB = specialityVM.To<Speciality>();
                specialtyFromDB.Profession = null;
                specialtyFromDB.DOC = null;
 
                this.repository.Update<Speciality>(specialtyFromDB);
                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    msg = "Записът е успешен!";
                }
                else
                {
                    msg = "Грешка при запис в базата данни!";
                }

                this.repository.Detach<Speciality>(specialtyFromDB);

                if (specialtyFromDB.DOC != null)
                {
                    this.repository.Detach<Data.Models.Data.DOC.DOC>(specialtyFromDB.DOC);
                }

                List<SpecialityNKPD> specialityNKPDs = this.repository.AllReadonly<SpecialityNKPD>(x => x.IdSpeciality == specialityVM.IdSpeciality).AsNoTracking().ToList();
                this.repository.HardDeleteRange<SpecialityNKPD>(specialityNKPDs);
                await this.repository.SaveChangesAsync();

                List<SpecialityOrder> specialityOrders = this.repository.AllReadonly<SpecialityOrder>(x => x.IdSpeciality == specialityVM.IdSpeciality).AsNoTracking().ToList();
                this.repository.HardDeleteRange<SpecialityOrder>(specialityOrders);
                await this.repository.SaveChangesAsync();

                foreach (var id in specialityVM.IdsNkpd)
                {
                    SpecialityNKPD specialityNKPD = new SpecialityNKPD
                    {
                        IdNKPD = id,
                        IdSpeciality = specialtyFromDB.IdSpeciality
                    };

                    await this.repository.AddAsync<SpecialityNKPD>(specialityNKPD);
                    await this.repository.SaveChangesAsync();

                    if (specialityNKPD.NKPD != null)
                    {
                        this.repository.Detach<NKPD>(specialityNKPD.NKPD);
                    }

                    this.repository.Detach<SpecialityNKPD>(specialityNKPD);
                }

                foreach (var order in specialityVM.OrderVMs)
                {
                    SpecialityOrder specialityOrder = new SpecialityOrder
                    {
                        IdSPPOOOrder = order.IdOrder,
                        IdSpeciality = specialtyFromDB.IdSpeciality,
                        IdTypeChange = order.IdTypeChange
                    };

                    await this.repository.AddAsync<SpecialityOrder>(specialityOrder);
                    await this.repository.SaveChangesAsync();

                    if (specialityOrder.SPPOOOrder != null)
                    {
                        this.repository.Detach<SPPOOOrder>(specialityOrder.SPPOOOrder);
                    }

                    this.repository.Detach<SpecialityOrder>(specialityOrder);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        public async Task<IEnumerable<SpecialityVM>> GetAllSpecialitiesAsync(SpecialityVM specialityVM)
        {
            var doc = this.repository.All<Data.Models.Data.SPPOO.Speciality>(FilterValue(specialityVM));

            IEnumerable<SpecialityVM> result = await doc.To<SpecialityVM>().ToListAsync();

            return result;
        }

        public async Task<List<SpecialityVM>> GetSpecialitiesByDocIdAsync(int idDoc)
        {

            var doc = this.repository.All<Data.Models.Data.SPPOO.Speciality>(d => d.IdDOC == idDoc);

            var list = await doc.To<SpecialityVM>(s => s.Profession.ProfessionalDirection).ToListAsync();

            return list;
        }

        public async Task<List<SpecialityVM>> GetSpecialitiesByERUIdAsync(int idERU)
        {
            var specIds = this.repository.All<ERUSpeciality>(e => e.IdERU == idERU).Select(e => e.IdSpeciality).ToList();

            var listSpec = this.repository.All<Data.Models.Data.SPPOO.Speciality>(s => specIds.Contains(s.IdSpeciality));

            return await listSpec.To<SpecialityVM>(s => s.Profession.ProfessionalDirection).ToListAsync();
        }

        public async Task<SpecialityVM> GetSpecialityByIdAsync(int id)
        {
            var speciality = this.repository.All<Data.Models.Data.SPPOO.Speciality>(s => s.IdSpeciality == id);

            var list = await speciality.To<SpecialityVM>(s => s.Profession.ProfessionalDirection).ToListAsync();
            
            return list.FirstOrDefault();

        }

        public SpecialityVM GetSpecialityById(int id)
        {
            var speciality = this.repository.All<Data.Models.Data.SPPOO.Speciality>(s => s.IdSpeciality == id);

            var list = speciality.To<SpecialityVM>(s => s.Profession.ProfessionalDirection).ToList();

            return list.FirstOrDefault();

        }

        public IEnumerable<SpecialityVM> GetAllSpecialities(SpecialityVM specialityVM)
        {
            var doc = this.repository.All<Data.Models.Data.SPPOO.Speciality>(FilterValue(specialityVM));

            IEnumerable<SpecialityVM> result = doc.To<SpecialityVM>().ToList();

            return result;
        }

        public async Task<IEnumerable<SpecialityVM>> GetSpecialitiesByListIdsAsync(List<int> ids)
        {
            IQueryable<Speciality> specialities = this.repository.AllReadonly<Speciality>(x => ids.Contains(x.IdSpeciality));

            return await specialities.To<SpecialityVM>(
                x => x.Profession,
                x => x.Profession.ProfessionalDirection).ToListAsync();
        }

        public List<int> GetSpecialitiesIdsByIdArea(int idArea)
        {
            var listIds = new List<int>();
            try
            {
                var professionalDirections = this.dataSourceService.GetAllProfessionalDirectionsList().Where(x => x.IdArea == idArea && x.IdStatus == dataSourceService.GetActiveStatusID());
                var listProfessionalDirectionIds = professionalDirections.Select(x => x.IdProfessionalDirection).ToList();
                var professions = this.dataSourceService.GetAllProfessionsList().Where(x => listProfessionalDirectionIds.Contains(x.IdProfessionalDirection) && x.IdStatus == dataSourceService.GetActiveStatusID());
                var professionIds = professions.Select(x => x.IdProfession).ToList();
                var specialities = this.dataSourceService.GetAllSpecialitiesList().Where(x => professionIds.Contains(x.IdProfession) && x.IdStatus == dataSourceService.GetActiveStatusID());
                var specialityIds = specialities.Select(x => x.IdSpeciality).ToList();

                listIds.AddRange(specialityIds);
            }
            catch (Exception ex)
            {

            }

            return listIds;
        }

        public List<int> GetSpecialitiesIdsByIdProfessionalDirection(int idProfessionalDirection)
        {
            var listIds = new List<int>();
            try
            {
                var professions = this.dataSourceService.GetAllProfessionsList().Where(x => x.IdProfessionalDirection == idProfessionalDirection && x.IdStatus == dataSourceService.GetActiveStatusID());
                var professionIds = professions.Select(x => x.IdProfession).ToList();
                var specialities = this.dataSourceService.GetAllSpecialitiesList().Where(x => professionIds.Contains(x.IdProfession) && x.IdStatus == dataSourceService.GetActiveStatusID());
                var specialityIds = specialities.Select(x => x.IdSpeciality).ToList();

                listIds.AddRange(specialityIds);
            }
            catch (Exception ex)
            {

            }

            return listIds;
        }

        public List<int> GetSpecialitiesIdsByIdProfession(int idProfession)
        {
            var listIds = new List<int>();
            try
            {
                var specialities = this.dataSourceService.GetAllSpecialitiesList().Where(x => x.IdProfession == idProfession && x.IdStatus == dataSourceService.GetActiveStatusID());
                var specialityIds = specialities.Select(x => x.IdSpeciality).ToList();

                listIds.AddRange(specialityIds);
            }
            catch (Exception ex)
            {

            }

            return listIds;
        }

        public List<int> GetSpecialitiesIdsByIdDOC(int idDOC)
        {
            var listIds = new List<int>();
            try
            {
                var specialities = this.dataSourceService.GetAllSpecialitiesList().Where(x => x.IdDOC == idDOC && x.IdStatus == dataSourceService.GetActiveStatusID());
                var specialityIds = specialities.Select(x => x.IdSpeciality).ToList();

                listIds.AddRange(specialityIds);
            }
            catch (Exception ex)
            {

            }

            return listIds;
        }

        private bool DoesSpecialityWithCodeExist(string code)
        {
            return this.repository.AllReadonly<Speciality>(x => x.Code == code && x.IdStatus == dataSourceService.GetActiveStatusID()).Any();
        }

        protected Expression<Func<Data.Models.Data.SPPOO.Speciality, bool>> FilterValue(SpecialityVM model)
        {
            var predicate = PredicateBuilder.True<Data.Models.Data.SPPOO.Speciality>();

            if (!string.IsNullOrEmpty(model.SpecialityComboFilter))
            {
                predicate = predicate.And(p => p.Name.Contains(model.SpecialityComboFilter) || p.Code.Contains(model.SpecialityComboFilter));
            }

            if (model.IdProfession > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdProfession == model.IdProfession);
            }

            if (model.IdSpeciality > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdSpeciality == model.IdSpeciality);
            }

            if (model.IdStatus > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdStatus == model.IdStatus);
            }

            return predicate;
        }

        protected Expression<Func<Speciality, bool>> FilterByProfessionId(ProfessionVM model)
        {
            var predicate = PredicateBuilder.True<Speciality>();

            predicate = predicate.And(p => p.IdProfession == model.IdProfession && p.IdStatus != dataSourceService.GetRemoveStatusID());

            return predicate;
        }

        protected Expression<Func<Speciality, bool>> FilterByActiveId()
        {
            var predicate = PredicateBuilder.True<Speciality>();

            predicate = predicate.And(p => p.IdStatus == dataSourceService.GetActiveStatusID());

            return predicate;
        }
    }
}
