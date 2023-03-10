using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using SwiftCaps.Client.Core.Interfaces;
using UIKit;

namespace SwiftCAPS.iOS.Services
{
    public class NativeKeyboardVisibilityService : INativeKeyboardVisibilityService
    {
        public bool IsKeyboardVisible { get; private set; }

        public NativeKeyboardVisibilityService()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            UIKeyboard.Notifications.ObserveDidShow(OnKeyboardDidShow);
            UIKeyboard.Notifications.ObserveDidHide(OnKeyboardDidHide);
        }

        private void OnKeyboardDidShow(object sender, EventArgs e)
        {
            IsKeyboardVisible = true;
        }

        private void OnKeyboardDidHide(object sender, EventArgs e)
        {
            IsKeyboardVisible = false;
        }

    }
}
