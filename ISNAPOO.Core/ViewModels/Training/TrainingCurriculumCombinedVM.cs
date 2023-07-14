using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class TrainingCurriculumCombinedVM
    {
        public List<TrainingCurriculumVM> theory { get; set; }

        public List<TrainingCurriculumVM> productionPractice { get; set; }
        public List<TrainingCurriculumVM> educationalPractice { get; set; }



    }
}
