using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using BlazorQuiz.Shared.DTO;
using Microsoft.AspNetCore.Components;
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
        private readonly NavigationManager _navigationManager;

        public QuizAuthStateProvider(IJSRuntime jSRuntime, NavigationManager navigationManager)
        {
            _jsRuntime = jSRuntime;
            _navigationManager = navigationManager;
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
                var udate = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", UserDataKey);
                if (string.IsNullOrWhiteSpace(udate))
                {
                    RedirectToLogin();
                    return;
                }

                var user = LoggedInUser.LoadForm(udate);
                if (user == null || user.Id == 0)
                {
                    RedirectToLogin();
                    return;
                }

                // Check if JWT token is still valid
                if (!IsTokenValid(user.Token))
                {
                    RedirectToLogin();
                    return;
                }

                await SetLoginAsync(user);
               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during initialization: {ex.Message}");
            }
            finally
            {
                IsInitializing = false;
                _isInitialized = true;
                Console.WriteLine("InitializeAsync completed");
            }
        }

        private void RedirectToLogin()
        {
           _navigationManager.NavigateTo("auth/login");
        }

        private static bool IsTokenValid(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var jwtHandler = new JwtSecurityTokenHandler();
            if (!jwtHandler.CanReadToken(token))
                return false;

            var jwt = jwtHandler.ReadJwtToken(token);
            var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
            if (expClaim == null)
                return false;

            var expTime = long.Parse(expClaim.Value);
            var expDateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(expTime).UtcDateTime;

            return expDateTimeUtc > DateTime.UtcNow;
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
