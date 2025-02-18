using System.Security.Claims;
using System.Text.Json;
using BlazorQuiz.Shared.DTO;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorQuiz.Web.Auth
{
    public class QuizAuthStateProvider : AuthenticationStateProvider
    {
        private const string AuthType = "quiz-auth";
        private const string UserDataKey = "udate";
        private Task<AuthenticationState> _authStateTask;
        public IJSRuntime _jsRuntime;
        public QuizAuthStateProvider(IJSRuntime jSRuntime)
        {
            _jsRuntime = jSRuntime;
            SetAuthStateTask();
        }
        public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authStateTask;

        public LoggedInUser User { get; private set; }
        public bool IsLoggedIn => User?.Id > 0;

        public async Task SetLoginAsync(LoggedInUser user)
        {
            User = user;
            SetAuthStateTask();
            NotifyAuthenticationStateChanged(_authStateTask);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserDataKey, user.ToJson());
        }

        public async Task SetLogoutAsync()
        {
            User = null;
            SetAuthStateTask();
            NotifyAuthenticationStateChanged(_authStateTask);
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", UserDataKey);
        }

        public bool IsInitializing { get; private set; } = true;
        private bool _isInitialized = false;

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            try
            {
                Console.WriteLine("InitializeAsync started");
                var udate = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", UserDataKey);
                if (string.IsNullOrWhiteSpace(udate))
                {
                    Console.WriteLine("No user data found in localStorage");
                    return;
                }

                var user = LoggedInUser.LoadForm(udate);
                if (user == null || user.Id == 0)
                {
                    Console.WriteLine("Invalid user data");
                    return;
                }
                await SetLoginAsync(user);
                Console.WriteLine("User logged in");
            }
            finally
            {
                IsInitializing = false;
                _isInitialized = true;
                Console.WriteLine("InitializeAsync completed");
            }
        }

        private void SetAuthStateTask()
        {
            if (IsLoggedIn)
            {
                var identity = new ClaimsIdentity(User.ToClaims(), AuthType);
                var principal = new ClaimsPrincipal(identity);
                var authState = new AuthenticationState(principal);
                _authStateTask = Task.FromResult(authState);
            }
            else
            {
                var identity = new ClaimsIdentity();
                var principal = new ClaimsPrincipal(identity);
                var authState = new AuthenticationState(principal);
                _authStateTask = Task.FromResult(authState);
            }
        }
    }
}
