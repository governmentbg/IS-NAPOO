using System.Security.Principal;
using System.Text.RegularExpressions;
using Data.Models.Data.Common;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Common
{
    public partial class UserProfile : BlazorBaseComponent
    {
        ToastMsg toast;

        [Inject]
        IPersonService personService { get; set; }
        [Inject]
        ICandidateProviderService candidateProviderService { get; set; }
        [Inject]
        NavigationManager navigationManager { get; set; }

        string path = string.Empty;
        ChangePasswordModel changePasswordModel;
        ChangeInformationModel changeInformationModel;
        ResultContext<ApplicationUser> resultContext = new ResultContext<ApplicationUser>();
        List<string> errorMessages = new List<string>();
        ApplicationUser user = new ApplicationUser();
        bool isPositionVisible = true;

        PersonVM person = new PersonVM();

        EditContext editContextPassword;

        CandidateProviderVM candidateProvider = new CandidateProviderVM();
        private ValidationMessageStore? messageStore;

        private string inputOldPasswordType = "password";
        private string inputNewFirstPasswordType = "password";
        private string inputNewSecondPasswordType = "password";
        private string oldPasswordIconClass = "fa fa-solid fa-eye password-icon";
        private string newFirstPasswordIconClass = "fa fa-solid fa-eye password-icon";
        private string newSecondPasswordIconClass = "fa fa-solid fa-eye password-icon";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            this.changePasswordModel = new ChangePasswordModel();

            this.changeInformationModel = new ChangeInformationModel();

            this.editContextPassword = new EditContext(this.changePasswordModel);
            this.editContextPassword.EnableDataAnnotationsValidation();

            this.editContext = new EditContext(this.changeInformationModel);
            this.editContext.EnableDataAnnotationsValidation();

            this.user = await UserManager.FindByIdAsync(this.UserProps.ID);

            person = await personService.GetPersonByIdWithIncludeAsync((int)user.IdPerson);

            this.changeInformationModel.Email = person.Email;


            if (person.CandidateProviderPersons.Count() > 0)
                candidateProvider = await candidateProviderService.GetCandidateProviderByIdAsync(new CandidateProviderVM
                {
                    IdCandidate_Provider = person.CandidateProviderPersons.First().IdCandidate_Provider
                });


            this.isPositionVisible = this.UserProps.IdCandidateProvider != 0;
            this.changeInformationModel.Title = person.Position;
            this.isVisible = true;

        }

        public override bool IsContextModified { get { return base.editContext.IsModified(); } }

        private async void OnInformationReset()
        {
            errorMessages.Clear();

            this.editContext.OnValidationRequested += this.ValidatePosition;
            this.messageStore = new ValidationMessageStore(this.editContext);
            bool isValid = this.editContext.Validate();

            Regex regex = new Regex("(?:[a-z0-9!#$%&'*+\\=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+\\=?^_`{|}~-]+)*|\"\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\"\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])");
            Match match = regex.Match(this.changeInformationModel.Email);

            if (isValid && match.Success)
            {
                this.SpinnerShow();
                ResultContext<PersonVM> resultContext = new ResultContext<PersonVM>();
                person.Email = changeInformationModel.Email;
                if (changeInformationModel.Title == "delete")
                {
                    person.Position = string.Empty;
                }
                else
                {
                    person.Position = changeInformationModel.Title;
                }

                user.Email = changeInformationModel.Email;

                user.NormalizedEmail = changeInformationModel.Email.ToUpper();

                resultContext.ResultContextObject = person;

                await personService.UpdatePersonAsync(resultContext);
                await UserManager.UpdateAsync(user);

                this.editContext = new EditContext(this.changeInformationModel);

                await this.ShowSuccessAsync("Записът е успешен!");

            }
            else
            {
                errorMessages = this.editContext.GetValidationMessages().ToList();
                if (!match.Success)
                {
                    this.errorMessages.Add("Въведеният E-mail е невалиден!");
                }
            }
        }

        private async void OnReset()
        {

            if (loading) return;

            try
            {
                loading = true;
                errorMessages.Clear();

                bool isValid = this.editContextPassword.Validate();
                if (isValid)
                {
                    if (this.user == null)
                    {
                        //TODO: Send confirmation
                    }
                    else
                    {
                        if (this.changePasswordModel.OldPassword.Equals(changePasswordModel.NewPassword))
                        {
                            errorMessages.Add("Въведената нова парола съвпада със старата парола!");

                        }
                        else
                        {
                            var result = await UserManager.ChangePasswordAsync(this.user, changePasswordModel.OldPassword, changePasswordModel.NewPassword);
                            if (result.Succeeded)
                            {
                                await this.ShowSuccessAsync("Вашата парола е променена успешно!");
                                this.user = await UserManager.FindByIdAsync(this.UserProps.ID);
                            }
                            else
                            {
                                errorMessages.AddRange(result.Errors.Select(e => e.Description));
                            }

                            changePasswordModel.OldPassword = String.Empty;
                            changePasswordModel.NewPassword = String.Empty;
                            changePasswordModel.ConfirmPassword = String.Empty;
                        }
                    }
                }
                else
                {
                    errorMessages = this.editContextPassword.GetValidationMessages().ToList();
                }

                //if (prevEmail != person.Email || prevTitle != person.Title)
                //{
                //    if (person.Title.Equals(""))
                //    {
                //        errorMessages.Add("Полето \'Длъжност\' е задължително!");
                //    }
                //    else
                //    {
                //        ResultContext<PersonVM> resultContext = new ResultContext<PersonVM>();
                //        resultContext.ResultContextObject = person;
                //        await personService.UpdatePersonAsync(resultContext);

                //        toast.sfSuccessToast.Content = "Записът е успешен.";
                //        toast.sfSuccessToast.ShowAsync();

                //        person = await personService.GetPersonByIdWithIncludeAsync((int)user.IdPerson);
                //    }
                //}
                this.StateHasChanged();
            }
            finally
            {
                loading = false;
            }
        }
        protected virtual void OnXClickHandler(BeforeCloseEventArgs args)
        {
            if (args.ClosedBy == "Close Icon")
            {
                args.Cancel = true;

                this.CancelClickedHandler();
                Redirect();
            }
        }
        private void Redirect()
        {
            navigationManager.NavigateTo("/");
        }
        private void ValidatePosition(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();
            if (string.IsNullOrEmpty(changeInformationModel.Title) && this.UserProps.IdCandidateProvider != 0)
            {
                FieldIdentifier fi = new FieldIdentifier(changeInformationModel, "Title");
                this.messageStore?.Add(fi, $"Полето 'Длъжност' е задължително!");
            }
        }

        private void ShowOldPassword()
        {
            if (this.inputOldPasswordType == "password")
            {
                this.inputOldPasswordType = "text";
                this.oldPasswordIconClass = "fa fa-solid fa-eye-slash password-icon";
            }
            else
            {
                this.inputOldPasswordType = "password";
                this.oldPasswordIconClass = "fa fa-solid fa-eye password-icon";
            }
        }

        private void ShowNewFirstPassword()
        {
            if (this.inputNewFirstPasswordType == "password")
            {
                this.inputNewFirstPasswordType = "text";
                this.newFirstPasswordIconClass = "fa fa-solid fa-eye-slash password-icon";
            }
            else
            {
                this.inputNewFirstPasswordType = "password";
                this.newFirstPasswordIconClass = "fa fa-solid fa-eye password-icon";
            }
        }

        private void ShowNewSecondPassword()
        {
            if (this.inputNewSecondPasswordType == "password")
            {
                this.inputNewSecondPasswordType = "text";
                this.newSecondPasswordIconClass = "fa fa-solid fa-eye-slash password-icon";
            }
            else
            {
                this.inputNewSecondPasswordType = "password";
                this.newSecondPasswordIconClass = "fa fa-solid fa-eye password-icon";
            }
        }
    }
}
