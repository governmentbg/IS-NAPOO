namespace ISNAPOO.Core.Services.SPPOO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using Data.Models.Common;
    using Data.Models.Data.Common;
    using Data.Models.Data.SPPOO;

    using ISNAPOO.Common.Constants;
    using ISNAPOO.Common.Framework;
    using ISNAPOO.Core.Contracts.SPPOO;
    using ISNAPOO.Core.HelperClasses;
    using ISNAPOO.Core.Mapping;
    using ISNAPOO.Core.ViewModels.SPPOO;
    using System.Text.RegularExpressions;
    using ISNAPOO.Core.Contracts.Common;
    using Microsoft.Extensions.Logging;
    using ISNAPOO.Core.ViewModels.Common.ValidationModels;

    public class FrameworkProgramService : BaseService, IFrameworkProgramService
    {
        private readonly IRepository repository;
        private readonly IFrameworkProgramFormEducationService frameworkProgramFormEducationService;
        private readonly IDataSourceService dataSourceService;
        private readonly ILogger<FrameworkProgramService> logger;

        public FrameworkProgramService(
            IRepository repository,
            IFrameworkProgramFormEducationService frameworkProgramFormEducationService, IDataSourceService dataSourceService, ILogger<FrameworkProgramService> logger) : base(repository)
        {
            this.repository = repository;
            this.frameworkProgramFormEducationService = frameworkProgramFormEducationService;
            this.dataSourceService = dataSourceService;
            this.logger = logger;

        }

        public async Task<IEnumerable<FrameworkProgramVM>> GetAllFrameworkProgramsAsync(FrameworkProgramVM filter)
        {
            var data = this.repository.All<FrameworkProgram>(FilterFrameworkProgram(filter));
            var regex = new Regex(@"^\D[0-9]{1}$");
            var keyValues = this.dataSourceService.GetAllKeyValueList();
            //var formEducations = await this.frameworkProgramFormEducationService.GetAllAsync();
            var result = await data.To<FrameworkProgramVM>(x => x.FrameworkProgramFormEducations).ToListAsync();
            var statusesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("StatusTemplate");

            result.ForEach(r =>
            {
                if (r.IdStatus.HasValue)
                {
                    var status = statusesSource.FirstOrDefault(x => x.IdKeyValue == r.IdStatus.Value);
                    if (status is not null)
                    {
                        r.StatusValue = status.Name;
                    }
                }

                if (regex.IsMatch(r.Name))
                {
                    var regName = string.Empty;
                    char[] splitName = new char[r.Name.Length + 1];
                    for (int i = 0; i < r.Name.Length; i++)
                    {
                        if (char.IsDigit(r.Name[i]))
                        {
                            regName += "0";
                        }

                        regName += r.Name[i];
                    }

                    r.FrameworkProgramNameFormatted = regName;
                }

                if (string.IsNullOrEmpty(r.FrameworkProgramNameFormatted))
                {
                    r.FrameworkProgramNameFormatted = r.Name;
                }
                if(r.IdVQS != 0)
                r.VQSName = keyValues.Where(k => r.IdVQS == k.IdKeyValue).FirstOrDefault().Name;
                if (r.IdTypeFrameworkProgram != 0)
                r.TypeFrameworkProgramName = keyValues.Where(k => r.IdTypeFrameworkProgram == k.IdKeyValue).FirstOrDefault().Name;
                if (r.IdQualificationLevel != 0)
                    r.QualificationLevelName = keyValues.Where(k => r.IdQualificationLevel == k.IdKeyValue).FirstOrDefault().Name;
                r.FormEducationNames = String.Join(", ", keyValues.Where(k => r.FrameworkProgramFormEducations.Select(fe => fe.IdFormEducation).Contains(k.IdKeyValue)).Select(k => k.Name));
            });

            //return await data.To<TemplateDocumentVM>().ToListAsync();
            return result.OrderBy(x => x.Name);
        }

        public async Task<ResultContext<List<FrameworkProgramVM>>> GetAllFrameworkProgramsAsync(ResultContext<FrameworkProgramVM> resultContext)
        {
            var data = this.repository.All<FrameworkProgram>(FilterFrameworkProgram(resultContext.ResultContextObject));
            var regex = new Regex(@"^\D[0-9]{1}$");
           // var keyValues = this.repository.All<KeyValue>();
            var keyValues = this.dataSourceService.GetAllKeyValueList();
            //var formEducations = await this.frameworkProgramFormEducationService.GetAllAsync();
            var result = await data.To<FrameworkProgramVM>(x => x.FrameworkProgramFormEducations).ToListAsync();

            result.ForEach(r =>
            {
                if (regex.IsMatch(r.Name))
                {
                    var regName = string.Empty;
                    char[] splitName = new char[r.Name.Length + 1];
                    for (int i = 0; i < r.Name.Length; i++)
                    {
                        if (char.IsDigit(r.Name[i]))
                        {
                            regName += "0";
                        }

                        regName += r.Name[i];
                    }

                    r.FrameworkProgramNameFormatted = regName;
                }

                if (string.IsNullOrEmpty(r.FrameworkProgramNameFormatted))
                {
                    r.FrameworkProgramNameFormatted = r.Name;
                }
                r.VQSName = keyValues.Where(k => r.IdVQS == k.IdKeyValue).FirstOrDefault().Name;
                r.TypeFrameworkProgramName = keyValues.Where(k => r.IdTypeFrameworkProgram == k.IdKeyValue).FirstOrDefault().Name;
                r.QualificationLevelName = keyValues.Where(k => r.IdQualificationLevel == k.IdKeyValue).FirstOrDefault().Name;
                r.FormEducationNames = String.Join(", ", keyValues.Where(k => r.FrameworkProgramFormEducations.Select(fe => fe.IdFormEducation).Contains(k.IdKeyValue)).Select(k => k.Name));
            });

            resultContext.AddErrorMessage("test");
            ResultContext<List<FrameworkProgramVM>> returnContext = new ResultContext<List<FrameworkProgramVM>>();

            returnContext.ResultContextObject = result;

            return returnContext;
        }

        protected Expression<Func<FrameworkProgram, bool>> FilterFrameworkProgram(FrameworkProgramVM model)
        {
            var predicate = PredicateBuilder.True<FrameworkProgram>();

            return predicate;
        }

        public async Task<int> CreateFrameworkProgram(FrameworkProgramVM model)
        {
            var newTemplate = model.To<FrameworkProgram>();
 

            if (newTemplate.Practice == 0.0)
            {
                newTemplate.Practice = 0.1;
            }

            if (newTemplate.SectionB == 0.0)
            {
                newTemplate.SectionB = 0.1;
            }
            
            if (newTemplate.SectionА1 == 0.0)
            {
                newTemplate.SectionА1 = 0.1;
            }
            
            if (newTemplate.SectionА == 0.0)
            {
                newTemplate.SectionА = 0.1;
            }

            newTemplate.Theory = 0.1;

            if (string.IsNullOrEmpty(newTemplate.MinimumLevelQualification))
            {
                newTemplate.MinimumLevelQualification = "";
            }
            if (string.IsNullOrEmpty(newTemplate.Description))
            {
                newTemplate.Description = "";
            }
            
            if (string.IsNullOrEmpty(newTemplate.ShortDescription))
            {
                newTemplate.ShortDescription = "";
            }

            await this.repository.AddAsync<FrameworkProgram>(newTemplate);
            var result = await this.repository.SaveChangesAsync();

            if (result == 1)
            {
                result = newTemplate.IdFrameworkProgram;
                model.IdFrameworkProgram = newTemplate.IdFrameworkProgram;
            }

            await this.frameworkProgramFormEducationService.CreateFrameworkProgramFormEducation(model.IdFrameworkProgram, model.FormEducationIds);

            return result;
        }

        public async Task<int> UpdateFrameworkProgramAsync(FrameworkProgramVM model)
        {
            if (model.IdFrameworkProgram == 0)
            {
                var result = await this.CreateFrameworkProgram(model);
                return result;
            }
            else
            {
                var updatedEntity = await this.GetByIdAsync<FrameworkProgram>(model.IdFrameworkProgram);
                this.repository.Detach<FrameworkProgram>(updatedEntity);

                updatedEntity.To<FrameworkProgramVM, FrameworkProgram>(model, updatedEntity);
 

                this.repository.Update(updatedEntity);
                var result = await this.repository.SaveChangesAsync();

                await this.frameworkProgramFormEducationService.UpdateFrameworkProgramFormEducationAsync(model.IdFrameworkProgram, model.FormEducationIds);

                return result;
            }
        }

        public async Task<FrameworkProgramVM> GetFrameworkProgramByIdAsync(int id)
        {
            FrameworkProgram framworkProgram = await this.repository.GetByIdAsync<FrameworkProgram>(id);
            var frameworkProgramFormEducationsList = await this.frameworkProgramFormEducationService.GetAllFrameworkProgramsFormEducationsByIdAsync(framworkProgram.IdFrameworkProgram);

           
            //this.repository.Detach<FrameworkProgram>(framworkProgram);
            FrameworkProgramVM frameworkProgramVM = framworkProgram.To<FrameworkProgramVM>();
            frameworkProgramVM.FrameworkProgramFormEducations = frameworkProgramFormEducationsList.ToList();
            return frameworkProgramVM;
        }

        public async Task<FrameworkProgramVM> GetFrameworkPgoramByIdWithFormEducationsIncludedAsync(FrameworkProgramVM frameworkProgramVM)
        {
            IQueryable<FrameworkProgram> frameworkPrograms = this.repository.AllReadonly<FrameworkProgram>(x => x.IdFrameworkProgram == frameworkProgramVM.IdFrameworkProgram);
            var result = frameworkPrograms.To<FrameworkProgramVM>(x => x.FrameworkProgramFormEducations);

            return await result.FirstOrDefaultAsync();
        }

        public async Task RemoveFrameworkProgram(FrameworkProgramVM frameworkProgram)
        {
            int frameworksId = frameworkProgram.IdFrameworkProgram;

            
            var documentsToDelete = this.repository
                .All<FrameworkProgram>(x => frameworksId == x.IdFrameworkProgram)
                .ToList();

            this.repository.HardDeleteRange<FrameworkProgram>(documentsToDelete);
            await this.repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<FrameworkProgramVM>> GetAllFrameworkProgramsBySpecialityVQSIdAsync(int specialityVQSId)
        {
            IQueryable<FrameworkProgram> frameworkPrograms = this.repository.AllReadonly<FrameworkProgram>(x => x.IdVQS == specialityVQSId);

            return await frameworkPrograms.To<FrameworkProgramVM>(x => x.FrameworkProgramFormEducations).OrderBy(x => x.Name).ToListAsync();
        }

        public List<FrameworkProgramVM> getAllFrameworkProgramsWithoutAsync()
        {
            return this.repository.All<FrameworkProgram>().To<FrameworkProgramVM>().ToList();
        }

        public async Task<IEnumerable<FrameworkProgramVM>> GetAllFrameworkProgramsBySpecialityVQSIdAndIdTypeOfFrameworkAsync(int specialityVQSId, int idTypeOfFramework)
        {
            KeyValueVM currentType = new KeyValueVM();
            try
            {
                var types = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram");
                if (idTypeOfFramework != 0)
                {
                     currentType = types.Where(x => x.IdKeyValue == idTypeOfFramework).First();
                }
                
                var frameworkType = 0;

                if (currentType.KeyValueIntCode == "ValidationOfPartOfProfession")
                {
                        frameworkType = types.Where(x => x.KeyValueIntCode.Equals("PartProfession")).First().IdKeyValue;
                }               
                else
                {
                    frameworkType = types.Where(x => x.KeyValueIntCode.Equals("ProfessionalQualification")).First().IdKeyValue;
                }
                IQueryable<FrameworkProgram> frameworkPrograms = this.repository.AllReadonly<FrameworkProgram>(x => x.IdVQS == specialityVQSId && x.IdTypeFrameworkProgram == frameworkType);

                return await frameworkPrograms.To<FrameworkProgramVM>(x => x.FrameworkProgramFormEducations).OrderBy(x => x.Name).ToListAsync();

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                logger.LogError(ex.InnerException?.Message);
                logger.LogError(ex.StackTrace);
                return null;
            }
        }
    }
}
