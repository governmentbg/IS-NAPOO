using ISNAPOO.Core.DataViewModels.Registers;
using ISNAPOO.Core.ViewModels.Training;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface IRegisterService
    {
        Task<List<ProfessionalCertificateDataView>> GetProfessionalCertificateDataView(ClientCourseDocumentVM model);
    }
}
