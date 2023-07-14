using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Archive;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Archive
{
    public interface IRegiXLogRequestService : IBaseService
    {
        Task CreateRegiXLogRequestAsync(ResultContext<RegiXLogRequestVM> inputContext);
    }
}
