using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using SwiftCaps.Client.Cache.Service.Interfaces;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Services.Interfaces;
using static SwiftCaps.Values.Constants;
using SwiftCaps.Views;
using Xamariners.Core.Common.Helpers;
using Xamariners.Core.Interface;
using Xamariners.Mobile.Core.Exceptions;
using Xamariners.Mobile.Core.Helpers.MVVM;
using Xamariners.Mobile.Core.Infrastructure;
using Xamariners.Mobile.Core.Interfaces;
using Xamariners.Mvvm;
using Unity;
using Xamarin.Forms;
using Xamariners.Mobile.Core.Helpers;

namespace SwiftCaps.ViewModels
{
    public abstract partial class ViewModelBase : ValidatableBindableBase
    {

        protected IPopupInputService _popupInputService => Client.Bootstrap.BootStrapper.Container.Resolve<IPopupInputService>();

        public IErrorService _errorService => Client.Bootstrap.BootStrapper.Container.Resolve<IErrorService>();

        protected ILogger _logger => Client.Bootstrap.BootStrapper.Container.Resolve<ILogger>();

        protected IAuthService _authService => Client.Bootstrap.BootStrapper.Container.Resolve<IAuthService>();

        public bool IsValid { get; set; }

        /// <summary>
        /// Gets the on error command.
        /// </summary>
        public DelegateCommand<object> OnErrorCommand => new DelegateCommand<object>(OnError);

        protected void OnError(object param)
        {
            var error = (Exception)param;
            _errorService.AddError(error.Message, ViewModelError.ErrorAction.DisplayAndLog, ViewModelError.ErrorSeverity.Error);
        }

        protected virtual bool Validate(Dictionary<string, object> properties = null, Dictionary<string, string> propertiesDisplayName = null, List<Tuple<string, bool, string>> validators = null)
        {
            // clean any inline val property
            _errorService.ClearInlineValidators();

            if (properties?.Count > 0)
            {
                if (ValidatePropertyList(properties, propertiesDisplayName, validators))
                    return true;
            }
            else
            {
                var isValid = ValidateProperties(null, validators);

                if (isValid)
                    return true;
            }

            string formattedErrors = "";
            var errors = GetAllErrors();

            // invalid but no errors: move on
            if (!errors.Any())
                return true;

            foreach (var key in errors.Keys)
            {
                // TODO we would need to replace the CamelCase filed key with a globalized name
                // in the meantime, we de-camel case
                //formattedErrors += $"\n\u2022 {key.SplitCamelCase()} : {string.Join("\n", errors[key])}";
                if (errors.Keys.Count == 1)
                {
                    formattedErrors = string.Join("\n", errors[errors.Keys.First()]);
                }
                else
                {
                    formattedErrors += $"\u2022 {string.Join("\n", errors[key])}\n";
                }

                // replace property name with display name
                if (propertiesDisplayName != null)
                    formattedErrors = propertiesDisplayName.Aggregate(formattedErrors, (current, propertyDisplayName) => current.Replace(propertyDisplayName.Key, propertyDisplayName.Value));

                // split camel case
                formattedErrors = formattedErrors.SplitCamelCase();
            }


            // remove the last \n space
            formattedErrors = formattedErrors.ReplaceLast("\n", "");

            _errorService.AddError(formattedErrors, ViewModelError.ErrorAction.Display, ViewModelError.ErrorSeverity.Warning, errors: errors);

            _errorService.ProcessErrors();

            return false;
        }

        protected void HandleUIError(Exception ex, bool hideSpinner = true, ViewModelError.ErrorAction action = ViewModelError.ErrorAction.DisplayAndLog)
        {
            string title = null;
            if (hideSpinner)
                _spinner.HideLoading();

            // We are not authenticated
            if (ex is AuthenticationException)
            {
                Logout();

                return;
            }

            if (ex is HandledException exception && !exception.IsLog)
                action = ViewModelError.ErrorAction.Display;

            if (ex is HandledException exception2)
                title = exception2.Title;

            // hack: we just use this exception to surface messages to ui from any lib without includes
            if (ex is RankException)
                action = ViewModelError.ErrorAction.Display;

            var onPopupClosed = GetErrorAction(ex?.InnerException);

            _errorService.AddError(ex.Message, action, ViewModelError.ErrorSeverity.Error, title, ex: ex, onPopupClosed: onPopupClosed);

            _errorService.ProcessErrors();
        }

        private Action GetErrorAction(Exception ex)
        {
            if (ex is null) return null;

            switch (ex)
            {
                case MsalUiRequiredException msa:
                    return async () =>
                    {
                        await _authService.Logout();
                        ThreadingHelpers.InvokeOnMainThread(() => _navigationService.GoToAsync(ShellNavigation.LoginPagePath));
                    };

                default:
                    return null;
            }
        }
    }
}
