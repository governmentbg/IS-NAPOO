namespace ISNAPOO.Core.ViewModels.Archive
{
    public class AnnualTrainerQualificationsVM
    {
        public int Id { get; set; }

        public string Profession { get; set; }

        public int InternalTrainingCount{ get; set; }

        public int InternalTrainingHours{ get; set; }

        public int ExternalTrainingCount { get; set; }

        public int ExternalTrainingHours { get; set; }

        public int TotalHours => this.InternalTrainingHours + this.ExternalTrainingHours;
    }
}
