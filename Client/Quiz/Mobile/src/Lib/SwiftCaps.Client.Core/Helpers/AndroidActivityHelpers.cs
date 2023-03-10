using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SwiftCaps.Client.Core.Helpers
{
    /// <summary>
    /// Abstracting this layer separately for Android since the
    /// CrossCurrentActivity plugin doesn't support dot net standard projects
    /// </summary>
    public static class CrossCurrentActivity
    {
        public static object Current { get; set; }
    }
}
