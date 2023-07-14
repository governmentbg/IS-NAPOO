using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ISNAPOO.WebSystem.Pages
{
    public partial class _Host : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
       // private readonly IRememberMe _rememberMe;


        

        public _Host(IHttpContextAccessor httpContextAccessor)
        {
            //_rememberMe = new RememberMe();
            ////_rememberMe = rememberMe;
            //_httpContextAccessor = httpContextAccessor;
        }

        public void OnGet()
        {
           //_rememberMe.RemoteIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

        }
    }

    //public interface IRememberMe
    //{
    //    public string RemoteIpAddress { get; set; }
    //}

    //public class RememberMe : IRememberMe
    //{
    //    public string RemoteIpAddress { get; set; }
    //}
}
