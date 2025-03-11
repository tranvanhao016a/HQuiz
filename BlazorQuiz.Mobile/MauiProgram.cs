using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using BlazorQuiz.Mobile.Services;
using BlazorQuiz.Shared;
using BlazorQuiz.Shared.Components.Auth;
using BlazorQuiz.Web.Apis;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using BlazorQuiz.Shared;
using Refit;
#if ANDROID
using Xamarin.Android.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
#endif
#if IOS
using Security;
#endif

namespace BlazorQuiz.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<QuizAuthStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<QuizAuthStateProvider>());
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddAuthorizationCore();

        builder.Services.AddSingleton<IStorageService, StorageService>()
            .AddSingleton<IAppState, AppState>();
        ConfigureRefit(builder.Services);

        return builder.Build();
    }

    private static readonly string ApiBaseUrl = DeviceInfo.Platform == DevicePlatform.Android
                        ? "https://10.0.2.2:7270"
                        : "https://localhost:7270";

    static void ConfigureRefit(IServiceCollection services)
    {
        services.AddRefitClient<IAuthApi>(GetRefitSettings)
            .ConfigureHttpClient(SetHttpClient)
            .ConfigurePrimaryHttpMessageHandler(GetHttpMessageHandler);

        static void SetHttpClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri(ApiBaseUrl);
        }

        static HttpMessageHandler GetHttpMessageHandler()
        {
#if ANDROID
            var androidMessageHandler = new AndroidClientHandler
            {
                ServerCertificateCustomValidationCallback = (HttpRequestMessage requestMessage, X509Certificate2? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors) =>
                {
                    return certificate?.Issuer == "CN=localhost" || sslPolicyErrors == SslPolicyErrors.None;
                }
            };
            return androidMessageHandler;
#elif IOS
            var nsUrlSessionHandler = new NSUrlSessionHandler
            {
                TrustOverrideForUrl = (NSUrlSessionHandler sender, string url, SecTrust trust) => url.StartsWith("https://localhost")
            };
            return nsUrlSessionHandler;
#else
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
#endif
        }

        static RefitSettings GetRefitSettings(IServiceProvider sp)
        {
            var authStateProvider = sp.GetRequiredService<QuizAuthStateProvider>();
            return new RefitSettings
            {
                AuthorizationHeaderValueGetter = (_, __)
                => Task.FromResult(
                    authStateProvider.User?.Token ?? "")
            };
        }
    }
}
