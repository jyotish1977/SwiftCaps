using System;
using System.Reflection;
using Newtonsoft.Json;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Models.Models;
using Xamariners.Core.Common;
using Xamariners.Core.Common.Enum;
using Xamariners.Core.Common.Json;
using Xamariners.Core.Common.Model;
using Xamariners.Core.Model.Enums;
using Xamariners.Core.Model.Interface;

namespace SwiftCaps.Client.Core.Services.Infrastructure
{
    public class ClientState : ISwiftCapsClientState
    {
        public ClientState()
        {
            Credential = new Credential();
        }

        public Guid StateId { get; set; }
        public AppStatus AppStatus { get; set; } = AppStatus.Running;
        public bool DeviceIsRegistered { get; set; }
        public bool IsAuthenticated => Credential?.AuthToken != null;
        public bool IsNotificationGranted { get; set; }
        public bool IsPushNotificationSubscribed { get; set; }
        public bool IsRememberMyMobileNumber { get; set; }
        public DateTime CacheLastUpdated { get; set; }
        public DateTime LastLogoutTime { get; set; }
        public DeviceType DeviceType { get; set; }
        public Guid DeviceId { get; set; }

        [JsonConverter(typeof(ConcreteConverter<User>))]
        public IMember Member { get; set; } = new User();

        public ObjectType DataUpdateTargetType { get; set; }
        public PushNotification PushNotification { get; set; }
        public string ApiInfo { get; set; }
        public string AppConfiguration { get; set; }
        public string CountryCode { get; set; }
        public string CurrentSearchPhrase { get; set; }
        public string DeviceInstallation { get; set; }
        public string DeviceToken { get; set; }

        public Credential Credential { get; private set; }

        public string AppDataPath { get; set; }

        // we no longer need this
        public void Clear()
        {
            foreach (var prop in GetType().GetProperties())
                prop.SetValue(this,
                    prop.PropertyType.GetTypeInfo().IsValueType ? Activator.CreateInstance(prop.PropertyType) : null);

            LastLogoutTime = DateTime.MinValue;
            CacheLastUpdated = DateTime.MinValue;

            Credential = new Credential();
            Member = new User();
        }

        public void SetDeviceToken(string deviceToken, DeviceType deviceType)
        {
            throw new NotImplementedException();
        }

    }
}
