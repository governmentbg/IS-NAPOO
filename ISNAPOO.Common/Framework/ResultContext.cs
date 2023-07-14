using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISNAPOO.Common.Framework
{
    /// <summary>
    /// Клас за подаване/получаване на данни между различните слоеве
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultContext<T> : IResultContext<T>
    {

        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? PersonFullName { get; set; }
        public int? NewEntityId { get; set; }
        public ResultContext()
        {
            this.ListErrorMessages = new List<string>();
            this.ListMessages = new List<string>();
            ResultContextObject = (T)Activator.CreateInstance(typeof(T));
        }
        public T ResultContextObject { get; set; }

       
        public ICollection<string> ListErrorMessages { get; set; }
        public ICollection<string> ListMessages { get; set; }

        /// <summary>
        /// Добаване на грешка към списък в ResultContext
        /// </summary>
        /// <param name="errorMessage"></param>
        public void AddErrorMessage(string errorMessage)
        {
            this.ListErrorMessages.Add(errorMessage);
        }

        public void AddMessage(string message)
        {
            this.ListMessages.Add(message);
        }

        /// <summary>
        /// Добаване на грешка към списък в ResultContext
        /// </summary>
        /// <param name="key"></param>
        /// <param name="errorMessage"></param>
        //public void AddErrorMessageWithKey(string key, string errorMessage)
        //{
        //    this.ListErrorMessages.Add(new ErrorMessage{Key = key,Msg = errorMessage});
        //}

        /// <summary>
        /// Връща дали в ResultContext има грешки
        /// </summary>
        public bool HasErrorMessages => this.ListErrorMessages.Any();
        public bool HasMessages => this.ListMessages.Any();
    }

    

   
}
