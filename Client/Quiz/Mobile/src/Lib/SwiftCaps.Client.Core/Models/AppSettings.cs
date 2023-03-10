using System;
using System.Collections.Generic;
using CommonServiceLocator;
using PCLAppConfig;
using SwiftCaps.Client.Core.Enums;
using SwiftCaps.Client.Core.Interfaces;

namespace SwiftCaps.Client.Core.Models
{
    public class AppSettings : IAppSettings
    {
        public AppSettings()
        {
            // GENERAL
            AppName = ConfigurationManager.AppSettings["app.name"];
            AppConfig = ConfigurationManager.AppSettings["app.config"];
            RaygunApikey = ConfigurationManager.AppSettings["raygun.apikey"];
            LocaleDefault = ConfigurationManager.AppSettings["locale.default"];
            AppStoreiOS = ConfigurationManager.AppSettings["app.store.ios"];
            AppStoreDroid = ConfigurationManager.AppSettings["app.store.droid"];

            // QUIZ LAYOUT STYLE
            QuizAnswersLayout = ConfigurationManager.AppSettings["quiz.answers.layout"]
                              == "inline" ? QuizAnswersLayout.InLine : QuizAnswersLayout.Separate;

            //APP CENTRE
            AppCenterLogTag = ConfigurationManager.AppSettings["appcenter.logtag"];
            AppCenterAndroidKey = ConfigurationManager.AppSettings["appcenter.androidkey"];
            AppCenterIosKey = ConfigurationManager.AppSettings["appcenter.ioskey"];

            // ENDPOINTS
            ApiEndpoint = ConfigurationManager.AppSettings["api.endpoint"];

            ApimSubscriptionKey = ConfigurationManager.AppSettings["apim.subscriptionkey"];

            ApiLoginType = ConfigurationManager.AppSettings["api.logintype"];
            ApiTimeOut = ConfigurationManager.AppSettings["api.timeout"];
            
            // CUTE ERROR
            CuteErrorEnabled = ConfigurationManager.AppSettings["cuteerror.enabled"];
            CureErrorTitle = ConfigurationManager.AppSettings["cuteerror.title"];
            CuteErrorDescription = ConfigurationManager.AppSettings["cuteerror.description"];

            //FAKE
            FakeAssemblyName = ConfigurationManager.AppSettings["fakeassembly.name"];

            //IDENTITY
            IdentityClientId = ConfigurationManager.AppSettings["identity.client.id"];
            IdentityTenantId = ConfigurationManager.AppSettings["identity.tenant.id"];
            IdentityScope = ConfigurationManager.AppSettings["identity.scope"];

            Headers = new Dictionary<string, string>
            {
                { "Ocp-Apim-Subscription-Key", ApimSubscriptionKey },
            };
        }

        public string FakeAssemblyName { get; set; }
        public string AppName { get; }
        public string AppConfig { get; }
        public string RaygunApikey { get; }
        public string LocaleDefault { get; set; }
        public string AppStoreiOS { get; }
        public string AppStoreDroid { get; }
        public string CuteErrorDescription { get; set; }
        public string CureErrorTitle { get; set; }
        public string CuteErrorEnabled { get; set; }
        public string ApiEndpoint { get; set; }
        public string ApiTimeOut { get; set; }
        public string ApiLoginType { get; set; }
        public string AppCenterIosKey { get; set; }
        public string AppCenterAndroidKey { get; set; }
        public string AppCenterLogTag { get; set; }
        public string IdentityClientId { get; set; }
        public string IdentityTenantId { get; set; }      
        public string IdentityScope { get; set; }

        public string ApimSubscriptionKey { get; private set; }
        public Dictionary<string, string> Headers { get; private set; }

        public QuizAnswersLayout QuizAnswersLayout { get; private set; }
    }
}
