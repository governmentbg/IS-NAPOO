using System;
using Microsoft.AspNetCore.Identity;

namespace ISNAPOO.WebSystem.Shared
{
    public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "Паролата трябва да задържа поне една цифра!"
            };
        }

        public override IdentityError PasswordMismatch()
        {
              return new IdentityError
            {
                Code = nameof(PasswordMismatch),
                Description = "Грешна стара парола!"
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "Паролата трябва да съдържа поне една малка буква!"
            };
        }
        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "Паролата трябва да съдържа поне една голяма буква!"
            };
        }

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUniqueChars),
                Description = $"Паролата трябва да съдържа {uniqueChars} символа!"
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $""
            };
        }

         
    }
}

