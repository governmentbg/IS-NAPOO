using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts
{
    /// <summary>
    /// Текущ контекст на изпълнение на операциите
    /// </summary>
    public interface IUserContext
    {
        /// <summary>
        /// Идентификатор на потребител
        /// </summary>
        string UserId { get; }

       
        /// <summary>
        /// Електронна поща на потребител
        /// </summary>
        string Email { get; }

        /// <summary>
        /// Имена на потребител
        /// </summary>
        string FullName { get; }
        string UserName { get; set; }

        event Action? OnChange;

        /// <summary>
        /// Проверка за налична роля на потребител
        /// </summary>
        /// <param name="role">Наименование на роля</param>
        /// <returns></returns>
        bool IsUserInRole(string role);

    }
}
