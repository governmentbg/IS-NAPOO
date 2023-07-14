using System.Text;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using RegiX.Class.AVTR.GetActualState;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ShowRegixDataModal : BlazorBaseComponent
    {
        private SfDialog sfDialog = new SfDialog();
        private ActualStateResponseType actualStateRegix = new ActualStateResponseType();

        public async Task OpenModal(ActualStateResponseType actualStateResponseType)
        {
            actualStateRegix = actualStateResponseType;
            this.isVisible = true;
            this.StateHasChanged();
        }

        private string GetDetailsFromRegix()
        {
            var names = this.actualStateRegix.Details.Where(d => d.FieldCode == "10").Select(m => m.Subject.Name).ToList();
            var joinStr = string.Join(", ", names);
            return joinStr;
        }

        private string GetAddressFromRegix()
        {
            var sb = new StringBuilder();

            sb.Append($"{this.actualStateRegix.Seat.Address.Country}, ");
            sb.Append($"област {this.actualStateRegix.Seat.Address.District}, ");
            sb.Append($"община {this.actualStateRegix.Seat.Address.Municipality}, ");
            sb.Append($"{this.actualStateRegix.Seat.Address.Settlement} {this.actualStateRegix.Seat.Address.PostCode}, {this.actualStateRegix.Seat.Address.Area}, ");
            //Ако няма попълнена улица значи е блок
            if (!string.IsNullOrEmpty(this.actualStateRegix.Seat.Address.Block))
            {
                sb.Append($"жк. {this.actualStateRegix.Seat.Address.HousingEstate}, бл. {this.actualStateRegix.Seat.Address.Block}, ");
                sb.Append($"вх. {this.actualStateRegix.Seat.Address.Entrance}, ет. {this.actualStateRegix.Seat.Address.Floor}, ап. {this.actualStateRegix.Seat.Address.Apartment}, ");

            }
            else if (!string.IsNullOrEmpty(this.actualStateRegix.Seat.Address.Street))
            {
                sb.Append($"{this.actualStateRegix.Seat.Address.Street} {this.actualStateRegix.Seat.Address.StreetNumber}");
            }

            if (this.actualStateRegix.Seat.Contacts != null)
            {
                sb.Append($"тел. {this.actualStateRegix.Seat.Contacts.Phone}, ");
                sb.Append($"факс {this.actualStateRegix.Seat.Contacts.Fax}, ");
                sb.Append($"Електронна поща: {this.actualStateRegix.Seat.Contacts.EMail}, ");
                sb.Append($"Интернет страница: {this.actualStateRegix.Seat.Contacts.URL}");
            }

            return sb.ToString();
        }
    }
}
