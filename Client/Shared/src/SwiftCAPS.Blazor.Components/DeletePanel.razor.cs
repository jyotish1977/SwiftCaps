using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Refit;
using SwiftCaps.Models.Enums;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCAPS.Blazor.Components
{
    public partial class DeletePanel
    {
        [Parameter] public EventCallback OnNavigationBackCallback { get; set; }
        [Parameter] public EventCallback OnCloseCallback { get; set; }
        [Parameter] public Func<Task<HttpResponseMessage>> OnDeleteCallback { get; set; }

        private bool _isPanelOpen;

        private string _entity;

        private bool _showBackButton = false;

        private string _errorMessage;

        private bool _showError;
        private string _deleteLoadingLabel => $"Please don't close until we're finished deleting the {_entity}";

        private enum DeleteDisplayMode
        {
            Content,
            Loading,
            Error,
            Success
        }

        private DeleteDisplayMode _displayMode;

        private async Task PanelDismissHandler()
        {
            _isPanelOpen = false;
            await OnCloseCallback.InvokeAsync();
        }

        async Task OnNavigationBackClick()
        {
            _isPanelOpen = false;
            await OnNavigationBackCallback.InvokeAsync();
        }

        private async Task DeleteClick()
        {
            _displayMode = DeleteDisplayMode.Loading;

            try
            {
                var response = await OnDeleteCallback();

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    _displayMode = DeleteDisplayMode.Success;
                    _showBackButton = false;
                }
                else
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<bool>>(json);
                    _errorMessage = serviceResponse.Errors?.FirstOrDefault();
                    _displayMode = DeleteDisplayMode.Content;
                    _showError = true;
                }
            }
            catch
            {
                _showError = true;
                _displayMode = DeleteDisplayMode.Content;
            }
            finally
            {
                StateHasChanged();
            }

        }

        public async Task Show(string entity, bool showNavigation = false)
        {
            _displayMode = DeleteDisplayMode.Content;
            _showBackButton = showNavigation;
            _errorMessage = string.Empty;
            _showError = false;
            _entity = entity;
            _isPanelOpen = true;
            StateHasChanged();
        }
    }
}
