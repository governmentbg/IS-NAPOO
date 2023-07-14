using RegiX.Adapters.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using TechnoLogica.RegiX.Adapters.Common.ExportExtension;
using TechnoLogica.RegiX.Common;
using TechnoLogica.RegiX.NapooStudentDocumentsAdapter;
using TechnoLogica.RegiX.NapooStudentDocumentsAdapter.AdapterService;

namespace TechnoLogica.RegiX.NapooStudentDocumentsAdapter.Mock
{    
    public class NapooStudentDocumentsAdapterMock : BaseAdapterServiceProxy<INapooStudentDocumentsAdapter>
    {
        [Export(typeof(IAdapterService))]
        [ExportSimpleName(typeof(NapooStudentDocumentsAdapterMock), typeof(IAdapterService))]
        [ExportFullName(typeof(NapooStudentDocumentsAdapterMock), typeof(IAdapterService))]
        public static INapooStudentDocumentsAdapter MockExport
        {
            get
            {
                return new NapooStudentDocumentsAdapterMock().Create();
            }
        }
    }
}

