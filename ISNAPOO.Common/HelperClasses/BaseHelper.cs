using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Collections;

namespace ISNAPOO.Common.HelperClasses
{
    public class BaseHelper
    {
        public static string GetTokenWithParams(List<KeyValuePair<string, object>> keyValuePairs, int Minutes = GlobalConstants.MAX_MINUTE_VALID_TOKEN)
        {

            string res = string.Empty;

            res = JwtBuilder.Create()
                      .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                      .WithSecret(GlobalConstants.TOKEN_SECRET)
                      .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(Minutes).ToUnixTimeSeconds())
                      .AddClaims(keyValuePairs)
                      .Encode();

            return res;
        }

        public static ResultContext<TokenVM>  GetDecodeToken(ResultContext<TokenVM> currentContext)
        {

            try
            {
                JWT.IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);



                currentContext.ResultContextObject.DecodeToken = decoder.Decode(currentContext.ResultContextObject.Token, GlobalConstants.TOKEN_SECRET, verify: true);
                currentContext.ResultContextObject.IsValid = true;

                var deserializeObject = serializer.Deserialize<Newtonsoft.Json.Linq.JObject>(currentContext.ResultContextObject.DecodeToken);

                foreach (var item in deserializeObject)
                {
                    currentContext.ResultContextObject.ListDecodeParams.Add(new KeyValuePair<string, object>(item.Key, item.Value));
                }



            }
            catch (TokenExpiredException e)
            {
                throw e;
            }
            catch (SignatureVerificationException e)
            {
                throw e;

            }
            catch (Exception ee)
            {
                throw ee;

            }
            return currentContext;
        }

        public static float? ConvertToFloat(object obj, int decimalDigits)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return new Nullable<float>();
            }
            else
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                nfi.NumberDecimalDigits = decimalDigits;

                float tmpValue = 0;
                if (float.TryParse(obj.ToString().Replace(",", "."), NumberStyles.Any, nfi, out tmpValue))
                {
                    return tmpValue;
                }
                else
                {
                    return new Nullable<float>();
                }
            }
        }

        public static DateTimeFormatInfo GetDateTimeFormatInfo()
        {
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.DateSeparator = ".";
            dtfi.ShortDatePattern = GlobalConstants.DATE_FORMAT;

            return dtfi;
        }
        public static string GetGradeName(double grade)
        {
            if (grade >= 5.50 && grade <= 6)
            {
                return "Отличен";
            }
            else if (grade >= 4.50 && grade < 5.50)
            {
                return "Много добър";
            }
            else if (grade >= 3.50 && grade < 4.50)
            {
                return "Добър";
            }
            else if (grade >= 3 && grade < 3.50)
            {
                return "Среден";
            }
            else if (grade < 3)
            {
                return "Слаб";
            }
            else
            {
                return null;
            }
        }
        static public string ConvertCyrToLatin(string arg)
        {
            StringBuilder builder = new StringBuilder(arg, arg.Length + 10);
            //convert lower-case letters
            builder.Replace("а", "a");
            builder.Replace("б", "b");
            builder.Replace("в", "v");
            builder.Replace("г", "g");
            builder.Replace("д", "d");
            builder.Replace("е", "e");
            builder.Replace("ж", "zh");
            builder.Replace("з", "z");
            builder.Replace("и", "i");
            builder.Replace("й", "y");
            builder.Replace("к", "k");
            builder.Replace("л", "l");
            builder.Replace("м", "m");
            builder.Replace("н", "n");
            builder.Replace("о", "o");
            builder.Replace("п", "p");
            builder.Replace("р", "r");
            builder.Replace("с", "s");
            builder.Replace("т", "t");
            builder.Replace("у", "u");
            builder.Replace("ф", "f");
            builder.Replace("х", "h");
            builder.Replace("ц", "ts");
            builder.Replace("ч", "ch");
            builder.Replace("ш", "sh");
            builder.Replace("щ", "sht");
            builder.Replace("ъ", "a");
            builder.Replace("ь", "y");
            builder.Replace("ю", "yu");
            builder.Replace("я", "ya");

            //convert upper-case letters
            builder.Replace("А", "A");
            builder.Replace("Б", "B");
            builder.Replace("В", "V");
            builder.Replace("Г", "G");
            builder.Replace("Д", "D");
            builder.Replace("Е", "E");
            builder.Replace("Ж", "Zh");
            builder.Replace("З", "Z");
            builder.Replace("И", "I");
            builder.Replace("Й", "Y");
            builder.Replace("К", "K");
            builder.Replace("Л", "L");
            builder.Replace("М", "M");
            builder.Replace("Н", "N");
            builder.Replace("О", "O");
            builder.Replace("П", "P");
            builder.Replace("Р", "R");
            builder.Replace("С", "S");
            builder.Replace("Т", "T");
            builder.Replace("У", "U");
            builder.Replace("Ф", "F");
            builder.Replace("Х", "H");
            builder.Replace("Ц", "Ts");
            builder.Replace("Ч", "Ch");
            builder.Replace("Ш", "Sh");
            builder.Replace("Щ", "Sht");
            builder.Replace("Ъ", "A");
            builder.Replace("Ь", "Y");
            builder.Replace("Ю", "Yu");
            builder.Replace("Я", "Ya");
            builder.Replace("-", "-");

            return builder.ToString();
        }
        public static string GetTotalMonths(DateTime firstDate, DateTime lastDate)
        {
            int yearsApart = lastDate.Year - firstDate.Year;

            int monthsApart = lastDate.Month - firstDate.Month;
            if (lastDate.Day >= 15 || monthsApart == 0)
            {
                monthsApart++;
            }
            return (yearsApart * 12) + monthsApart == 1 ? (yearsApart * 12) + monthsApart + " месец" : (yearsApart * 12) + monthsApart + " месеца";
        }

        public static string CalculateHmac(string secretCode, string content)
        {
            string hmac;

            var secretCodeBytes = Encoding.UTF8.GetBytes(secretCode);
            var contentBytes = Encoding.UTF8.GetBytes(content);

            using (var hmacSHA256 = new HMACSHA256(secretCodeBytes))
            {
                var hash = hmacSHA256.ComputeHash(contentBytes);
                hmac = Convert.ToBase64String(hash);
            }

            return hmac;
        }

        public static string Decrypt(string encryptedQueryString)
        {
            try
            {
                byte[] buffer = Convert.FromBase64String(encryptedQueryString);
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
                des.Key = MD5.ComputeHash(ASCIIEncoding.UTF8.GetBytes(GlobalConstants.ENCRYPTION_KEY));
                des.IV = GlobalConstants.ENCRYPTION_IV;
                return Encoding.UTF8.GetString(
                    des.CreateDecryptor().TransformFinalBlock(
                    buffer,
                    0,
                    buffer.Length
                    )
                    );
            }
            catch (CryptographicException)
            {
                throw new CryptographicException();
            }
            catch (FormatException)
            {
                throw new FormatException();
            }

        }

        public static string Encrypt(string serializedQueryString)
        {

            byte[] buffer = Encoding.UTF8.GetBytes(serializedQueryString);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            des.Key = MD5.ComputeHash(ASCIIEncoding.UTF8.GetBytes(GlobalConstants.ENCRYPTION_KEY));
            des.IV = GlobalConstants.ENCRYPTION_IV;
            string encriptString = Convert.ToBase64String(
                des.CreateEncryptor().TransformFinalBlock(
                buffer,
                0,
                buffer.Length
                )
                );
            return encriptString;
        }
        public static int CalculateNumberOfLines(string text, float boxWidthInPoints, Font font)
        {
            var bitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            bitmap.SetResolution(72, 72);



            using (var g = Graphics.FromImage(bitmap))
            {


                var stringFormat = StringFormat.GenericTypographic;
                stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                int charactersFitted;
                int linesFitted;
                g.MeasureString(text, font, new SizeF(boxWidthInPoints, 0), stringFormat, out charactersFitted,
                    out linesFitted);
                if (text.Trim().Length == 0)
                {
                    return linesFitted - 1;
                }
                return linesFitted;
            }

        }

        


    }
}
