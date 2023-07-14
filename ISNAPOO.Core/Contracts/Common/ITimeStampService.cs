using ISNAPOO.Core.ViewModels.Common;
using System.IO;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface ITimeStampService : IBaseService
    {
        Task GenerateTimeStampFilesAsync(NotificationVM notification, bool generateForCPO);
    }
}
