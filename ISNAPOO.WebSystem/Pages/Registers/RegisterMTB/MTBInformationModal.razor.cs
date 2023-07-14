using System.ComponentModel.DataAnnotations;

using Data.Models.Data.Candidate;

using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.Register;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;

using Microsoft.AspNetCore.Components;

using Microsoft.JSInterop;

using Syncfusion.Blazor.Grids;

using Syncfusion.Blazor.Popups;
using Syncfusion.DocIO.DLS;

namespace ISNAPOO.WebSystem.Pages.Registers.RegisterMTB
{
    public partial class MTBInformationModal : BlazorBaseComponent
    {
        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IProfessionalDirectionService ProfessionalDirectionService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        SfDialog sfDialog = new SfDialog();
        SfGrid<CandidateProviderPremisesDocumentVM> candidateProviderPremisesDocumentsGrid = new SfGrid<CandidateProviderPremisesDocumentVM>();
        SfGrid<CandidateProviderPremisesRoomVM> premisesRoomsGrid = new SfGrid<CandidateProviderPremisesRoomVM>();
        SfGrid<CandidateProviderPremisesDocumentVM> mtbDocumentsGrid = new SfGrid<CandidateProviderPremisesDocumentVM>();
        RegisterMTBModalVM registerPremisesModalVM = new RegisterMTBModalVM();
        List<CandidateProviderVM> candidateProvidersSource = new List<CandidateProviderVM>();
        CandidateProviderPremisesVM candidateProviderPremisesVM = new CandidateProviderPremisesVM();
        List<CandidateProviderPremisesDocumentVM> mtbDocumentsSource = new List<CandidateProviderPremisesDocumentVM>();
        List<CandidateProviderPremisesRoomVM> premisesRoomsSource = new List<CandidateProviderPremisesRoomVM>();
        KeyValueVM kvTheory;
        KeyValueVM kvPractice;
        KeyValueVM kvPracticeAndTheory;

        public bool IsAnyRooms { get; set; } = false;
        public bool IsAnyDocuments { get; set; } = false;
        protected override async Task OnInitializedAsync()
        {
            this.registerPremisesModalVM = new RegisterMTBModalVM();
            this.candidateProvidersSource = new List<CandidateProviderVM>();
            this.candidateProviderPremisesVM = new CandidateProviderPremisesVM();
            this.mtbDocumentsSource = new List<CandidateProviderPremisesDocumentVM>();
            this.premisesRoomsSource = new List<CandidateProviderPremisesRoomVM>();
            kvTheory = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TheoryTraining");
            kvPractice = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "PracticalTraining");
            kvPracticeAndTheory = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TrainingInTheoryAndPractice");
        }
        public async Task OpenModal(RegisterMTBVM model)
        {
            
            this.registerPremisesModalVM = new RegisterMTBModalVM();
            this.candidateProviderPremisesVM = new CandidateProviderPremisesVM();
            this.mtbDocumentsSource = new List<CandidateProviderPremisesDocumentVM>();
            this.premisesRoomsSource = new List<CandidateProviderPremisesRoomVM>();
            this.registerPremisesModalVM = LoadInformationForPremises(model);
            this.premisesRoomsSource = (await this.CandidateProviderService.GetAllCandidateProviderPremisesRoomsByCandidateProviderPremisesIdAsync(new CandidateProviderPremisesRoomVM() { IdCandidateProviderPremises = model.CandidateProviderPremises.IdCandidateProviderPremises })).ToList();
            this.mtbDocumentsSource = (await this.CandidateProviderService.GetAllCandidateProviderPremisesDocumentsByCandidateProviderPremisesIdAsync(new CandidateProviderPremisesDocumentVM() { IdCandidateProviderPremises = model.CandidateProviderPremises.IdCandidateProviderPremises })).ToList();

            if(this.premisesRoomsSource.Any())
                this.IsAnyRooms = true;
            else
                this.IsAnyRooms = false;

            if (this.mtbDocumentsSource.Any())
                this.IsAnyDocuments = true;
            else
                this.IsAnyDocuments = false;

            var kvDocumentType = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MTBDocumentType");
            foreach (var doc in mtbDocumentsSource)
            {
                doc.DocumentTypeName = kvDocumentType.FirstOrDefault(x => x.IdKeyValue == doc.IdDocumentType).Name;
            }
            foreach (var doc in candidateProviderPremisesVM.CandidateProviderPremisesDocuments)
            {
                doc.UploadedByName = this.ApplicationUserService.GetApplicationUsersPersonNameAsync(doc.IdModifyUser).Result;
            }
            registerPremisesModalVM.IdStatus = this.DataSourceService.GetKeyValueByIdAsync(candidateProviderPremisesVM.IdStatus).Result.Name;
            this.isVisible = true;
            this.StateHasChanged();
        }

        public RegisterMTBModalVM LoadInformationForPremises(RegisterMTBVM model)
        {
            candidateProviderPremisesVM = model.CandidateProviderPremises;
            candidateProviderPremisesVM.CandidateProvider = model.CandidateProvider;
            foreach (var property in candidateProviderPremisesVM.GetType().GetProperties())
            {
                if ((property.GetValue(candidateProviderPremisesVM) == null || property.GetValue(candidateProviderPremisesVM) == "") && property.ToString().Contains("String"))
                {
                    property.SetValue(candidateProviderPremisesVM, " ");
                }
            }
            var registerPremisesModalVM = new RegisterMTBModalVM()
            {


                IdCandidateProviderPremises = candidateProviderPremisesVM.IdCandidateProviderPremises,

                IdCandidate_Provider = candidateProviderPremisesVM.IdCandidate_Provider,

                CandidateProvider = candidateProviderPremisesVM.CandidateProvider,

                PremisesName = candidateProviderPremisesVM.PremisesName.ToString(),

                PremisesNote = candidateProviderPremisesVM.PremisesNote.ToString(),

                IdLocation = candidateProviderPremisesVM.IdLocation.ToString(),

                Location = candidateProviderPremisesVM.Location,

                ProviderAddress = candidateProviderPremisesVM.ProviderAddress.ToString(),

                ZipCode = candidateProviderPremisesVM.ZipCode.ToString(),

                Phone = candidateProviderPremisesVM.Phone.ToString(),

                IdOwnership = candidateProviderPremisesVM.IdOwnership.ToString(),

                IdStatus = candidateProviderPremisesVM.IdStatus.ToString(),

                StatusValue = candidateProviderPremisesVM.StatusValue.ToString(),
            };
            if (!String.IsNullOrEmpty(registerPremisesModalVM.IdStatus))
            {
                registerPremisesModalVM.IdStatus = this.DataSourceService.GetKeyValueByIdAsync(int.Parse(registerPremisesModalVM.IdStatus)).Result.Name;
            }
            else
            {
                registerPremisesModalVM.IdStatus = " ";
            }
            if (registerPremisesModalVM.Phone == "" || registerPremisesModalVM.Phone == null)
            {
                registerPremisesModalVM.Phone = " ";
            }
            if (!String.IsNullOrEmpty(registerPremisesModalVM.IdOwnership) && registerPremisesModalVM.IdOwnership != "0")
            {
                registerPremisesModalVM.IdOwnership = this.DataSourceService.GetKeyValueByIdAsync(int.Parse(registerPremisesModalVM.IdOwnership)).Result.Name;
            }
            else
            {
                registerPremisesModalVM.IdOwnership = " ";
            }
            foreach (var property in registerPremisesModalVM.GetType().GetProperties())
            {
                if ((property.GetValue(registerPremisesModalVM) == null || property.GetValue(registerPremisesModalVM) == "") && property.ToString().Contains("String"))
                {
                    property.SetValue(registerPremisesModalVM, " ");
                }
            }
            return registerPremisesModalVM;
        }
        private async Task OnDownloadClick(string fileName)
        {
            bool hasPermission = await CheckUserActionPermission("ViewApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                CandidateProviderPremisesDocumentVM document = this.mtbDocumentsSource.FirstOrDefault(x => x.FileName == fileName);

                if (document != null)
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CandidateProviderPremisesDocument>(document.IdCandidateProviderPremisesDocument);
                    if (hasFile)
                    {
                        var documentStream = await this.UploadFileService.GetUploadedFileAsync<CandidateProviderPremisesDocument>(document.IdCandidateProviderPremisesDocument);
                        if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, document.FileName, documentStream.MS!.ToArray());
                        }
                    }
                    else
                    {
                        await this.ShowErrorAsync(this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload"));
                    }
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

