using System;
using System.Collections.Generic;
using System.Text;

namespace ISNAPOO.Common.Framework
{
    public class TokenVM
    {

        public TokenVM()
        {
            ListDecodeParams = new List<KeyValuePair<string, object>>();
        }
        public string Token { get; set; }
        public string DecodeToken { get; set; }

        public bool IsValid { get; set; }

        public List<KeyValuePair<string, object>> ListDecodeParams { get; set; }
    }
}
