using System.Threading.Tasks;
using SwiftCaps.Client.Bootstrap;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Mobile.Shared.Services;
using SwiftCaps.Templates;
using Xamarin.Forms;
using Unity;
using Xamariners.Mobile.Core.Interfaces;
using Xamariners.RestClient.Helpers;

namespace SwiftCaps.Triggers
{
    /// <summary>
    /// Executes list of behaviors when swiping
    /// from one question card to another
    /// </summary>
    public class QuestionCardAppearingTrigger : TriggerAction<PanCardView.CarouselView>
    {
        private QuestionCardTemplate _previousQuestionCardTemplate;


        protected override async void Invoke(PanCardView.CarouselView sender)
        {
            var spinner = BootStrapper.Container.Resolve<ISpinner>();
           
            if (!(sender.CurrentView is ContentView selectedElement) ||
                !(selectedElement.Content is QuestionCardTemplate questionCard) ||
                !(spinner is Spinner spinnerImpl))
            {
                return;
            }

            if (_previousQuestionCardTemplate == questionCard)
                return;

            // To ignore if this is receive unfocus event
            _previousQuestionCardTemplate = questionCard;
            
            //RetryHelpers.Retry(() => (sender.Height > 0 && !spinnerImpl.IsSpinning),100, 50);

            var isKeyboardVisible = BootStrapper.Container.Resolve<INativeKeyboardVisibilityService>().IsKeyboardVisible;

            // we don't do anything if the Keyboard is not visible && we don't want the keyboard upon first card unless the keyboard is already visible
            if (!isKeyboardVisible || sender.SelectedIndex == 0)
                return;

            // Execute the OnAppearing event Command to do
            // whatever you want to do up on appearance of view
            questionCard.TriggerOnAppearingCommand.Execute(null);
        }
    }
}
