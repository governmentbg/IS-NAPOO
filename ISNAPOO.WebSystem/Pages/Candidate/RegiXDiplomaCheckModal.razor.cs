using ISNAPOO.WebSystem.Pages.Framework;
using RegiX.Class.RDSO.GetDiplomaInfo;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class RegiXDiplomaCheckModal : BlazorBaseComponent
    {
        private DiplomaDocumentsType diplomaResponse = new DiplomaDocumentsType();
        private string identifier = string.Empty;

        public void OpenModal(DiplomaDocumentsType diplomaResponse, string identifier)
        {
            this.diplomaResponse = diplomaResponse;
            this.identifier = identifier;

            this.isVisible = true;
            this.StateHasChanged();
        }
    }
}
