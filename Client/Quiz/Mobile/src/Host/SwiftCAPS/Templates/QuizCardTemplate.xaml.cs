using System;
using SwiftCaps.Models.Enums;
using SwiftCaps.Models.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamariners.Mobile.Core.Infrastructure;
using Application = Xamarin.Forms.Application;

namespace SwiftCaps.Templates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuizCardTemplate
    {
        #region Bindable

        public static readonly BindableProperty UserQuizProperty = BindableProperty.Create(
            nameof(UserQuiz),
            typeof(UserQuiz),
            typeof(QuizCardTemplate),
            default(UserQuiz),
            propertyChanged: UserQuizPropertyChanged);

        private static void UserQuizPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            try
            {
                if (bindable is null || !(bindable is QuizCardTemplate card))
                {
                    return;
                }

                if (newvalue is null || !(newvalue is UserQuiz userQuiz))
                {
                    return;
                }

                var title = string.Empty;
                switch (userQuiz.Schedule.Recurrence)
                {
                    case Recurrence.Weekly:
                        title = "WEEK {0} QUIZ";
                        break;
                    case Recurrence.Monthly:
                        title = "MONTH {0} QUIZ";
                        break;
                }

                card.Title = string.Format(title, userQuiz.Sequence);

                if (userQuiz.Completed.HasValue)
                {
                    // Quiz is completed

                    // Load the Quiz Completed Styles
                    if (Application.Current.Resources.TryGetValue("CompletedTestCardStyle",
                        out var resource) && resource is Style style)
                    {
                        card.Style = style;
                    }

                    // Add the Quiz Completed Overlay
                    if (!card.MainGrid.Children.Contains(card.OverlayFrame))
                        card.MainGrid.Children.Add(card.OverlayFrame);
                }
                else
                {
                    // Quiz is not completed

                    // Remove the Quiz Completed Overlay
                    if (card.MainGrid.Children.Contains(card.OverlayFrame))
                        card.MainGrid.Children.Remove(card.OverlayFrame);
                    switch (userQuiz.Schedule.Recurrence)
                    {
                        // Load the General Styles
                        case Recurrence.Weekly:
                            {
                                if (Application.Current.Resources.TryGetValue("WeeklyTestCardStyle",
                                    out var resource) && resource is Style style)
                                {
                                    card.Style = style;
                                }

                                break;
                            }
                        case Recurrence.Monthly:
                            {
                                if (Application.Current.Resources.TryGetValue("MonthlyTestCardStyle",
                                    out var resource) && resource is Style style)
                                {
                                    card.Style = style;
                                }

                                break;
                            }
                    }
                }


                var sectionCount = 0;
                var questionCount = 0;

                foreach (var quizSection in userQuiz.Schedule?.Quiz?.QuizSections)
                {
                    sectionCount++;
                    questionCount += quizSection.Questions.Count;
                }

                card.SectionsQuestionsCount.Text = $"{sectionCount} Sections / {questionCount} Questions Total";
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public UserQuiz UserQuiz
        {
            get => (UserQuiz) GetValue(UserQuizProperty);
            set => SetValue(UserQuizProperty, value);
        }

        #endregion

        public static readonly BindableProperty ButtonTappedCommandProperty = BindableProperty.Create(
            nameof(ButtonTappedCommand),
            typeof(XDelegateCommand<UserQuiz>),
            typeof(QuizCardTemplate));

        public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
            nameof(AccentColor),
            typeof(Color),
            typeof(QuizCardTemplate),
            Color.Transparent);

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(QuizCardTemplate));

        public static readonly BindableProperty TitleImageProperty = BindableProperty.Create(
            nameof(TitleImage),
            typeof(ImageSource),
            typeof(QuizCardTemplate));

        public static readonly BindableProperty ButtonTextProperty = BindableProperty.Create(
            nameof(ButtonText),
            typeof(string),
            typeof(QuizCardTemplate));

        public static readonly BindableProperty IsTestCompletedProperty = BindableProperty.Create(
            nameof(IsTestCompleted),
            typeof(bool),
            typeof(QuizCardTemplate));

        public XDelegateCommand<UserQuiz> ButtonTappedCommand
        {
            get => (XDelegateCommand<UserQuiz>) GetValue(ButtonTappedCommandProperty);
            set => SetValue(ButtonTappedCommandProperty, value);
        }

        public Color AccentColor
        {
            get => (Color) GetValue(AccentColorProperty);
            set => SetValue(AccentColorProperty, value);
        }

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public ImageSource TitleImage
        {
            get => (ImageSource) GetValue(TitleImageProperty);
            set => SetValue(TitleImageProperty, value);
        }

        public string ButtonText
        {
            get => (string) GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public bool IsTestCompleted
        {
            get => (bool) GetValue(IsTestCompletedProperty);
            set => SetValue(IsTestCompletedProperty, value);
        }

        public QuizCardTemplate()
        {
            InitializeComponent();
            MainGrid.Children.Remove(OverlayFrame);
        }
    }
}
