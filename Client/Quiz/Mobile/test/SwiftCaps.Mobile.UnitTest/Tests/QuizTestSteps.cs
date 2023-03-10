using System;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using Shouldly;
using SwiftCaps.Data.Context;
using SwiftCaps.Models.Enums;
using SwiftCaps.Models.Models;
using SwiftCaps.ViewModels;
using SwiftCAPS.Mobile.UnitTest.Infrastructure;
using TechTalk.SpecFlow;
using Unity;
using Xamariners.Core.Common.Helpers;
using Xamariners.Mobile.Core.Interfaces;

namespace SwiftCAPS.Mobile.UnitTest.Tests
{
    [Binding]
    public class QuizTestSteps : StepBase
    {
        private readonly SwiftCapsContext _swiftCapsContext;
        private QuizPageViewModel _viewModel => GetCurrentViewModel<QuizPageViewModel>();

        public QuizTestSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
            _swiftCapsContext = App.Container.Resolve<SwiftCapsContext>();
        }

        [Then(@"the user quiz has been created")]
        public async void ThenTheUserQuizHasBeenCreated()
        {
            _viewModel.ShouldNotBeNull();

             _viewModel?.UserQuiz?.Schedule?.Quiz?.QuizSections
                .Any(qs => qs.Questions
                .Any(q => q.QuizAnswers.Any()))
                .ShouldBeTrue();

            _scenarioContext.AddReplace("vm", _viewModel);
        }

        [Given(@"I am on the ""(.*)"" quiz")]
        [Then(@"I am on the ""(.*)"" quiz")]
        public void GivenIAmOnTheQuiz(Recurrence recurrence)
        {  
            _viewModel?.UserQuiz?.Schedule?.Recurrence.ShouldBe(recurrence);
        }
        
        [Given(@"I am on the quiz section (.*) out of (.*)")]
        [Then(@"I am on the quiz section (.*) out of (.*)")]

        public void GivenIAmOnTheQuizSectionOutOf(int currentSection, int totalSections)
        {
            _viewModel?.UserQuiz?.Schedule?.Quiz.QuizSections.Count().ShouldBe(totalSections);
            _viewModel.CurrentQuizSectionIndex.ShouldBe(currentSection -1);
        }

        [Given(@"I am on the question (.*) out of (.*)")]
        [Then(@"I am on the question (.*) out of (.*)")]
        public void GivenIAmOnTheQuestionOutOf(int currentQuestion, int totalQuestions)
        {
            _viewModel.UserQuiz.Schedule.Quiz.QuizSections.Skip(_viewModel.CurrentQuizSectionIndex).First()
                .Questions.Count().ShouldBe(totalQuestions);
            _viewModel.SelectedQuestionIndex.ShouldBe(currentQuestion - 1);
        }

        [Then(@"I can see (.*) answers placeholders")]
        public void ThenICanSeeAnswersPlaceholders(int answers)
        {
            _viewModel.CurrentQuestion.QuizAnswers.Count().ShouldBe(answers);
        }

        [Then(@"I can see the answer (.*) is invalid")]
        public void ThenICanSeeTheAnswerIsInvalid(int answerIndex)
        {
            _viewModel.CurrentQuestion.QuizAnswers[answerIndex].IsValid.ShouldBeFalse();
        }

        [Then(@"I can see the answer (.*) is valid")]
        public void ThenICanSeeTheAnswerIsValid(int answerIndex)
        {
            _viewModel.CurrentQuestion.QuizAnswers[answerIndex].IsValid.ShouldBeTrue();
        }

        [When(@"I enter ""(.*)"" in the answer (.*)")]
        public void WhenIEnterInTheAnswer(string answerText, int answerIndex)
        {
            _viewModel.CurrentQuestion.QuizAnswers[answerIndex].UserAnswer = answerText;

            // mimics answer unfocus
            _viewModel.ValidateSectionCommand.Execute();

            if (_viewModel.ValidateSectionCommand is { IsValid: true })
                _viewModel.ValidateSectionCommand?.WaitHandle.WaitOne();
        }

        [Then(@"I can see all the answers on the question (.*) are valid")]
        public void ThenICanSeeAllTheAnswersOnTheQuestionAreValid(int index)
        {
            _viewModel.CurrentSection.Questions[index - 1].QuizAnswers.Any(x => !x.IsValid).ShouldBeFalse();
        }

        [When(@"I navigate to the next question")]
        public void WhenINavigateToTheNextQuestion()
        {
            _viewModel.CurrentSection.Questions[_viewModel.SelectedQuestionIndex].Id.ShouldBe(_viewModel.CurrentQuestion.Id);

            _viewModel.SelectedQuestionIndex++;

            _viewModel.CurrentSection.Questions[_viewModel.SelectedQuestionIndex].Id.ShouldBe(_viewModel.CurrentQuestion.Id);

        }

        [When(@"I correctly answer all the answers on the question (.*)")]
        public void WhenICorrectlyAnswerAllTheAnswersOnTheQuestion(int index)
        {
            _viewModel.CurrentSection.Questions[index - 1].QuizAnswers.ForEach(answer =>
            {
                // fill answers
                answer.UserAnswer = answer.ActualAnswer;
                answer.IsValid.ShouldBeTrue();

                // answer unfocus => validate
                _viewModel.ValidateSectionCommand.Execute();

                if (_viewModel.ValidateSectionCommand is { IsValid: true })
                    _viewModel.ValidateSectionCommand?.WaitHandle.WaitOne();
            });

            ThenICanSeeAllTheAnswersOnTheQuestionAreValid(index);
        }

        [When(@"I correctly answer all the answers on the quiz section (.*)")]
        public void WhenICorrectlyAnswerAllTheAnswersOnTheQuizSection(int index)
        {
            _viewModel.CurrentSection.Questions.ForEach(q =>
                q.QuizAnswers.ForEach(answer =>
                {
                    answer.UserAnswer = answer.ActualAnswer;
                    answer.IsValid.ShouldBeTrue();
                }));

            _viewModel.ValidateSectionCommand.Execute();

            if (_viewModel.ValidateSectionCommand is { IsValid: true })
                _viewModel.ValidateSectionCommand?.WaitHandle.WaitOne();

            _viewModel.CurrentSection.Index.ShouldBe(index);
        }

        [When(@"I correctly answer all the questions of the ""(.*)"" quiz")]
        public void WhenICorrectlyAnswerAllTheQuestionsOfTheQuiz(Recurrence recurrent)
        {
            int count;
            var sections = _viewModel.UserQuiz.Schedule.Quiz.QuizSections.Count();

            for (count = 1; count <= sections; count++)
            {
                // fill section questions
                WhenICorrectlyAnswerAllTheAnswersOnTheQuizSection(count);

                _viewModel.CurrentSection.IsValid.ShouldBeTrue(count.ToString());
                _viewModel.UserQuiz.Schedule.Quiz.QuizSections.Single(x => x.Index == _viewModel.CurrentSection.Index).IsValid.ShouldBeTrue(count.ToString());

                // submit section
                _viewModel.SubmitSectionCommand.Execute();

                if (_viewModel.SubmitSectionCommand is {IsValid: true})
                    _viewModel.SubmitSectionCommand?.WaitHandle.WaitOne();
            }


            _viewModel.UserQuiz.Schedule.Quiz.QuizSections.Any(x => !x.IsValid).ShouldBeFalse(_viewModel.UserQuiz.Schedule.Quiz.QuizSections.Count(x => x.IsValid).ToString());
        }


        [Then(@"I can see all the answers on the quiz section (.*) are valid")]
        public void ThenICanSeeAllTheAnswersOnTheQuizSectionAreValid(int index)
        {
            _viewModel.UserQuiz.Schedule.Quiz.QuizSections[index - 1].Questions
                .Any(q => q.QuizAnswers.Any(x => !x.IsValid)).ShouldBeFalse();
        }

        [Then(@"I see that pop up is displayed")]
        public void ThenISeeThatPopUpIsDisplayed()
        {
            App.Container.Resolve<IPopupInputService>().IsShowing.ShouldBeTrue();
        }

        [When(@"I click on the close button")]
        public void WhenIClickOnTheCloseButton()
        {
            _viewModel.CloseInfoCommand?.Execute();

            if(_viewModel.CloseInfoCommand is {IsValid: true})
                _viewModel.CloseInfoCommand?.WaitHandle.WaitOne();

            App.Container.Resolve<IPopupInputService>().IsShowing.ShouldBeFalse();
        }

        [Then(@"the quiz is complete")]
        public void ThenTheQuizIsComplete()
        {
            ////  submit the quiz
            if (_viewModel.IsQuizCompleted)
            {
                _viewModel.SubmitSectionCommand.Execute();

                if (_viewModel.SubmitSectionCommand is { IsValid: true })
                    _viewModel.SubmitSectionCommand?.WaitHandle.WaitOne();
            }

            _swiftCapsContext.UserQuizzes.Any(uq => uq.Id == _viewModel.UserQuiz.Id).ShouldBeTrue();
            _swiftCapsContext.UserQuizzes.First(uq => uq.Id == _viewModel.UserQuiz.Id).Completed.HasValue.ShouldBeTrue();
        }

        [Then(@"the quiz is not complete")]
        public void ThenTheQuizIsNotComplete()
        {
            _viewModel.IsQuizCompleted.ShouldBeFalse();
            _swiftCapsContext.UserQuizzes.Any(uq => uq.Id == _viewModel.UserQuiz.Id).ShouldBeFalse();
        }


        [Then(@"I take note of the completed quiz id")]
        public void ThenITakeNoteOfTheCompletedQuizId()
        {
            _scenarioContext.Add(nameof(UserQuiz), _viewModel.UserQuiz.Id);
        }

        [Then(@"I can see the completed quiz with the id I took note of")]
        public void ThenICanSeeTheCompletedQuizWithTheIdITookNoteOf()
        {
            var quizId = _scenarioContext.Get<Guid>(nameof(UserQuiz));

            var vm = GetCurrentViewModel<QuizListPageViewModel>();
            var quiz = vm.UserQuizzes.FirstOrDefault(uq => uq.Id == quizId);

            quiz.ShouldNotBeNull();
            quiz.Id.ShouldBe(quizId);

            quiz.Completed.HasValue.ShouldBeTrue();
        }
    }
}
