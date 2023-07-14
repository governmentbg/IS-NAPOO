using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Syncfusion.Blazor.Popups;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.Contracts.DOC;
using Microsoft.AspNetCore.Components;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.Contracts.Common;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    partial class ValidationClientInformationModal : BlazorBaseComponent
    {
        #region Inject
        [Inject]
        public IDOCService DOCService { get; set; }
        [Inject]
        public IDataSourceService DataSourceService { get; set; }
        #endregion

        private ValidationClientVM client = new ValidationClientVM();
        private int selectedTab = 0;
        private SfDialog sfDialog = new SfDialog();
        private bool HideWhenSPK = false;
        private int PageType = 0;
        private DocVM doc = new DocVM();
        private KeyValueVM kvCourseFinished = new KeyValueVM();
        private KeyValueVM kvCourseCompleted = new KeyValueVM();
        public async Task OpenModal(ValidationClientVM validation, int pageType)
        {
            this.selectedTab = 0;
            this.client = validation;
            this.kvCourseCompleted = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusFinished");
            this.kvCourseFinished = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type5");
            if (this.client.Speciality is not null)
            {
                this.doc = await this.DOCService.GetActiveDocByProfessionIdAsync(this.client.Speciality.Profession);
            }
            this.PageType = pageType;
            this.isVisible = true;
            this.StateHasChanged();
        }
    }
}
