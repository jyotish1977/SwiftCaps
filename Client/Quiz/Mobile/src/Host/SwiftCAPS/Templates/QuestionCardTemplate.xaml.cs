using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using MoreLinq;
using SwiftCaps.Client.Core.Enums;
using SwiftCaps.Models.Models;
using SwiftCaps.Triggers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamariners.Mobile.Core.Behaviors;
using Xamariners.Mobile.Core.Helpers.MVVM;
using Application = Xamarin.Forms.Application;

namespace SwiftCaps.Templates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionCardTemplate : ContentView, INotifyPropertyChanged
    {
        private Question _question;

        public static readonly BindableProperty SubmitCommandProperty = BindableProperty.Create(
            nameof(SubmitCommand), 
            typeof(ICommand), 
            typeof(QuestionCardTemplate), 
            default(ICommand));

        public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
            nameof(AccentColor),
            typeof(Color),
            typeof(QuizCardTemplate),
            Color.Transparent);

        public static readonly BindableProperty TriggerOnAppearingCommandProperty = BindableProperty.Create(
            nameof(SubmitCommand),
            typeof(ICommand),
            typeof(QuestionCardTemplate),
            default(ICommand));

        public static readonly BindableProperty AnswerUnfocusedCommandProperty = BindableProperty.Create(
            nameof(AnswerUnfocusedCommand), 
            typeof(DelegateCommand), 
            typeof(QuestionCardTemplate), 
            default(DelegateCommand));

         public static readonly BindableProperty OpenInfoCommandProperty = BindableProperty.Create(
            nameof(OpenInfoCommand),
            typeof(DelegateCommand),
            typeof(QuestionCardTemplate),
            default(DelegateCommand));
        
        public static readonly BindableProperty QuizLayoutStyleProperty = BindableProperty.Create(
            nameof(QuizLayoutStyle),
            typeof(QuizAnswersLayout),
            typeof(QuestionCardTemplate),
            default(QuizAnswersLayout));

        public ICommand SubmitCommand
        {
            get => (ICommand)GetValue(SubmitCommandProperty);
            set => SetValue(SubmitCommandProperty, value);
        }
        
        /// <summary>
        /// Allow you to execute any given behavior
        /// upon OnAppearing of the Question Card
        /// </summary>
        public ICommand TriggerOnAppearingCommand
        {
            get => (ICommand)GetValue(TriggerOnAppearingCommandProperty);
            set => SetValue(TriggerOnAppearingCommandProperty, value);
        }
        
        public DelegateCommand AnswerUnfocusedCommand
        {
            get => (DelegateCommand)GetValue(AnswerUnfocusedCommandProperty);
            set => SetValue(AnswerUnfocusedCommandProperty, value);
        }
        
        public DelegateCommand OpenInfoCommand
        {
            get => (DelegateCommand)GetValue(OpenInfoCommandProperty);
            set => SetValue(OpenInfoCommandProperty, value);
        }

        public QuizAnswersLayout QuizLayoutStyle
        {
            get => (QuizAnswersLayout)GetValue(QuizLayoutStyleProperty);
            set => SetValue(QuizLayoutStyleProperty, value);
        }

        public Color AccentColor
        {
            get => (Color)GetValue(AccentColorProperty);
            set => SetValue(AccentColorProperty, value);
        }

        public Command<int> AnswerTappedCommand { get; set; }

        public QuestionCardTemplate()
        {
            InitializeComponent();
            AnswerTappedCommand = new Command<int>(AnswerTapped);
            TriggerOnAppearingCommand = new Command(TriggerOnAppearing);
        }

        protected override void OnBindingContextChanged()
        {
            // just fire on the first pass
            var newQuestion = (Question) BindingContext;
            
            if (newQuestion != null && (newQuestion?.Id != _question?.Id || _question == null))
            {
                _question = newQuestion;

                SetupQuestion(_question);
                SetupAnserCollectionVeiwHeight(_question.QuizAnswers?.Count);
            }

            base.OnBindingContextChanged();
        }

        private void SetupAnserCollectionVeiwHeight(int? count)
        {
           AnswerListCollectionView.HeightRequest = 60 * (int)count;
        }

        private void SetupQuestion(Question model)
        {
            if (QuizLayoutStyle == QuizAnswersLayout.InLine)
            {
                // Quiz Layout Style - ANSWER INLINE

                SeparateAnswerQuestionLabel.IsVisible = false;
                AnswersHeaderLabel.IsVisible = false;
                AnswerListCollectionView.IsVisible = false;

                InlineAnswerQuestionLayout.IsVisible = true;
                InlineAnswerQuestionLayout.Children.Clear();

                foreach (var answer in model.QuizAnswers)
                {
                    // Paragraph Question Render

                    // Render the prefix elements
                    if (!string.IsNullOrEmpty(answer.AnswerPrefix))
                    {
                        var prefixToRender = answer.AnswerPrefix;
                        if (Regex.IsMatch(prefixToRender, "\\d+\\.\\s"))
                        {
                            // Remove numbering (Used later)
                            prefixToRender = Regex.Replace(prefixToRender, "\\d+\\.\\s", "");
                            // Remove line breaks (Inline layout doesn't require)
                            prefixToRender = prefixToRender.Replace("\\n", "");
                        }

                        // Position each word in its own Label element
                        foreach (var word in prefixToRender.Split(" "))
                        {
                            if (string.IsNullOrWhiteSpace(word))
                               continue; // ignore white space text
                            
                            InlineAnswerQuestionLayout.Children.Add(new Label()
                            {
                                Style = (Style)this.Resources["ValueLabel"],
                                FontFamily = Application.Current.Resources["FontFamilySemiBold"] as string,
                                FontAttributes = FontAttributes.None,
                                FontSize = 24,
                                Margin = new Thickness(2, 0, 0, 2),
                                VerticalTextAlignment = TextAlignment.Center,
                                LineBreakMode = LineBreakMode.NoWrap,
                                Text = word + " ",
                            });
                        }
                    }

                    // Populate the the Answer Field
                    var grid = new Grid();
                    var entry = new Entry()
                    {
                        FontFamily = Application.Current.Resources["FontFamilySemiBold"] as string,
                        FontAttributes = FontAttributes.None,
                        BackgroundColor = (Color)Application.Current.Resources["EntryBackgroundColor"],
                        Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeCharacter),
                        FontSize = 22,
                        HeightRequest = 45,
                        Margin = new Thickness(0, 2, 0, 2),
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.Start,
                        BindingContext = answer,
                    };
                    entry.SetBinding(Entry.TextProperty, nameof(QuizAnswer.UserAnswer));

                    // Set up Trigger for Answer Validation Color and Focus next Entry
                    var setter = new Setter() {Property = Entry.BackgroundColorProperty, Value = Color.FromHex("#ffe6e6")};
                    var trigger = new DataTrigger(typeof(Entry))
                    {
                        Binding = new Binding(nameof(QuizAnswer.IsValid)),
                        Value = false,
                    };
                    trigger.Setters.Add(setter);
                    trigger.ExitActions.Add(new AnswerValidTriggerAction(){ QuizAnswersLayoutStyle = QuizAnswersLayout.InLine });
                    entry.Triggers.Add(trigger);

                    // Set up Behavior for Answer Validation and Submit Button enable
                    entry.Behaviors.Add(new EventToCommandBehavior()
                    {
                        EventName = nameof(entry.Unfocused),
                        Command = AnswerUnfocusedCommand,
                    });

                    // Check for Numbering
                    if (Regex.IsMatch(answer.AnswerPrefix, "\\d+\\.\\s"))
                    {
                        // Numbering detected, then render it
                        var match = Regex.Match(answer.AnswerPrefix, "\\d+\\.\\s");
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.15f, GridUnitType.Star) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.85f, GridUnitType.Star) });
                        grid.Children.Add(new Label()
                        {
                            Style = (Style)this.Resources["ValueLabel"],
                            FontFamily = Application.Current.Resources["FontFamilySemiBold"] as string,
                            FontAttributes = FontAttributes.None,
                            FontSize = 24,
                            Margin = new Thickness(2, 0, 0, 2),
                            VerticalTextAlignment = TextAlignment.Center,
                            LineBreakMode = LineBreakMode.NoWrap,
                            Text = match.Value + " ",
                        }, 0, 0);
                        grid.Children.Add(entry, 1, 0);
                        grid.ColumnSpacing = 0;
                    }
                    else
                    {
                        // No Numbering available 
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
                        grid.Children.Add(entry, 0, 0);
                    }

                    FlexLayout.SetBasis(grid, new FlexBasis(0.95f, true)); // Fill up the FlexLayout 95% width
                    InlineAnswerQuestionLayout.Children.Add(grid);

                    // Render the suffix elements
                    if (!string.IsNullOrEmpty(answer.AnswerSuffix) 
                        && !string.IsNullOrWhiteSpace(answer.AnswerSuffix))
                    {
                        // Remove line breaks (Inline layout doesn't require)
                        var suffixToRender = answer.AnswerSuffix;
                        suffixToRender = suffixToRender.Replace("\\n", "");

                        // Position each word in its own Label element
                        foreach (var word in suffixToRender.Split(" "))
                        {
                            if (string.IsNullOrWhiteSpace(word))
                                continue; // ignore white space text

                            InlineAnswerQuestionLayout.Children.Add(new Label()
                            {
                                Style = (Style)this.Resources["ValueLabel"],
                                FontFamily = Application.Current.Resources["FontFamilySemiBold"] as string,
                                FontAttributes = FontAttributes.None,
                                FontSize = 24,
                                Margin = new Thickness(2, 0, 0, 2),
                                VerticalTextAlignment = TextAlignment.Center,
                                LineBreakMode = LineBreakMode.NoWrap,
                                Text = word + " ",
                            });
                        }
                    }
                }
            }
            else
            {
                // Quiz Layout Style - ANSWER SEPARATE

                InlineAnswerQuestionLayout.IsVisible = false;

                SeparateAnswerQuestionLabel.IsVisible = true;
                SeparateAnswerQuestionLabel.FormattedText = new FormattedString();
                AnswersHeaderLabel.IsVisible = true;
                AnswerListCollectionView.IsVisible = true;
                BindableLayout.SetItemsSource(AnswerListCollectionView, ((Question)BindingContext).QuizAnswers);

                foreach (var answer in model.QuizAnswers)
                {
                    // Separate Answer Question Render

                    // Render the prefix elements
                    if (!string.IsNullOrEmpty(answer.AnswerPrefix))
                        SeparateAnswerQuestionLabel.FormattedText.Spans.Add(
                            new Span { FontSize = 24, Text = answer.AnswerPrefix.Replace("\\n", "\n"), });

                    SeparateAnswerQuestionLabel.FormattedText.Spans.Add(
                        new Span
                        {
                            GestureRecognizers =
                            {
                                new TapGestureRecognizer
                                {
                                    Command = AnswerTappedCommand, CommandParameter = answer.AnswerIndex
                                }
                            },
                            FontSize = 10,
                            TextColor = (Color)Application.Current.Resources["BlueColor"],
                            FontAttributes = FontAttributes.Bold,
                            Text = answer.AnswerIndex.ToString()
                        });

                    SeparateAnswerQuestionLabel.FormattedText.Spans.Add(
                        new Span
                        {
                            GestureRecognizers =
                            {
                                new TapGestureRecognizer
                                {
                                    Command = AnswerTappedCommand, CommandParameter = answer.AnswerIndex
                                }
                            },
                            Text = new string('_', answer.AnswerLength),
                            FontSize = 24,
                            TextColor = (Color)Application.Current.Resources["BlueColor"],
                            FontAttributes = FontAttributes.Bold,
                        });

                    // Render the suffix elements
                    SeparateAnswerQuestionLabel.FormattedText.Spans.Add(
                        new Span
                        {
                            FontSize = 24,
                            Text = !string.IsNullOrEmpty(answer.AnswerSuffix)
                                ? answer.AnswerSuffix
                                : answer.AnswerIndex == model.QuizAnswers.Count
                                    ? ""
                                    : " ",
                        });
                }
            }
        }

        private void AnswerTapped(int index)
        {
            // Why a foreach you may ask?
            // well check out the logical children i would reply
            AnswerListCollectionView.Children
                .Where(entry => ((Entry)entry).Placeholder == index.ToString())
                 .ForEach(entry => ((Entry)entry).Focus());
        }

        private void TriggerOnAppearing()
        {
            // Set the Keyboard focus on the first Entry element
            if (QuizLayoutStyle == QuizAnswersLayout.InLine)
            {
                var firstChildGrid = (Grid)InlineAnswerQuestionLayout.Children.First(x => x.GetType() == typeof(Grid));
                var firstChildEntry = (Entry)firstChildGrid.Children.First(x => x.GetType() == typeof(Entry));
                firstChildEntry.Focus();
            }
            else
            {
                ((Entry)AnswerListCollectionView.Children.First()).Focus();
            }
        }
    }
}
