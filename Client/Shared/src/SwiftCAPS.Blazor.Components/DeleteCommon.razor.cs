using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SwiftCaps.Models.Enums;

namespace SwiftCAPS.Blazor.Components
{
    public partial class DeleteCommon
    {
        [Parameter]
        public string Entity { get; set; }

        [Parameter]
        public Func<Task<bool>> OnDeleteCallback { get; set; }

        [Parameter]
        public EventCallback OnCloseCallback { get; set; }

        private enum DeleteDisplayMode
        {
            Content,
            Loading,
            Error,
            Success
        }

        private DeleteDisplayMode _displayMode;

        private string _deleteLoadingLabel => $"Please don't close until we're finished deleting the {Entity}";

        private async Task DeleteClick()
        {
            try
            {
                _displayMode = DeleteDisplayMode.Loading;
                var result = await OnDeleteCallback();
                if (result)
                {
                    _displayMode = DeleteDisplayMode.Success;
                }
                else
                {
                    _displayMode = DeleteDisplayMode.Error;
        }
                StateHasChanged();
            }
            catch
        {
                _displayMode = DeleteDisplayMode.Error;
        }
            finally
        {
                StateHasChanged();
            }
        }

        private async Task CloseDeletePanel()
        {
            await OnCloseCallback.InvokeAsync();
        }
    }
}
