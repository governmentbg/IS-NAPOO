using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechnoLogica.RegiX.Adapters.TestUtils;
using TechnoLogica.RegiX.NapooStudentDocumentsAdapter.AdapterService;
using TechnoLogica.RegiX.NapooStudentDocumentsAdapter.Mock;

namespace TechnoLogica.RegiX.NapooStudentDocumentsAdapter.Tests
{
    [TestClass]
    public class NapooStudentDocumentsAdapterMockTest : MockTest<NapooStudentDocumentsAdapterMock, INapooStudentDocumentsAdapter>
    {
    }
}

