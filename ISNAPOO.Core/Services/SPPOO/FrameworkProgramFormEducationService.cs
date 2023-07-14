namespace ISNAPOO.Core.Services.SPPOO
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    using Microsoft.EntityFrameworkCore;
    
    using Data.Models.Common;
    using Data.Models.Data.SPPOO;
    
    using ISNAPOO.Core.Contracts.SPPOO;
    using ISNAPOO.Core.Mapping;
    using ISNAPOO.Core.ViewModels.SPPOO;
    
    public class FrameworkProgramFormEducationService : BaseService, IFrameworkProgramFormEducationService
    {
        private readonly IRepository repository;
        public FrameworkProgramFormEducationService(IRepository repository) : base(repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<FrameworkProgramFormEducationVM>> GetAllAsync()
        {
            var data = this.repository.All<FrameworkProgramFormEducation>();

            var result = await data.To<FrameworkProgramFormEducationVM>().ToListAsync();

            return result;
        }

        public async Task<IEnumerable<FrameworkProgramFormEducationVM>> GetAllFrameworkProgramsFormEducationsByIdAsync(int idFrameworkProgram)
        {
            var data = this.repository.All<FrameworkProgramFormEducation>(x => x.IdFrameworkProgram == idFrameworkProgram);

            var result = await data.To<FrameworkProgramFormEducationVM>().ToListAsync();
            
            return result;
        }

        public async Task<int> UpdateFrameworkProgramFormEducationAsync(int idFrameworkProgram, int[] formEducationIds)
        {


            var formEducationForDelete = this.repository.All<FrameworkProgramFormEducation>(x => x.IdFrameworkProgram == idFrameworkProgram ).ToList();

            this.repository.HardDeleteRange<FrameworkProgramFormEducation>(formEducationForDelete);
            var result = await this.repository.SaveChangesAsync();

            List<FrameworkProgramFormEducation> newFormEducationList = new List<FrameworkProgramFormEducation>();

           

            for (int i = 0; i < formEducationIds.Length; i++)
            {
                if (formEducationIds[i] != 0)
                {
                    var formEducation = new FrameworkProgramFormEducation
                    {
                        IdFormEducation = formEducationIds[i],
                        IdFrameworkProgram = idFrameworkProgram
                    };

                    newFormEducationList.Add(formEducation);
                }
            }

            if (newFormEducationList.Count != 0)            {
                await this.repository.AddRangeAsync<FrameworkProgramFormEducation>(newFormEducationList);

                result = await this.repository.SaveChangesAsync();

            }

            //var formEducations = await GetAllFrameworkProgramsFormEducationsByIdAsync(idFrameworkProgram);

            // await this.RemoveFrameworkProgramFormEducation(formEducations.ToList());

            //  var result = await this.CreateFrameworkProgramFormEducation(idFrameworkProgram, formEducationIds);

            //   var result = await this.repository.SaveChangesAsync();

            return result;
        }

        public async Task<int> CreateFrameworkProgramFormEducation(int idFrameworkProgram, int[] formEducationIds)
        {
            int result = 0;
            List<FrameworkProgramFormEducation> newFormEducationList = new List<FrameworkProgramFormEducation>();

            for (int i = 0; i < formEducationIds.Length; i++)
            {
                if (formEducationIds[i] != 0)
                {
                    var formEducation = new FrameworkProgramFormEducation
                    {
                        IdFormEducation = formEducationIds[i],
                        IdFrameworkProgram = idFrameworkProgram
                    };

                    newFormEducationList.Add(formEducation);
                }
            }

            if (newFormEducationList.Count != 0)
            {
                await this.repository.AddRangeAsync<FrameworkProgramFormEducation>(newFormEducationList);
                result = await this.repository.SaveChangesAsync();
            }

            return result;
        }

        public async Task RemoveFrameworkProgramFormEducation(List<FrameworkProgramFormEducationVM> frameworkProgramsFormEducationsList)
        {
            List<int> formEducationIds = new List<int>();

            foreach (var formEducaiton in frameworkProgramsFormEducationsList)
            {
                formEducationIds.Add(formEducaiton.IdFrameworkProgramFormEducation);
            }

            var documentsToDelete = this.repository
                .All<FrameworkProgramFormEducation>(x => formEducationIds.Contains(x.IdFrameworkProgramFormEducation))
                .ToList();

            this.repository.HardDeleteRange<FrameworkProgramFormEducation>(documentsToDelete);
            await this.repository.SaveChangesAsync();
        }
    }
}
