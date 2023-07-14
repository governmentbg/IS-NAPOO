using Data.Models.Data.ProviderData;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace ISNAPOO.WebSystem.Pages.Archive
{
    public partial class AnnualInfoReportConfirmModal : BlazorBaseComponent
    {
        [Inject]
        public ISettingService settingService { get; set; }
        [Inject]
        public ICandidateProviderService candidateProviderService { get; set; }
        [Inject]
        public IPersonService personService { get; set; }
        [Inject]
        public IArchiveService ArchiveService { get; set; }
        public async Task openModal()
        {
            this.isVisible = true;

            this.StateHasChanged();
        }

        public async Task sendReport()
        {
            var setting = await settingService.GetSettingByIntCodeAsync("DateRangeForReport");

            var dateNow = DateTime.Now.Date;

            string[] dates = setting.SettingValue.Split("-");
            if (DateTime.Parse(dates[0]).Date < dateNow && dateNow < DateTime.Parse(dates[1]).Date)
            {
                AnnualInfoVM annualInfo = new AnnualInfoVM();

                var candidate = await this.candidateProviderService.GetCandidateProviderByIdAsync(new CandidateProviderVM() { IdCandidate_Provider = this.UserProps.IdCandidateProvider });

                annualInfo.IdCandidateProvider = this.UserProps.IdCandidateProvider;

                annualInfo.Year = dateNow.Year;

                var person = await this.personService.GetPersonByIdAsync(this.UserProps.IdPerson);

                annualInfo.Name = $"{person.FirstName} {person.FamilyName}";

                annualInfo.Title = person.Position;

                annualInfo.Phone = person.Phone;

                annualInfo.Email = person.Email;

                annualInfo.IdCreateUser = this.UserProps.UserId;

                annualInfo.CreationDate = DateTime.Now;

                annualInfo.IdModifyUser = this.UserProps.UserId;

                annualInfo.ModifyDate = DateTime.Now;

                var isAlreadySend = ArchiveService.GetAnnualInfoByIdCandProvAndYear(this.UserProps.IdCandidateProvider, DateTime.Now.Year);
                if (isAlreadySend == null)
                    await ArchiveService.SaveAnnualInfoAsync(annualInfo);
                else
                    await this.ShowErrorAsync($"Вече имате подаден отчет за {DateTime.Now.Year} година!");
                this.isVisible = false;

                this.StateHasChanged();
                
            }else
            {
                await this.ShowErrorAsync($"Времето за подаване на отчетите е {setting.SettingValue}");
            }
        }
    }
}
