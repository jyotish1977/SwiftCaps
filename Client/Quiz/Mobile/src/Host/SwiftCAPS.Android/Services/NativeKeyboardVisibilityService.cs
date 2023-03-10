using Android.Content;
using Android.Views.InputMethods;
using Plugin.CurrentActivity;
using SwiftCaps.Client.Core.Interfaces;

namespace SwiftCAPS.Droid.Services
{
    public class NativeKeyboardVisibilityService : INativeKeyboardVisibilityService
    {
        public bool IsKeyboardVisible
        {
            get
            {
                var inputMethodService = (InputMethodManager)
                    CrossCurrentActivity.Current.Activity.GetSystemService(Context.InputMethodService);
                return inputMethodService is {IsAcceptingText: true};
            }
        }
    }
}