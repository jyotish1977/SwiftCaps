using System.Threading.Tasks;
using Newtonsoft.Json;
using SwiftCaps.Client.Core.Enums;
using Xamarin.Forms;
using Xamariners.Core.Common.Helpers;

namespace SwiftCaps.Mobile.Shared.Helpers
{
    public static class AppPropertyHelpers
    {

        public static bool ContainsKey(AppProperty key)
        {
            Application myApp = Application.Current;
            return myApp.Properties.ContainsKey(key.ToString());
        }

        public static async Task AddReplace<T>(AppProperty key, T value)
        {
            var myApp = Application.Current;

            bool check = myApp.Properties.ContainsKey(key.ToString());
            if (check)
                myApp.Properties.Remove(key.ToString());

            myApp.Properties.Add(key.ToString(), value);

            await myApp.SavePropertiesAsync().ConfigureAwait(false);
        }

        public static async Task AddReplaceValue<T>(AppProperty key, T value) where T : struct
        {
            var myApp = Application.Current;

            bool check = myApp.Properties.ContainsKey(key.ToString());
            if (check)
                myApp.Properties.Remove(key.ToString());

            myApp.Properties.Add(key.ToString(), value);

            await myApp.SavePropertiesAsync().ConfigureAwait(false);
        }

        public static async Task AddReplaceBinary<T>(AppProperty key, T value) where T : class
        {
            var bytes = SerialiserHelper.ToBytes(value);

            var myApp = Application.Current;

            bool check = myApp.Properties.ContainsKey(key.ToString());
            if (check)
                myApp.Properties.Remove(key.ToString());

            myApp.Properties.Add(key.ToString(), bytes);

            await myApp.SavePropertiesAsync().ConfigureAwait(false);
        }

        public static async Task AddReplaceJson<T>(AppProperty key, T value)
        {
            var myApp = Application.Current;

            bool check = myApp.Properties.ContainsKey(key.ToString());
            if (check)
                myApp.Properties.Remove(key.ToString());

            string json;
            
            json = JsonConvert.SerializeObject(value);
           
            myApp.Properties.Add(key.ToString(), json);
            await myApp.SavePropertiesAsync().ConfigureAwait(false);
        }

        public static T Get<T>(AppProperty key)
        {
            Application myApp = Application.Current;
            bool check = myApp.Properties.ContainsKey(key.ToString());

            if (check)
            {
                var property = myApp.Properties[key.ToString()];
                if (property is null)
                {
                    Clear(key);
                    return default(T);
                }

                return (T)property;
            }

            return default(T);
        }

        public static async Task<T> GetValue<T>(AppProperty key) where T : struct
        {
            var myApp = Application.Current;
            bool check = myApp.Properties.ContainsKey(key.ToString());

            if (check)
            {
                var property = myApp.Properties[key.ToString()];
                return (T)property;
            }

            return default(T);
        }

        public static async Task<T> GetBinary<T>(AppProperty key) where T : class
        {
            var myApp = Application.Current;
            bool check = myApp.Properties.ContainsKey(key.ToString());

            if (check)
            {
                var property = myApp.Properties[key.ToString()];
                if (property is null)
                {
                    await Clear(key);
                    return default(T);
                }

                var result = SerialiserHelper.FromBytes<T>((byte[])property);
                return result;
            }

            return default(T);
        }

        public static async Task<T> GetJson<T>(AppProperty key) where T : class
        {
            var myApp = Application.Current;
            bool check = myApp.Properties.ContainsKey(key.ToString());

            if (check)
            {
                var property = myApp.Properties[key.ToString()] as string;
                if (string.IsNullOrEmpty(property))
                {
                    await Clear(key);
                    return default(T);
                }

                var result = JsonConvert.DeserializeObject<T>(property);
                return result;
            }

            return default(T);
        }

        public static async Task Clear()
        {
            var myApp = Application.Current;

            myApp.Properties.Clear();
            await myApp.SavePropertiesAsync().ConfigureAwait(false);
        }

        public static async Task Clear(AppProperty key)
        {
            var myApp = Application.Current;
            bool check = myApp.Properties.ContainsKey(key.ToString());

            if (check)
            {
                myApp.Properties.Remove(key.ToString());
                await myApp.SavePropertiesAsync().ConfigureAwait(false);
            }

        }

    }
}
