using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Common;
using Microsoft.AspNetCore.Components;

namespace ISNAPOO.WebSystem.Extensions
{
    public interface ICommonService
    {
        

        public static async Task clearLocalStorage()
        {
           
        }

        ResultContext<TokenVM> GetDecodeToken(ResultContext<TokenVM> currentContext);   
        string GetTokenWithParams(ResultContext<TokenVM> currentContext, int Minutes = GlobalConstants.MAX_MINUTE_VALID_TOKEN);
    }
}
