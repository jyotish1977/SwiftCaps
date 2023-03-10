using System.Collections.Generic;
using SwiftCaps.Client.Core.Enums;

namespace SwiftCaps.Client.Core.Interfaces
{
    public interface IAppSettings
    {
        string FakeAssemblyName { get; set; }
        string AppName { get; }
        string AppConfig { get; }
        string RaygunApikey { get; }
        string LocaleDefault { get; set; }
        string AppStoreiOS { get; }
        string AppStoreDroid { get; }
        string CuteErrorDescription { get; set; }
        string CureErrorTitle { get; set; }
        string CuteErrorEnabled { get; set; }

        string ApiTimeOut { get; set; }
        string ApiLoginType { get; set; }
        string AppCenterIosKey { get; set; }
        string AppCenterAndroidKey { get; set; }
        string AppCenterLogTag { get; set; }

        // ENDPOINTS
        string ApiEndpoint { get; set; }

        // IDENTITY
        string IdentityClientId { get; set; }
        string IdentityTenantId { get; set; }
        string IdentityScope { get; set; }

        string ApimSubscriptionKey { get; }
        Dictionary<string, string> Headers { get; }
        QuizAnswersLayout QuizAnswersLayout { get; }
    }
}
