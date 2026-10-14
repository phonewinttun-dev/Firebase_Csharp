using Microsoft.JSInterop;

namespace Firebase_Csharp.Client.Services
{
    /// <summary>
    /// Strongly-typed JS interop wrapper managing the collocated Home.razor.js module.
    /// </summary>
    public sealed class FirebaseInterop : IAsyncDisposable
    {
        internal const string ModulePath = "./Pages/Home.razor.js";
        internal const string InitMethod = "initialize";
        internal const string RequestTokenMethod = "requestTokenAndListen";
        internal const string CopyToClipboardMethod = "copyToClipboard";
        internal const string DisposeMethod = "dispose";

        private readonly IJSRuntime _js;
        private IJSObjectReference? _module;

        public FirebaseInterop(IJSRuntime js) => _js = js ?? throw new ArgumentNullException(nameof(js));

        private async ValueTask<IJSObjectReference> GetModuleAsync()
            => _module ??= await _js.InvokeAsync<IJSObjectReference>("import", ModulePath);

        /// <summary>
        /// Initializes the JS client manager with a DotNetObjectReference and retrieves saved config from localStorage.
        /// </summary>
        public async ValueTask<string> InitializeAsync<T>(DotNetObjectReference<T> dotNetRef) where T : class
        {
            var module = await GetModuleAsync();
            return await module.InvokeAsync<string>(InitMethod, dotNetRef);
        }

        /// <summary>
        /// Requests browser notification permission, retrieves FCM token, and begins listening for foreground push.
        /// </summary>
        public async ValueTask RequestTokenAndListenAsync(string configJson, string vapidKey)
        {
            var module = await GetModuleAsync();
            await module.InvokeVoidAsync(RequestTokenMethod, configJson, vapidKey);
        }

        /// <summary>
        /// Copies the specified text to clipboard using navigator.clipboard API.
        /// </summary>
        public async ValueTask<bool> CopyToClipboardAsync(string text)
        {
            var module = await GetModuleAsync();
            return await module.InvokeAsync<bool>(CopyToClipboardMethod, text);
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (_module is not null)
                {
                    await _module.InvokeVoidAsync(DisposeMethod);
                    await _module.DisposeAsync();
                }
            }
            catch (JSDisconnectedException)
            {
                // Circuit or page was disconnected; safe to ignore
            }
        }
    }
}
