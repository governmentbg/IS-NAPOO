using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.WebSystem.Pages.Test;
using Syncfusion.Blazor.Spinner;
using global::Data.Models.Data.DOC;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.AspNetCore.Components.Forms;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.Core.ViewModels.DOC.NKPD;
using Syncfusion.Blazor.Grids;
using ISNAPOO.WebSystem.Pages.DOC.NKPD;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Inputs;
using ISNAPOO.Core.HelperClasses;
using Syncfusion.Blazor.DropDowns;
using Microsoft.JSInterop;

namespace ISNAPOO.WebSystem.Pages.DOC
{
    public partial class DOCDataModal : BlazorBaseComponent
    {
        private string dialogClass = "";

        private int SpecialityValue = GlobalConstants.INVALID_ID;

        private bool isActiveStatus = false;
        private KeyValueVM kvActiveStatus;
        private KeyValueVM kvDraftStatus;
        private KeyValueVM kvInactiveStatus;

        private int firstStatusId;
        private string UploadeteFileName;

        SpecialityVM specialityTodelete = new SpecialityVM();

        int idNKPDTodelete = GlobalConstants.INVALID_ID;

        ValidationMessageStore messageStore;

        ToastMsg toast;
        SfGrid<NKPDVM> sfGridNKPD = new SfGrid<NKPDVM>();
        IEnumerable<NKPDVM> nKPDVMList = new List<NKPDVM>();
        NkpdSelectorModal nkpdSelectorModal = new NkpdSelectorModal();
        List<int> rowsIds = new List<int>();
        List<NKPDVM> nkpds = new List<NKPDVM>();

        SfGrid<SpecialityVM> specialityGrid = new SfGrid<SpecialityVM>();
        List<SpecialityVM> addedSpecialitys = new List<SpecialityVM>();
        private IEnumerable<KeyValueVM> statusValues = new List<KeyValueVM>();


        //DocVM model = new DocVM();
        ProfessionVM professionFilterVM = new ProfessionVM();
        SpecialityVM specialityFilterVM = new SpecialityVM();
        IEnumerable<ProfessionVM> professionSource;
        IEnumerable<ProfessionVM> originalProfessionSource;
        IEnumerable<SpecialityVM> specialitySource;
        public bool IsDateValid { get; set; } = true;

        //За филтър по 2 полета ти трябва тези два реда и
        // да се добави референция на ComboBoxProf и Query в комбобокса
        //public Query Query = new Query();
        //SfComboBox<int, ProfessionVM> ComboBoxProf;


        [Parameter]
        public EventCallback<DocVM> CallbackAfterSave { get; set; }

        [Parameter]
        public EventCallback<IEnumerable<string>> ValidationWhenSave { get; set; }

        [Parameter]
        public DocVM DocVM { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.addedSpecialitys.Clear();

            this.FormTitle = "ДОС";
            editContext = new EditContext(this.DocVM);
            this.statusValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("StatusSPPOO", this.DocVM.IdStatus);

            if (this.DocVM.EditButtonClick == 1)
            {
                this.DocVM.EditButtonClick = 0;
            }

            this.kvActiveStatus = this.statusValues.FirstOrDefault(x => x.KeyValueIntCode == "Active");
            this.kvDraftStatus = this.statusValues.FirstOrDefault(x => x.KeyValueIntCode == "Draft");
            this.kvInactiveStatus = this.statusValues.FirstOrDefault(x => x.KeyValueIntCode == "Inactive");

            //Записваме си на какъв статус е договора като го отворим за може да проверим когато записваме
            //да не може да се върне от Активен на Работен #16889
            this.firstStatusId = this.DocVM.IdStatus;

            this.rowsIds = new List<int>();
            this.nkpds = new List<NKPDVM>();
            this.nKPDVMList = new List<NKPDVM>();

            IsDocActive();

            this.professionFilterVM = new ProfessionVM();
            this.specialityFilterVM = new SpecialityVM();

            if (!this.isActiveStatus)
            {
                this.specialityFilterVM.IdStatus = this.kvDraftStatus.IdKeyValue;
                this.professionFilterVM.IdStatus = this.kvDraftStatus.IdKeyValue;
            }
            else
            {
                this.specialityFilterVM.IdStatus = 0;
                this.professionFilterVM.IdStatus = 0;
            }

            this.professionFilterVM.IdProfession = DocVM.IdProfession;
            this.professionSource = this.dataSourceService.GetAllProfessionsList().Where(x => x.IdStatus != this.kvInactiveStatus.IdKeyValue).OrderBy(x => x.Code).ToList();
            this.originalProfessionSource = this.professionSource.ToList();

            this.specialityFilterVM.IdProfession = this.DocVM.IdProfession;
            //Филтрираме само тези коитоса са на статус работен ако договора е на работен
            this.specialitySource = await this.specService.GetAllSpecialitiesAsync(specialityFilterVM);
            this.specialitySource = this.specialitySource.Where(x => x.IdStatus == this.DocVM.IdStatus).ToList();
            //Зареждане на специалностите
            this.addedSpecialitys = await this.specService.GetSpecialitiesByDocIdAsync(DocVM.IdDOC);


            if (this.DocVM.DOCNKPDs.Count != 0)
            {
                List<int> idsNKPDs = new List<int>();

                foreach (var docNKPD in this.DocVM.DOCNKPDs)
                {
                    idsNKPDs.Add(docNKPD.IdNKPD);
                }

                this.rowsIds.AddRange(idsNKPDs);
                IEnumerable<NKPDVM> nKPDVMs = await this.NKPDService.GetNKPDsByIdsAsync(idsNKPDs);
                this.nkpds = new List<NKPDVM>();

                foreach (var nkpdVM in nKPDVMs)
                {
                    this.nkpds.Add(nkpdVM);
                }

                this.nKPDVMList = this.nkpds;
            }

            this.StateHasChanged();
        }

        private void OnDOCStatusChange(ChangeEventArgs<int, KeyValueVM> args)
        {
            if (args is not null && args.ItemData is not null)
            {
                if (this.DocVM.IdProfession != 0)
                {
                    this.specialitySource = this.dataSourceService.GetAllSpecialitiesList().Where(x => x.IdProfession == this.DocVM.IdProfession && x.IdStatus == args.ItemData.IdKeyValue).ToList();
                }
                else
                {
                    this.SpecialityValue = GlobalConstants.INVALID_ID;
                    this.specialitySource = new List<SpecialityVM>();
                }
            }
            else
            {
                this.SpecialityValue = GlobalConstants.INVALID_ID;
                this.specialitySource = new List<SpecialityVM>();
            }
        }

        private async Task Save()
        {

            bool hasPermission = await CheckUserActionPermission("ManageDOCData", false);
            if (!hasPermission) { return; }


            editContext = new EditContext(this.DocVM);
            this.editContext.EnableDataAnnotationsValidation();
            string msg = string.Empty;

            bool isValid = this.editContext.Validate();
            this.messageStore = new ValidationMessageStore(this.editContext);
            await ValidationWhenSave.InvokeAsync(this.GetValidationMessages());
            if (IsDateValid)
            {
                if (isValid)
                {
                    this.SpinnerShow();

                    //Проверка да не може да се върне от Активен на Работен #16889
                    if (this.firstStatusId == kvActiveStatus.IdKeyValue && this.DocVM.IdStatus == kvDraftStatus.IdKeyValue)
                    {
                        await this.ShowErrorAsync("Не може да променяте статуса на ДОС от Активен на Работен!");
                        return;
                    }

                    //Проверка дали има други активни със същата професия
                    var haveActiveWithProf = await this.docService.CheckForActiveDocWithSameProfession(this.DocVM);
                    if (haveActiveWithProf)
                    {
                        await this.ShowErrorAsync("Не може да запишете два пъти ДОС на статус Активен за една и съща професия!");
                        return;
                    }


                    if (this.addedSpecialitys.Count > 0)
                    {
                        this.addedSpecialitys.ForEach((spec) =>
                        {
                            if (!this.DocVM.Specialities.Contains(spec))
                            {
                                this.DocVM.Specialities.Add(spec);
                            }
                        });

                        //Проверка дали има други активни със някоя от специалностите
                        var haveActiveWithSpec = await this.docService.CheckForActiveDocWithSameSpeciality(this.DocVM);
                        if (haveActiveWithSpec)
                        {
                            await this.ShowErrorAsync("В базата вече има Активен ДОС с същата специалност!");
                            return;
                        }
                    }

                    if (this.nKPDVMList.Count() > 0)
                    {
                        foreach (var nkpd in this.nKPDVMList)
                        {
                            if (!this.DocVM.IdsNkpd.Contains(nkpd.IdNKPD))
                            {
                                this.DocVM.IdsNkpd.Add(nkpd.IdNKPD);
                            }
                        }
                    }

                    if (this.DocVM.IdDOC == GlobalConstants.INVALID_ID_ZERO)
                    {
                        msg = await this.docService.CreateDOCAsync(this.DocVM);
                    }
                    else
                    {
                        if (this.UploadeteFileName != null)
                        {
                            this.DocVM.UploadedFileName = this.UploadeteFileName;
                        }
                        msg = await this.docService.UpdateDOCAsync(this.DocVM);
                    }
                    var currentDoc = await this.docService.GetDOCByIdAsync(new DocVM() { IdDOC = this.DocVM.IdDOC });
                    this.DocVM = currentDoc;
                    IsDocActive();
                    await this.ShowSuccessAsync(msg);
                    await CallbackAfterSave.InvokeAsync(this.DocVM);
                }
            }
            else
            {
                toast.sfErrorToast.Content = "Въведената дата в полето 'В сила от' не може да е след 'В сила до'!";
                await toast.sfErrorToast.ShowAsync();
            }
        }

        private void HandleSpecialitiesSourceData(BeforeOpenEventArgs args)
        {
            var activeSpecialities = this.dataSourceService.GetAllSpecialitiesList().Where(x => x.IdProfession == this.DocVM.IdProfession && x.IdStatus == this.dataSourceService.GetActiveStatusID()).ToList();
            foreach (var speciality in this.addedSpecialitys)
            {
                activeSpecialities.RemoveAll(x => x.IdSpeciality == speciality.IdSpeciality);
            }

            this.specialitySource = activeSpecialities.ToList();
        }

        public async Task<EditContext> ReturnEditContext()
        {
            return editContext;
        }

        private void ClearProfession(ChangeEventArgs<int, ProfessionVM> args)
        {
            if (args is not null && args.ItemData is not null)
            {
                if (this.DocVM.IdStatus != 0)
                {
                    this.specialitySource = this.dataSourceService.GetAllSpecialitiesList().Where(x => x.IdProfession == args.ItemData.IdProfession && x.IdStatus == this.DocVM.IdStatus).ToList();
                }
                else
                {
                    this.SpecialityValue = GlobalConstants.INVALID_ID;
                    this.specialitySource = new List<SpecialityVM>();
                }
            }
            else
            {
                this.SpecialityValue = GlobalConstants.INVALID_ID;
                this.specialitySource = new List<SpecialityVM>();
            }

            if (this.DocVM.IdProfession == 0)
            {
                professionFilterVM.ProfessionComboFilter = string.Empty;
                this.professionSource = this.originalProfessionSource.ToList();
            }
        }

        private void FilteringSpeciality(FilteringEventArgs args)
        {
            //Използваме кода да вземем от базата филтрираните редове
            this.specialityFilterVM.SpecialityComboFilter = args.Text;

            //Филтрираме само тези коитоса са на статус работен ако договора е на работен
            if (!this.isActiveStatus)
            {
                this.specialityFilterVM.IdStatus = this.kvDraftStatus.IdKeyValue;
            }
            else
            {
                this.specialityFilterVM.IdStatus = 0;
            }

            this.specialitySource = new List<SpecialityVM>();
            this.specialitySource = this.specService.GetAllSpecialities(specialityFilterVM);
            this.StateHasChanged();

        }

        private void FilterSpeciality(SelectEventArgs<ProfessionVM> args)
        {
            this.SpecialityValue = GlobalConstants.INVALID_ID;
            this.specialityFilterVM.IdProfession = args.ItemData.IdProfession;
            FilteringSpeciality(new FilteringEventArgs());
        }

        private async Task OnAddSpecialityClick()
        {
            bool hasPermission = await CheckUserActionPermission("ManageDOCData", false);
            if (!hasPermission) { return; }
            if (this.SpecialityValue > 0)
            {
                if (!this.addedSpecialitys.Any(x => x.IdSpeciality == this.SpecialityValue))
                {
                    SpecialityVM addedSpeciality = await this.specService.GetSpecialityByIdAsync(this.SpecialityValue);
                    this.addedSpecialitys.Add(addedSpeciality);


                    this.specialityGrid.Refresh();
                    this.SpecialityValue = GlobalConstants.INVALID_ID;
                }
                else
                {
                    toast.sfErrorToast.Content = "Тази специалност е вече добавена!";
                    await toast.sfErrorToast.ShowAsync();
                }
            }
            else
            {
                await this.ShowErrorAsync("Моля, изберете специалност");
            }
        }

        private async Task DeleteRowSpeciality(SpecialityVM specialityVM)
        {

            this.specialityTodelete = specialityVM;

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {

                var result = await this.docService.RemoveDocFromSpecialityById(specialityVM.IdSpeciality);
                if (result > 0)
                {

                    this.DocVM.Specialities.Remove(specialityVM);
                    this.toast.sfSuccessToast.Content = "Записът е изтрит успешно!";
                    await toast.sfSuccessToast.ShowAsync();

                }

                this.addedSpecialitys.Remove(specialityVM);
                this.specialityGrid.Refresh();
                await CallbackAfterSave.InvokeAsync(this.DocVM);
            }

        }

        #region UploadetFiles

        private async Task OnChange(UploadChangeEventArgs args)
        {
            bool hasPermission = await CheckUserActionPermission("ManageDOCData", false);
            if (!hasPermission) { return; }

            if (this.DocVM.IdDOC > 0)
            {
                if (args.Files.Count == 1)
                {
                    bool isConfirmed = true;

                    var resCheck = await this.uploadService.CheckIfExistUploadedFileAsync<Data.Models.Data.DOC.DOC>(this.DocVM.IdDOC);
                    if (resCheck)
                    {
                        isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "За избрания ДОС вече има прикачен файл. Искате ли да го подмените?");
                    }

                    if (isConfirmed)
                    {
                        var fileName = args.Files[0].FileInfo.Name;

                        var result = await this.uploadService.UploadFileAsync<Data.Models.Data.DOC.DOC>(args.Files[0].Stream, fileName, "DOC", this.DocVM.IdDOC);

                        if (!string.IsNullOrEmpty(result))
                        {
                            this.UploadeteFileName = result;

                            this.StateHasChanged();
                        }
                    }

                    editContext = new EditContext(this.DocVM);
                }
            }
        }

        private async Task OnRemove(RemovingEventArgs args)
        {
            if (args.FilesData.Count == 1)
            {
                if (this.DocVM.IdDOC > 0)
                {
                    bool isConfirmed = true;
                    if (args.FilesData[0].Name == this.DocVM.FileName)
                    {
                        isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?");
                    }

                    if (isConfirmed)
                    {
                        var result = await this.uploadService.RemoveFileByIdAsync<Data.Models.Data.DOC.DOC>(this.DocVM.IdDOC);

                        if (result == 1)
                        {
                            this.DocVM = await this.docService.GetDOCByIdAsync(this.DocVM);

                            this.StateHasChanged();
                        }
                    }
                }
            }
        }

        private async Task OnDownloadClick()
        {
            bool hasPermission = await CheckUserActionPermission("ViewDOCData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var hasFile = await this.uploadService.CheckIfExistUploadedFileAsync<Data.Models.Data.DOC.DOC>(this.DocVM.IdDOC);
                if (hasFile)
                {
                    var documentStream = await this.uploadService.GetUploadedFileAsync<Data.Models.Data.DOC.DOC>(this.DocVM.IdDOC);

                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, this.DocVM.FileName, documentStream.MS!.ToArray());
                    }
                }
                else
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                    await this.ShowErrorAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnRemoveClick()
        {
            bool hasPermission = await CheckUserActionPermission("ManageDOCData", false);
            if (!hasPermission) { return; }

            if (this.DocVM.IdDOC > 0)
            {
                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?");
                if (isConfirmed)
                {
                    var result = await this.uploadService.RemoveFileByIdAsync<Data.Models.Data.DOC.DOC>(this.DocVM.IdDOC);

                    if (result == 1)
                    {
                        this.DocVM = await this.docService.GetDOCByIdAsync(this.DocVM);

                        this.StateHasChanged();
                    }
                }
            }
        }

        #endregion

        private async Task DeleteRowNkpd(int idNkpd)
        {
            bool hasPermission = await CheckUserActionPermission("ManageDOCData", false);
            if (!hasPermission) { return; }

            this.idNKPDTodelete = idNkpd;

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {
                var result = await this.docService.DelteNKPDFromDocById(idNkpd, this.DocVM.IdDOC);
                if (result > 0)
                {
                    this.toast.sfSuccessToast.Content = "Записът е изтрит успешно!";
                    await toast.sfSuccessToast.ShowAsync();
                }

                this.rowsIds.Remove(idNkpd);
                this.nKPDVMList = await this.NKPDService.GetNKPDsByIdsAsync(this.rowsIds);
                this.sfGridNKPD.Refresh();
            }

        }
        private void IsEndDateValid()
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime(); ;
            if (this.DocVM.StartDate.HasValue)
            {
                startDate = this.DocVM.StartDate.Value.Date;
            }
            if (this.DocVM.EndDate.HasValue)
            {
                endDate = this.DocVM.EndDate.Value.Date;

            }
            int result = DateTime.Compare(startDate, endDate);

            if (result > 0 && this.DocVM.EndDate.HasValue && this.DocVM.StartDate.HasValue)
            {
                IsDateValid = false;
            }
            else
            {
                IsDateValid = true;
            }
        }

        private async Task OpenNkpdSelectorModal()
        {
            bool hasPermission = await CheckUserActionPermission("ManageDOCData", false);
            if (!hasPermission) { return; }

            await this.nkpdSelectorModal.Open();
        }

        private async Task NKPDOnSelectHandler(List<int> rowsIds)
        {
            rowsIds.ForEach((nkpdId) =>
            {
                if (!this.rowsIds.Contains(nkpdId))
                {
                    this.rowsIds.Add(nkpdId);
                }
            });

            this.nKPDVMList = await this.NKPDService.GetNKPDsByIdsAsync(this.rowsIds);
        }

        private void IsDocActive()
        {
            if (this.DocVM.IdStatus == this.kvActiveStatus.IdKeyValue)
            {
                this.isActiveStatus = true;
            }
            else
            {
                this.isActiveStatus = false;
            }
        }

        private async Task ChangeStatus(SelectEventArgs<KeyValueVM> args)
        {

            if (args.ItemData.IdKeyValue == this.kvActiveStatus.IdKeyValue)
            {
                this.isActiveStatus = true;
            }
            else if (args.ItemData.IdKeyValue == this.kvDraftStatus.IdKeyValue)
            {
                this.isActiveStatus = false;
            }
            else
            {
                this.isActiveStatus = false;
            }

            this.professionFilterVM = new ProfessionVM();

            if (!this.isActiveStatus && args.ItemData.IdKeyValue == this.kvInactiveStatus.IdKeyValue)
            {
                this.professionFilterVM.IdStatus = this.kvInactiveStatus.IdKeyValue;
            }
            else if (!this.isActiveStatus && args.ItemData.IdKeyValue == this.kvDraftStatus.IdKeyValue)
            {
                this.professionFilterVM.IdStatus = this.kvDraftStatus.IdKeyValue;
            }
            else
            {
                this.professionFilterVM.IdStatus = this.kvActiveStatus.IdKeyValue;
            }

            this.professionSource = await this.profService.GetAllAsync(professionFilterVM);


        }

    }
}
