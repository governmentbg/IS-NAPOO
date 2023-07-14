using AutoMapper.Configuration.Annotations;
using Data.Models.Data.Archive;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Archive
{
    public partial class AnnualInfoReportDataModal : BlazorBaseComponent
    {
        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IArchiveService ArchiveService { get; set; }
        private IEnumerable<AnnualInfoStatusVM> annualInfoSource = new List<AnnualInfoStatusVM>();
        private AnnualInfoVM annualInfo = new AnnualInfoVM();
        private SfGrid<AnnualInfoStatusVM> sfGrid = new SfGrid<AnnualInfoStatusVM>();
        private string cpoCipo = string.Empty;
        private SfDialog sfDialog = new SfDialog();


        public async Task OpenModal(CandidateProviderVM candidateProviderVM, int year, string licensingType)
        {
            this.isVisible = true;
            annualInfo = new AnnualInfoVM();
            annualInfoSource = new List<AnnualInfoStatusVM>();
            annualInfo = candidateProviderVM.AnnualInfos.FirstOrDefault(x => x.Year == year);
            if (annualInfo != null)
            {
                annualInfoSource = annualInfo.AnnualInfoStatuses;
                foreach (var status in annualInfoSource)
                {
                    var kvStatus = await this.DataSourceService.GetKeyValueByIdAsync(status.IdStatus);
                    status.StatusValue = kvStatus.Name;
                    status.StatusValueIntCode = kvStatus.KeyValueIntCode;
                    status.StatusDate = status.ModifyDate;
                    status.PersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(status.IdCreateUser);
                    status.Title = await this.ApplicationUserService.GetPersonTitleAsync(status.IdCreateUser);
                }
            }
            if (licensingType == "InfoNAPOOCPO")
            {
                this.cpoCipo = candidateProviderVM.CPONameOwnerGrid;
            }
            else if (licensingType != "InfoNAPOOCIPO")
            {
                this.cpoCipo = candidateProviderVM.CIPONameOwnerGrid;
            }
            this.StateHasChanged();
        }


    }
}
