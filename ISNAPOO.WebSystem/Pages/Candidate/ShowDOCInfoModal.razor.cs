using ISNAPOO.WebSystem.Pages.Framework;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ShowDOCInfoModal : BlazorBaseComponent
    {
        private string information = string.Empty;
        private string title = string.Empty;

        public void OpenModal(string information, string title)
        {
            this.information = information;
            this.title = title;

            this.isVisible = true;
            this.StateHasChanged();
        }
    }
}
