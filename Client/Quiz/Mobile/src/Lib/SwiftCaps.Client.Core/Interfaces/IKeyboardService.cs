using System;
using System.Collections.Generic;
using System.Text;

namespace SwiftCaps.Client.Core.Interfaces
{
    /// <summary>
    /// Provides native Keyboard visibility status in mobile phone
    /// </summary>
    public interface INativeKeyboardVisibilityService
    {
        bool IsKeyboardVisible { get; }
    }
}
