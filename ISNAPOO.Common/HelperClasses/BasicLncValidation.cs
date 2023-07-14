using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ISNAPOO.Common.HelperClasses
{
    public class BasicLncValidation
    {
        private const string cErrorMessage = "Невалидно ЛНЧ!";
        private string lnch;

        public string ErrorMessage { get; private set; } = string.Empty;

        public BasicLncValidation(string lnch)
        {
            this.lnch = lnch;
        }

        public bool Validate()
        {
            bool isValid = CheckLNCH(lnch);

            if (!isValid)
            {
                ErrorMessage = cErrorMessage;
            }

            return isValid;
        }


        private bool CheckLNCH(string personalId)
        {
            Regex rgx = new Regex(@"^\d{10}$");

            if (string.IsNullOrEmpty(personalId) ||
                !rgx.IsMatch(personalId))
            {
                return false;
            }

            int lastNumber = 0;
            int sum = 0;
            int[] multipliers = new int[] { 21, 19, 17, 13, 11, 9, 7, 3, 1 };

            for (int i = 0; i < personalId.Length - 1; i++)
            {
                lastNumber = int.Parse(personalId[i].ToString());
                sum += lastNumber * multipliers[i];
            }

            lastNumber = int.Parse(personalId[personalId.Length - 1].ToString());

            return sum % 10 == lastNumber;
        }
    }
}
