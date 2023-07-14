using Data.Models.Data.SPPOO;
using DocuServiceReference;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.RichTextEditor;

namespace ISNAPOO.WebSystem.Pages.DOC.ERU
{
    partial class ERUDataModal : BlazorBaseComponent
    {
        [Inject]
        IJSRuntime JsRuntime { get; set; }
        [Inject]
        IDOCService docService { get; set; }
        [Inject]
        IDataSourceService DataSourceService { get; set; }
        [Inject]
        ISpecialityService specService { get; set; }
        [Inject]
        IApplicationUserService ApplicationUserService { get; set; }


        SfRichTextEditor sfRichTextEditor = new SfRichTextEditor();
        SfGrid<SpecialityVM> specialityGrid = new SfGrid<SpecialityVM>();
        List<SpecialityVM> addedSpecialitys = new List<SpecialityVM>();

        IEnumerable<KeyValueVM> professionalTraining;
        IEnumerable<KeyValueVM> nkrValue;
        IEnumerable<KeyValueVM> ekrValue;
        IEnumerable<SpecialityVM> specialitySource;

        public string CreationDateStr { get; set; } = "";
        public string ModifyDateStr { get; set; } = "";

        public override bool IsContextModified => this.editContext.IsModified();

        SpecialityVM specialityTodelete = new SpecialityVM();
        ERUVM model = new ERUVM();
        List<string> validationMessages = new List<string>();
        private int SpecialityValue = GlobalConstants.INVALID_ID;
        protected DialogEffect AnimationEffect = DialogEffect.Zoom;
        bool IsReadOnly = false;
        bool IsEnable = true;
        private bool IsNKR = false;

        [Parameter]
        public EventCallback OnSubmitHandler { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.FormTitle = "ЕРУ";
            this.professionalTraining = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(SPPOOValues.ProfessionalTraining);
            this.nkrValue = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(SPPOOValues.NKRLevel)).OrderBy(x => x.Order).ToList();
            this.ekrValue = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(SPPOOValues.EKRLevel)).OrderBy(x => x.Order).ToList(); 
        }

        public async void Open(ERUVM model, string permission = "Edit")
        {
            validationMessages.Clear();

            this.model = model;

            await SetCreateAndModifyDate();

            this.specialitySource = await this.specService.GetSpecialitiesByDocIdAsync(model.IdDOC);
            if (model.IdERU != 0)
            {
                this.addedSpecialitys = await this.specService.GetSpecialitiesByERUIdAsync(model.IdERU);
            }
            else
            {
                this.addedSpecialitys = new List<SpecialityVM>();
            }
            this.editContext = new EditContext(this.model);
            CheckPermission(permission);
            this.isVisible = true;


            this.StateHasChanged();
        }

        private async Task CheckPermission(string permission)
        {
            bool hasPermission = await HasClaim("ManageDOCData");
            if (!hasPermission || permission == "View")
            {
                IsReadOnly = true;
                IsEnable = false;
                return;
            }
            else
            {
                IsReadOnly = false;
                IsEnable = true;
            }
        }

        private async Task SetCreateAndModifyDate()
        {
            if (this.model.IdERU == 0)
            {
                this.CreationDateStr = "";
                this.ModifyDateStr = "";
                this.model.ModifyPersonName = "";
                this.model.CreatePersonName = "";
            }
            else
            {
                this.CreationDateStr = this.model.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.model.ModifyDate.ToString("dd.MM.yyyy");
                this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);
            }
        }

        private void ChangeBoth()
        {
            if (this.IsNKR)
            {
                this.IsNKR = false;
                if (model.IdNKRLevel != 0)
                {
                    string nkrKeyValueIntCode = nkrValue.FirstOrDefault(n => n.IdKeyValue == model.IdNKRLevel).KeyValueIntCode;
                    model.IdEKRLevel = ekrValue.FirstOrDefault(e => e.KeyValueIntCode == nkrKeyValueIntCode).IdKeyValue;
                }
                else
                {
                    model.IdEKRLevel = 0;
                }
            }
            else
            {
                if (model.IdEKRLevel != 0)
                {
                    string ekrKeyValueIntCode = ekrValue.FirstOrDefault(n => n.IdKeyValue == model.IdEKRLevel).KeyValueIntCode;
                    model.IdNKRLevel = nkrValue.FirstOrDefault(e => e.KeyValueIntCode == ekrKeyValueIntCode).IdKeyValue;
                }
                else
                {
                    model.IdNKRLevel = 0;
                }
            }

        }

        private async Task SubmitHandler()
        {
            validationMessages.Clear();
            if (model.RUText == "<p><br></p>")
            {
                model.RUText = "";
            }
            bool hasPermission = await CheckUserActionPermission("ManageDOCData", false);
            if (!hasPermission) { return; }

            this.editContext = new EditContext(this.model);

            this.editContext.EnableDataAnnotationsValidation();
            this.SpinnerShow();

            string msg = string.Empty;
            bool isValid = this.editContext.Validate();
            validationMessages.AddRange(this.GetValidationMessages());

            if (isValid)
            {
                if (this.addedSpecialitys.Count > 0)
                {
                    this.addedSpecialitys.ForEach((spec) =>
                    {
                        if (!this.model.Specialities.Contains(spec))
                        {
                            this.model.Specialities.Add(spec);
                        }
                    });
                }

                if (this.model.IdERU == GlobalConstants.INVALID_ID_ZERO)
                {
                    msg = await this.docService.CreateERUAsync(this.model);
                }
                else
                {
                    msg = await this.docService.UpdateERUAsync(this.model);
                }

                if (msg.Contains("успешeн"))
                {
                    await this.ShowSuccessAsync(msg);
                }
                else
                {
                    await this.ShowErrorAsync(msg);
                }

                await SetCreateAndModifyDate();

                var test = string.Empty;
            }
            await this.OnSubmitHandler.InvokeAsync();
            this.SpinnerHide();
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
                    await this.ShowErrorAsync("Тази специалност е вече добавена!");
                }
            }
            else
            {
                await this.ShowErrorAsync("Моля, изберете специалност");
            }
        }

        private async Task DeleteRowSpeciality(SpecialityVM specialityVM)
        {
            bool hasPermission = await CheckUserActionPermission("ManageDOCData", false);
            if (!hasPermission) { return; }

            this.specialityTodelete = specialityVM;

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {
                var result = await this.docService.DelteSpecialityFromERUById(specialityVM.IdSpeciality, model.IdERU);
                if (result > 0)
                {
                    await this.ShowSuccessAsync("Записът е изтрит успешно!");
                }

                this.model.ERUSpecialities.RemoveAll(x => x.IdSpeciality == specialityVM.IdSpeciality);
                this.model.Specialities.Remove(specialityVM);
                this.addedSpecialitys.Remove(specialityVM);

                this.specialityGrid.Refresh();
            }

        }

        private void DialogOpen()
        {
            this.sfRichTextEditor.RefreshUIAsync();
        }

        private List<ToolbarItemModel> Tools = new List<ToolbarItemModel>()
    {
        new ToolbarItemModel() { Command = ToolbarCommand.Bold },
        new ToolbarItemModel() { Command = ToolbarCommand.Italic },
        new ToolbarItemModel() { Command = ToolbarCommand.Underline },
        new ToolbarItemModel() { Command = ToolbarCommand.StrikeThrough },
        new ToolbarItemModel() { Command = ToolbarCommand.FontName },
        new ToolbarItemModel() { Command = ToolbarCommand.FontSize },
        new ToolbarItemModel() { Command = ToolbarCommand.FontColor },
        new ToolbarItemModel() { Command = ToolbarCommand.BackgroundColor },
        new ToolbarItemModel() { Command = ToolbarCommand.LowerCase },
        new ToolbarItemModel() { Command = ToolbarCommand.UpperCase },
        new ToolbarItemModel() { Command = ToolbarCommand.SuperScript },
        new ToolbarItemModel() { Command = ToolbarCommand.SubScript },
        new ToolbarItemModel() { Command = ToolbarCommand.Separator },
        new ToolbarItemModel() { Command = ToolbarCommand.Formats },
        new ToolbarItemModel() { Command = ToolbarCommand.Alignments },
        new ToolbarItemModel() { Command = ToolbarCommand.NumberFormatList },
        new ToolbarItemModel() { Command = ToolbarCommand.BulletFormatList },
        new ToolbarItemModel() { Command = ToolbarCommand.Outdent },
        new ToolbarItemModel() { Command = ToolbarCommand.Indent },
        new ToolbarItemModel() { Command = ToolbarCommand.Undo },
        new ToolbarItemModel() { Command = ToolbarCommand.Redo }
    };



    }
}
