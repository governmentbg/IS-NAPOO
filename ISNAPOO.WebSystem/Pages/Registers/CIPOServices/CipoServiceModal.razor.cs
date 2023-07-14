using System;
using Data.Models.Data.Candidate;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Register;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Registers.CIPOServices
{
    public partial class CipoServiceModal : BlazorBaseComponent
    {
        private SfDialog sfDialog = new SfDialog();
        ClientVM model = new ClientVM();

        [Inject]
        IDataSourceService dataSourceService { get; set; }
        [Inject]
        ITrainingService trainingService { get; set; }
        [Inject]
        IUploadFileService uploadFileService { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        private string modalHeaderDoc = string.Empty;
        private string modalHeaderName = string.Empty;
        private string header1 = string.Empty;
        private string header2 = string.Empty;
        private string header3 = string.Empty;
        private string header4 = string.Empty;
        private string timeSpan = string.Empty;
        private int idConsultingType = 0;
        private KeyValueVM sex = new KeyValueVM();
        private KeyValueVM nationality = new KeyValueVM();
        private KeyValueVM typeDocument = new KeyValueVM();
        private IEnumerable<ConsultingVM> consultingsSource = new List<ConsultingVM>();
        private ConsultingVM consultingToDelete = new ConsultingVM();
        private SfGrid<ConsultingVM> consultingsGrid = new SfGrid<ConsultingVM>();
        private List<KeyValueVM> kvConsultingTypeSource = new List<KeyValueVM>();
        private List<KeyValueVM> kvAllConsultingTypeSource = new List<KeyValueVM>();
        private bool isEditable = false;
        private ConsultingClientVM ConsultingClientVM { get; set; }
        public async Task OpenModal(ClientVM model, bool isEditable = false)
        {
            this.isEditable = isEditable;
            this.model = model;
            this.ConsultingClientVM = await trainingService.GetConsultingClientByIdClientAsync(model.IdClient, model.IdCandidateProvider);
            header1 = $"Информация за ЦИПО: {this.model.CandidateProvider.ProviderOwner}";
            header2 = $"Информация за лицето, което е ползвало услугите по информиране в ЦИПО: {this.model.FirstName} {this.model.FamilyName}";
            header3 = $"Информация за услугите:";
            //header4 = $"Информация за документа за ПК: {this.model.DocumentRegNo}";
            //if (this.model.DocumentDate != null)
            //    modalHeaderDoc = $"{this.model.DocumentRegNo}/{this.model.DocumentDate.Value.ToString(GlobalConstants.DATE_FORMAT)}";
            //else
            //    modalHeaderDoc = $"{this.model.DocumentRegNo}/";
            modalHeaderName = $"{this.model.FirstName} {this.model.FamilyName}";
            await this.LoadConsultingsDataAsync();
            await this.LoadConsultingTypesAsync();
            
            sex = await dataSourceService.GetKeyValueByIdAsync(this.model.IdSex);

            nationality = await dataSourceService.GetKeyValueByIdAsync(this.model.IdNationality);

         

            this.isVisible = true;
            this.StateHasChanged();
            
        }
        private async Task LoadConsultingTypesAsync()
        {
            var candidateProviderConsulting = await this.CandidateProviderService.GetAllCandidateProviderConsultingsByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider);
            var kvConsultingTypes = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ConsultingType")).ToList();
            if (candidateProviderConsulting.Any())
            {
                foreach (var consulting in candidateProviderConsulting)
                {
                    if (!this.kvAllConsultingTypeSource.Any(c => c.IdKeyValue == consulting.IdConsultingType))
                    {
                        this.kvAllConsultingTypeSource.Add(kvConsultingTypes.FirstOrDefault(x => x.IdKeyValue == consulting.IdConsultingType));
                    }
                }
            }
        }
        private async Task AddConsultingTypeBtn()
        {
            if (this.idConsultingType == 0)
            {
                await this.ShowErrorAsync("Моля, изберете вид на услугата!");
                return;
            }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var candidateProviderConsultingVM = new ConsultingVM()
                {
                    IdConsultingClient = this.ConsultingClientVM.IdConsultingClient,
                    IdConsultingType = this.idConsultingType
                };

                var result = await this.CandidateProviderService.CreateConsultingAsync(candidateProviderConsultingVM);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                }
                else
                {
                    this.idConsultingType = 0;

                    await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                    await this.LoadConsultingsDataAsync();
                    await this.CheckForAlreadyAddedConsultingTypesAsync();

                    this.StateHasChanged();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        //public async Task downloadDocuments()
        //{
        //    if (loading) return;

        //    try
        //    {
        //        loading = true;
        //        List<CourseDocumentUploadedFileVM> docs = trainingService.getCourseDocumentUploadedFile(model.IdClientCourseDocument);

        //        foreach (var doc in docs)
        //        {
        //            try
        //            {
        //                var memoryStream = await uploadFileService.GetUploadedFile<CourseDocumentUploadedFile>(doc.IdCourseDocumentUploadedFile);

        //                await FileUtils.SaveAs(JsRuntime, $"Certificate_{model.DocumentRegNo}.pdf", memoryStream.ToArray());
        //            }
        //            catch (Exception e)
        //            {
        //                await this.ShowErrorAsync("Няма прикачени файлове");
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        loading = false;
        //    }
        //}
        private async Task LoadConsultingsDataAsync()
        {
            if (this.ConsultingClientVM != null)
            {
                this.consultingsSource =
                    await this.CandidateProviderService.GetAllConsultingsByIdConsultingClientAsync(
                        this.ConsultingClientVM.IdConsultingClient);
            }
        }
        private async Task CheckForAlreadyAddedConsultingTypesAsync(bool IsDelete = false)
        {
            if (this.consultingsSource.Any())
            {
                this.kvConsultingTypeSource = this.kvAllConsultingTypeSource;

                foreach (var consulting in this.consultingsSource)
                {
                    this.kvConsultingTypeSource = this.kvConsultingTypeSource.Where(x => x.IdKeyValue != consulting.IdConsultingType).ToList();

                }
            }
            else
            {
                this.kvConsultingTypeSource = this.kvAllConsultingTypeSource;
            }

            this.StateHasChanged();
        }
        private async Task DeleteConsultingTypeBtn(ConsultingVM model)
        {

            this.consultingToDelete = model;
            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
            if (isConfirmed)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;
                    var result = await this.CandidateProviderService.DeleteConsultingByIdAsync(this.consultingToDelete.IdConsulting);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                        await this.LoadConsultingsDataAsync();
                        await this.CheckForAlreadyAddedConsultingTypesAsync(true);

                        this.StateHasChanged();
                    }
                }
            
                finally
                {
                this.loading = false;
                }

                 this.SpinnerHide();
            }
        }
    }


}

