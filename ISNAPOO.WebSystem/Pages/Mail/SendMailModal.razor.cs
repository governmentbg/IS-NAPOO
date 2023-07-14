using System;
using System.Net.Mail;
using System.Net.Mime;
using Data.Models;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Mail;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;
using Syncfusion.DocIO.DLS;

namespace ISNAPOO.WebSystem.Pages.Mail
{
    public partial class SendMailModal : BlazorBaseComponent
    {

        private SendMailModel sendMailModel = new SendMailModel(); 

        [Inject]
        IJSRuntime JsRuntime { get; set; }
        [Inject]
        IUploadFileService uploadService { get; set; }

        [Parameter]
        public EventCallback<MailMessage> CallbackAfterSubmit { get; set; }

        bool HasUploadedFile = false;

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(sendMailModel);
        }

        private async Task SendMails()
        {
            var valid = this.editContext.Validate();
          if(valid)
            {
                MailMessage mail = new MailMessage();

                mail.Subject = sendMailModel.Title;
                mail.Body = sendMailModel.body;
                mail.IsBodyHtml = true;
                if (sendMailModel.binaryFile != null)
                {
                    Attachment attachment = new Attachment(sendMailModel.binaryFile, sendMailModel.FileName);
                    mail.Attachments.Add(attachment);
                }
                this.isVisible = false;
                await this.CallbackAfterSubmit.InvokeAsync(mail);
            }
        }

        private async Task OnChange(UploadChangeEventArgs args)
        {
            HasUploadedFile = true;

                if (args.Files.Count == 1)
                {   
                   sendMailModel.FileName = args.Files[0].FileInfo.Name;

                    sendMailModel.binaryFile = new MemoryStream(args.Files.First().Stream.ToArray());
                }
            
          
        }

        public async Task openModal()
        {
            this.isVisible = true;
            sendMailModel = new SendMailModel();
            this.editContext = new EditContext(sendMailModel);
            this.editContext.EnableDataAnnotationsValidation();
            this.StateHasChanged();
        }

        private async void OnDownloadClick()
        {
            bool hasPermission = await CheckUserActionPermission("ViewOrderData", false);
            if (!hasPermission) { return; }

                await FileUtils.SaveAs(this.JsRuntime, sendMailModel.FileName, sendMailModel.binaryFile.ToArray());
        }

        //private async Task OnRemove(RemovingEventArgs args)
        //{

        //}

        private async Task OnRemoveClick()
        {
            bool hasPermission = await CheckUserActionPermission("ManageOrderData", false);
            if (!hasPermission) { return; }

         
                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?");

                if (isConfirmed)
                {
                HasUploadedFile = false;
                sendMailModel = new SendMailModel();
                }
            }
        

    }
}

