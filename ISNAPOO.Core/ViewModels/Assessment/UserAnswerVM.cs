using Microsoft.EntityFrameworkCore;

namespace ISNAPOO.Core.ViewModels.Assessment
{
    public class UserAnswerVM
    {
        public int IdUserAnswer { get; set; }

        [Comment("Връзка с  отговор")]
        public int? IdAnswer { get; set; }

        public virtual AnswerVM Answer { get; set; }

        [Comment("Връзка с отворен отговор")]
        public int IdUserAnswerOpen { get; set; }

        public virtual UserAnswerOpenVM UserAnswerOpen { get; set; }

        [Comment("Връзка с въпрос")]
        public int IdQuestion { get; set; }

        public virtual QuestionVM Question { get; set; }

        [Comment("Точки")]
        public decimal? Points { get; set; }
    }
}
