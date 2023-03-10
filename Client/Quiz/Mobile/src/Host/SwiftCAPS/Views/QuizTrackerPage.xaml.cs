using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwiftCaps.Infrastructure;
using SwiftCaps.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SwiftCaps.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuizTrackerPage : BaseContentPage
    {
        public QuizTrackerPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is QuizTrackerPageViewModel quizTrackerPageViewModel)
            {
                quizTrackerPageViewModel.GetLeaderBoardCommand.Execute();
            }
        }
    }
}