using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Framework;
using Syncfusion.Blazor.Grids;
using static ISNAPOO.WebSystem.Extensions.BlazorCookieLoginMiddleware;

namespace ISNAPOO.WebSystem.Pages.Common
{
    public partial class OnlineUserList : BlazorBaseComponent
    {
        ICollection<LoginInfo> OnlineUser;
        SfGrid<LoginInfo> OnlineUserGrid = new SfGrid<LoginInfo>();

        protected override void OnInitialized()
        {
            this.OnlineUser = BlazorCookieLoginMiddleware.OnlineUsers.Values;
            var counter = 1;
            foreach (var user in OnlineUser)
            {
                user.Id = counter++;
            }
        }
    }
}
