using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ISNAPOO.Common.HelperClasses
{
    public class BasicEGNValidation
    {
        private const string cErrorMessage = "Невалидно ЕГН!";
        private string egn;

        public string ErrorMessage { get; private set; } = string.Empty;

        public BasicEGNValidation(string egn)
        {
            this.egn = egn;
        }

        public bool Validate()
        {
            bool isValid = CheckEGN(egn);

            if (!isValid)
            {
                ErrorMessage = cErrorMessage;
            }

            return isValid;
        }

        private bool CheckEGN(string personalId)
        {
            int[] egnWeights = new int[] { 2, 4, 8, 5, 10, 9, 7, 3, 6 };
            Regex rgx = new Regex(@"^\d{10}$");

            if (string.IsNullOrEmpty(personalId) ||
                !rgx.IsMatch(personalId))
            {
                return false;
            }

            (int year, int month, int day) = ParseEGNDate(personalId);

            if (!CheckDate(year, month, day))
            {
                return false;
            }

            int checksum = int.Parse(personalId.Substring(9, 1));
            int egnSum = 0;

            for (int i = 0; i < 9; i++)
            {
                egnSum += int.Parse(personalId.Substring(i, 1)) * egnWeights[i];
            }

            int validChecksum = egnSum % 11;

            if (validChecksum == 10)
            {
                validChecksum = 0;
            }

            return checksum == validChecksum;
        }

        private bool CheckDate(int year, int month, int day)
        {
            bool result = true;

            try
            {
                var date = new DateTime(year, month, day);
            }
            catch (ArgumentOutOfRangeException)
            {
                result = false;
            }

            return result;
        }

        private (int year, int month, int day) ParseEGNDate(string egn)
        {
            int year = int.Parse(egn.Substring(0, 2));
            int month = int.Parse(egn.Substring(2, 2));
            int day = int.Parse(egn.Substring(4, 2));

            if (month > 40)
            {
                year += 2000;
                month -= 40;
            }
            else if (month > 20)
            {
                year += 1800;
                month -= 20;
            }
            else
            {
                year += 1900;
            }

            return (year, month, day);
        }

    }
}

