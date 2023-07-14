using System;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Registers.Courses
{
    public partial class CourseModal : BlazorBaseComponent
    {
        CourseVM model = new CourseVM();

        SfDialog sfDialog;

        public async Task openModal(CourseVM course)
        {
            this.model = course;

            this.isVisible = true;

            this.StateHasChanged();
        }
    }
}

