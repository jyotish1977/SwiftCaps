using SwiftCaps.Infrastructure;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace SwiftCaps.Views
{
    [Preserve]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuizListPage : BaseContentPage
    {
        public QuizListPage()
        {
            InitializeComponent();
        }
    }
}