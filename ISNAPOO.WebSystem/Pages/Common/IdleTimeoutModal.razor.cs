using System;
using System.Timers;
using Blazored.LocalStorage;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Syncfusion.Blazor.Popups;
using static ISNAPOO.WebSystem.Extensions.BlazorCookieLoginMiddleware;
using Timer = System.Timers.Timer;

namespace ISNAPOO.WebSystem.Pages.Common
{

    public partial class IdleTimeoutModal
    {
        private SfDialog sfDialog = new SfDialog();
        bool isVisible { get; set; } = false;
        private Timer timer;

        [Inject]
        public NavigationManager navMgr { get; set; }
        [Inject]
        public ILocalStorageService localStorage { get; set; }

        bool stay = false;

        public async Task openModal()
        {
            this.isVisible = true;

            this.StateHasChanged();

            timer = new Timer(20000);
            timer.Elapsed += UpdateTimer;
            timer.AutoReset = false;

            timer.Start();
        }

        private void resetTimeout()
        {
            this.stay = true;
            isVisible = false;
        }

        private void UpdateTimer(Object source, ElapsedEventArgs e)
        {
            InvokeAsync(async () =>
            {
                timer.Stop();
                if (!stay)
                {
                   LogoutUser();
                }
            });
        }

        private async void LogoutUser()
        {

            try
            {
                var key = await localStorage.GetItemAsync<string>("LOCAL_LOGIN_KEY");

                if (!string.IsNullOrEmpty(key)) {

                    var logOutUser = BlazorCookieLoginMiddleware.OnlineUsers.FirstOrDefault(c => c.Key == key.Replace("\"", ""));
                    if (logOutUser.Value != null)
                    {
                        logOutUser.Value.LogoutTime = DateTime.Now;
                        navMgr.NavigateTo("/exit?key=" + key, true);
                    }
                }
            }
            catch{}
            

            
        }

    }
}

