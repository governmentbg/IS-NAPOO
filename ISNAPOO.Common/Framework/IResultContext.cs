using System.Collections.Generic;

namespace ISNAPOO.Common.Framework
{
    public interface IResultContext<T>
    {
        bool HasErrorMessages { get; }
        bool HasMessages { get; }
        ICollection<string> ListErrorMessages { get; set; }
        ICollection<string> ListMessages { get; set; }
        int? NewEntityId { get; set; }
        string? PersonFullName { get; set; }
        T ResultContextObject { get; set; }
        int? UserId { get; set; }
        string? UserName { get; set; }

        void AddErrorMessage(string errorMessage);
        void AddMessage(string message);
        //void AddErrorMessageWithKey(string key, string errorMessage);
    }
}