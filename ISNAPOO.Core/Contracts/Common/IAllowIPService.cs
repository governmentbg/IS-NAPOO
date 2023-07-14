using Data.Models.Data.Common;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.EGovPayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface IAllowIPService
    {
        Task<IEnumerable<AllowIPVM>> GetAllAllowIPsAsync();
        Task<AllowIPVM> GetAllowIPAsync(int idAllowIP);
        Task<ResultContext<AllowIPVM>> CreateAllowIPAsync(ResultContext<AllowIPVM> resultContext);
        Task<ResultContext<AllowIPVM>> UpdateAllowIPAsync(ResultContext<AllowIPVM> resultContext);
        Task<ResultContext<AllowIPVM>> DeleteAllowIPdByIdAsync(int idAllowIP);
    }
}
