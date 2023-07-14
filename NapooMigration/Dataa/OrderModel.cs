using System;
using System.Text.RegularExpressions;

namespace NapooMigration.Data
{
	public class OrderModel
	{
		public string? Order { get; set; }
		public string? Date { get; set; }
		public int oldId { get; set; }

        public OrderModel(string reg)
        {		
			Regex regex = new Regex("(?<order>\\p{Lu}+?\\s?\\S+?\\s?\\d+?\\S+?\\s?\\S\\s?\\d+)\\s?(?<delimeter>\\/|\\S+)\\s?(?<date>\\d{2}.\\d{2}.\\d{4})?");
			Match match = regex.Match(reg);
			if(match.Success)
			{
				Order = match.Groups["order"].ToString();
				Order.Replace(" ", string.Empty);
				Date = match.Groups["date"].ToString();
			}
			else
			{
				Order = reg;
				Date = String.Empty;
			}
        }
    }
}

