using System.Collections.Generic;

namespace ISNAPOO.Core.ViewModels.Assessment
{
    public class UserAnswerModel
    {
        public UserAnswerModel()
        {
            this.AnswerIds = new List<int>();
            this.OpenAnswerText = string.Empty;
        }

        public int IdQuestion { get; set; }

        public string OpenAnswerText { get; set; }

        public List<int> AnswerIds { get; set; }

        public decimal Points { get; set; }
        public bool HasPoints { get; set; }
    }
}
