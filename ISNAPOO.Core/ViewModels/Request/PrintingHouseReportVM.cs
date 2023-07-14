using ISNAPOO.Common.Constants;
using System;
using System.Collections.Generic;

namespace ISNAPOO.Core.ViewModels.Request
{
    public class PrintingHouseReportVM
    {
        public PrintingHouseReportVM()
        {
            this.ProviderRequestDocumentsList = new List<ProviderRequestDocumentVM>();
        }

        public int Id { get; set; }

        public string District { get; set; }

        public long NAPOORequestNumber { get; set; }

        public DateTime? RequestDate { get; set; }

        public string NumberAndDate => $"{this.NAPOORequestNumber}/ {this.RequestDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г.";

        public NAPOORequestDocVM NAPOORequestDoc { get; set; }

        public List<ProviderRequestDocumentVM> ProviderRequestDocumentsList { get; set; }
    }
}
