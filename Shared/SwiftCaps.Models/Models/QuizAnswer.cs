using System;
using System.ComponentModel;
using SwiftCaps.Models.Helpers;

namespace SwiftCaps.Models.Models
{
    public class QuizAnswer : ModelBase, INotifyPropertyChanged
    {
        public Guid QuestionId { get; set; }

        public string ActualAnswer { get; set; }

        private string _userAnswer;
        public string UserAnswer
        {
            get => _userAnswer;
            set
            { 
                _userAnswer = value; // for dev testing - comment this
                //_userAnswer = ActualAnswer; // for dev testing - uncomment this
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(UserAnswer))
                    return false;

                var cleanUserAnswer = QuizHelper.CleanAnswer(UserAnswer);
                var cleanActualAnswer = QuizHelper.CleanAnswer(ActualAnswer);
                return cleanActualAnswer.Equals(cleanUserAnswer);
            }
        }
        
        public int AnswerIndex { get; set; }

        public string AnswerPrefix { get; set; }

        public string AnswerSuffix { get; set; }

        public int AnswerLength { get; set; }
       
    }
}
