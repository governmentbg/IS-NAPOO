using ISNAPOO.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ISNAPOO.Common.Constants;

namespace ISNAPOO.Core.UserContext
{
    public class UserContext : IUserContext
    {
        private string? savedString;

        public event Action? OnChange;


        public string UserName
        {
            get => savedString ?? string.Empty;
            set
            {
                savedString = value;
                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public string UserId => throw new NotImplementedException();

        public string Email => throw new NotImplementedException();

        public string FullName => throw new NotImplementedException();

      
        public bool IsUserInRole(string role)
        {
            throw new NotImplementedException();
        }

        

    }
}
