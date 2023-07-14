using System;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.Services.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Qualification
{
    public partial class ExpertsActivityModal : BlazorBaseComponent
    {

        CandidateProviderVM model;

        List<ClientCourseDocumentVM> documents;

        SfGrid<ClientCourseDocumentVM> sfGrid;

        public ExpertsActivityModal()
        {
        }

        [Inject]
        ITrainingService trainingService { get; set; }
        [Inject]
        IDataSourceService dataSourceService { get; set; }

        public async Task openModal(CandidateProviderVM candidate)
        {
            this.isVisible = true;
            model = candidate;

            documents = trainingService.GetDocumentsByCandidateProvder(candidate);


            this.StateHasChanged();
        }
    }


  
}

