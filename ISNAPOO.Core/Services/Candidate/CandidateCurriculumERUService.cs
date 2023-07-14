using Data.Models.Common;
using Data.Models.Data.Candidate;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.DOC;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Candidate
{
    public class CandidateCurriculumERUService : BaseService, ICandidateCurriculumERUService
    {
        private readonly IRepository repository;
        private readonly ILogger<CandidateCurriculumERUService> _logger;

        public CandidateCurriculumERUService(IRepository repository, ILogger<CandidateCurriculumERUService> logger)
            : base(repository)
        {
            this.repository = repository;
            this._logger = logger;
        }

        public async Task<IEnumerable<CandidateCurriculumERUVM>> GetAllCandidateCurriculumERUByCandidateCurriculumIdAsync(CandidateCurriculumVM candidateCurriculum)
        {
            IQueryable<CandidateCurriculumERU> data = this.repository.AllReadonly<CandidateCurriculumERU>(x => x.IdCandidateCurriculum == candidateCurriculum.IdCandidateCurriculum);

            return await data.To<CandidateCurriculumERUVM>().ToListAsync();
        }

        public CandidateCurriculumERUVM GetCandidateCurriculumERUByIdCandidateCurriculumAndIdERU(int idCandidateCurriculum, int idEru)
        {
            try
            {
                var data = this.repository.AllReadonly<CandidateCurriculumERU>(x => x.IdCandidateCurriculum == idCandidateCurriculum && x.IdERU == idEru).FirstOrDefault();

                if (data == null)
                {
                    return null;
                }
                else
                {
                    return data.To<CandidateCurriculumERUVM>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);

                return null;
            }
        }

        public async Task<ResultContext<NoResult>> DeleteCandidateCurriculumERUAsync(int idCandidateCurriculumERU)
        {
            var resultContext = new ResultContext<NoResult>();

            try
            {
                await this.repository.HardDeleteAsync<CandidateCurriculumERU>(idCandidateCurriculumERU);
                await this.repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }

        public async Task<ResultContext<NoResult>> AddERUsToCurriculumListAsync(List<ERUVM> erus, List<CandidateCurriculumVM> curriculums)
        {
            var output = new ResultContext<NoResult>();

            try
            {
                foreach (var curriculum in curriculums)
                {
                    foreach (var eru in erus)
                    {
                        var candidateCurriculumERU = this.repository.AllReadonly<CandidateCurriculumERU>(x => x.IdCandidateCurriculum == curriculum.IdCandidateCurriculum && x.IdERU == eru.IdERU).FirstOrDefault();
                        if (candidateCurriculumERU is null)
                        {
                            CandidateCurriculumERU curriculumERU = new CandidateCurriculumERU()
                            {
                                IdCandidateCurriculum = curriculum.IdCandidateCurriculum,
                                IdERU = eru.IdERU,
                            };

                            await this.repository.AddAsync<CandidateCurriculumERU>(curriculumERU);
                        }
                    }
                }

                await this.repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);

                output.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return output;
        }
    }
}
