using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorFluentUI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using SwiftCAPS.Admin.Web.Shared.Clients;
using SwiftCaps.Models.Models;
using SwiftCAPS.Admin.Web.Shared.Models;
using System.Windows.Input;
using SwiftCaps.Models.Enums;
using System.Net;
using SwiftCAPS.Admin.Web.Shared.States;

namespace SwiftCAPS.Admin.Web.Client.Pages
{
    [Authorize]
    public partial class Index
    {
        [Inject] public IAdminQuizClient QuizClient { get; set; }
        [Inject] public UserState UserState { get; set; }

        private readonly List<DetailsRowColumn<QuizSummaryItem>> _columns = new List<DetailsRowColumn<QuizSummaryItem>>();
        private bool _isPanelOpen;
        private Quiz _selectedQuiz;
        private Selection<QuizSummaryItem> _selectedQuizSummary;
        private bool _isBusy = false;
        private string _quizListError;
        private bool _showErrorMessage => !string.IsNullOrEmpty(_quizListError);
        private bool _canShowGrid => !_isBusy && string.IsNullOrEmpty(_quizListError);
        public List<QuizSummaryItem> Quizzes;
        private ICommand _buttonCommand;
        private List<CommandBarItem> _commandItems;

        private CommandItem _currentCommand;

        private PanelType _panelType = PanelType.LargeFixed;

        public Guid? EditRecordId = null;

        
        public string _PanelType { get; set; }
        private DeleteDisplayMode _deleteDisplayMode = DeleteDisplayMode.Content;

        private bool _displayPanelBackButton = false;

        public Selection<QuizSummaryItem> SelectedQuizSummary
        {
            get => _selectedQuizSummary;
            set
            {
                _selectedQuizSummary = value;
            }
        }

        private async Task OnQuizItemClick(QuizSummaryItem item)
        {
            _isPanelOpen = true;
            _panelType = PanelType.LargeFixed;
            _currentCommand = CommandItem.None;
            // Load Quiz details 
            await OnSelectionChanged(SelectedQuizSummary);
        }

        private async Task OpenEditQuizHandler()
        {
            _displayPanelBackButton = true;
            _panelType = PanelType.Medium;
            EditRecordId = SelectedQuizSummary?.SelectedItems?.First()?.Id;
            _currentCommand = CommandItem.Edit;
        }

        private async Task OpenDeleteQuizInvoke()
        {
            _displayPanelBackButton = true;
            _panelType = PanelType.SmallFixedFar;
            EditRecordId = SelectedQuizSummary?.SelectedItems?.First()?.Id;
            _currentCommand = CommandItem.Delete;
        }

        private async Task ChangePanelTypeHandler(Object panelType)
        {
            _displayPanelBackButton = true;
            _panelType = ((PanelType)panelType);
        }

        private async Task<bool> DeletePanelCloseHandler()
        {
            try
            {
                var selectedId = SelectedQuizSummary?.SelectedItems?.FirstOrDefault()?.Id;
                if (selectedId != null)
                {
                    var response = await QuizClient.DeleteQuiz(selectedId.Value);
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        _displayPanelBackButton = false;
                        StateHasChanged();
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        protected override async Task OnInitializedAsync()
        {
            AddColumns();
            ConfigureCommandBar();
            SelectedQuizSummary = new Selection<QuizSummaryItem>();
            await LoadQuizzes();

            SelectedQuizSummary.SelectionChanged.Subscribe(m =>
            {
                InvokeAsync(StateHasChanged);
            });
        }

        public void ConfigureCommandBar()
        {
            _buttonCommand = new RelayCommand((item) =>
            {
                if (item != null)
                {
                    CommandItemClick((string)item);
                    EditRecordId = null;
                    StateHasChanged();
                }
            });

            _commandItems = new List<CommandBarItem> {
                new CommandBarItem() {  Text= "Add Quiz", IconName="Add", Key=CommandItem.Add.ToString()},
                new CommandBarItem() {  Text= "Edit Quiz", Disabled=true, IconName="Edit", Key=CommandItem.Edit.ToString()},
                new CommandBarItem() {  Text= "Delete Quiz",Disabled=true, IconName="Delete", Key=CommandItem.Delete.ToString()}
            };
        }

        private async Task OnSelectionChanged(Selection<QuizSummaryItem> selection)
        {
            if (selection.SelectedItems.Any())
            {
                var selectedSummary = selection.SelectedItems.First();

                var response = await QuizClient.GetQuiz(selectedSummary.Id);
                if (!response.IsOK())
                    throw new Exception(string.Join(',', response.Errors));

                _selectedQuiz = response.Data;

                _currentCommand = CommandItem.Detail;
            }
        }

        private void CommandItemClick(string item)
        {
            if (item == CommandItem.Delete.ToString())
            {
                _isPanelOpen = true;
                _currentCommand = CommandItem.Delete;
                _panelType = PanelType.SmallFixedFar;
            }
            if (item == CommandItem.Add.ToString())
            {
                _isPanelOpen = true;
                _currentCommand = CommandItem.Add;
                _panelType = PanelType.Medium;
            }
            if (item == CommandItem.Edit.ToString())
            {
                _isPanelOpen = true;
                _currentCommand = CommandItem.Edit;
                _panelType = PanelType.Medium;
            }
        }

        private async Task ReloadSelectedQuiz()
        {
            _displayPanelBackButton = false; 
            await ReloadQuiz();
        }

        private async Task ReloadQuiz()
        {
            _currentCommand = CommandItem.None;
            _panelType = PanelType.LargeFixed;
            await OnSelectionChanged(_selectedQuizSummary);
        }

        private async Task LoadQuizzes()
        {
            try
            {
                _isBusy = true;
                _quizListError = string.Empty;
                var response = await QuizClient.GetQuizzes();
                if (!response.IsOK())
                {
                    _quizListError = "Error loading quizzes. please try again.";
                    Quizzes = new List<QuizSummaryItem>();
                }
                else
                    Quizzes = MapQuizzes(response.Data);
            }
            catch
            {
                _quizListError = "Error loading quizzes. please try again.";
            }
            finally
            {
                _isBusy = false;
            }
        }

        private void AddColumns()
        {
            _columns.Add(new DetailsRowColumn<QuizSummaryItem>("Title", x => x.Title) { MinWidth = 30, MaxWidth = 500, Index = 0, OnColumnClick = OnTitleColumnClick, IsResizable = true });
            _columns.Add(new DetailsRowColumn<QuizSummaryItem>("Sections", x => x.Sections) { Index = 1, MinWidth = 30, MaxWidth = 50, IsResizable = true });
            _columns.Add(new DetailsRowColumn<QuizSummaryItem>("Questions", x => x.Questions) { Index = 2, MinWidth = 30, MaxWidth = 50, IsResizable = true });
            _columns.Add(new DetailsRowColumn<QuizSummaryItem>("Last updated", x => x.LastUpdated) { Index = 3, MaxWidth = 80, IsResizable = true });
            _columns.Add(new DetailsRowColumn<QuizSummaryItem>("Schedules", x => x.Schedules) { Index = 4, MaxWidth = 80, IsResizable = true });

        }

        private void OnTitleColumnClick(IDetailsRowColumn<QuizSummaryItem> column)
        {
            if (column.IsSortedDescending && column.IsSorted)
                column.IsSorted = !column.IsSorted;

            if (column.IsSorted)
            {
                
                Quizzes = new List<QuizSummaryItem>(column.IsSorted ? Quizzes.OrderBy(x => x.Title) : Quizzes.OrderByDescending(x => x.Title));
                column.IsSortedDescending = !column.IsSortedDescending;
            }
            else
            {
                Quizzes = new List<QuizSummaryItem>(column.IsSorted ? Quizzes.OrderBy(x => x.Title) : Quizzes.OrderByDescending(x => x.Title));
                column.IsSorted = !column.IsSorted;
                column.IsSortedDescending = false;
            }
            StateHasChanged();
        }

        private async Task OnPanelDismiss()
        {
            UserState.ActiveQuizDetailTab = DetailsPivotType.General.ToString();
            UserState.ActiveQuizDetailSection = 1;
            _displayPanelBackButton = _isPanelOpen = false;
            _currentCommand = CommandItem.None;
            await LoadQuizzes();
        }

        public void Dispose()
        {
            UserState.OnChange -= StateHasChanged;
        }

        private async Task OnPanelOpened()
        {
            //if want perform any action on open
        }

        private List<QuizSummaryItem> MapQuizzes(IList<QuizSummary> quizzes)
        {
            var list = new List<QuizSummaryItem>();

            foreach (var quiz in quizzes)
            {
                var groupText = quiz.Groups == 1 ? "group" : "groups";

                list.Add(new QuizSummaryItem
                {
                    Id = quiz.Id,
                    LastUpdated = (quiz.LastUpdated == null) ? "" : quiz.LastUpdated?.ToString("dd/MM/yyyy"),
                    Questions = quiz.Questions,
                    Sections = quiz.Sections,
                    Title = quiz.Title,
                    Schedules = quiz.Schedules
                });
            }

            return list.ToList();
        }

        private string GetMarkup(string markdown)
        {
            return Markdig.Markdown.ToHtml(markdown);
        }
    }
}
