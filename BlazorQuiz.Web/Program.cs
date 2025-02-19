using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorQuiz.Web;
using Refit;
using BlazorQuiz.Web.Apis;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorQuiz.Web.Auth;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddCascadingAuthenticationState();


builder.Services.AddSingleton<QuizAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<QuizAuthStateProvider>());

var app = builder.Build();

// Initialize the authentication state provider
var authStateProvider = app.Services.GetRequiredService<QuizAuthStateProvider>();
await authStateProvider.InitializeAsync();
builder.Services.AddAuthorizationCore();
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

ConfigureRefit(builder.Services);

await builder.Build().RunAsync();

static void ConfigureRefit(IServiceCollection services)
{
    const string ApiBaseUrl = "https://localhost:7270";
    services.AddRefitClient<IAuthApi>()
        .ConfigureHttpClient(SetHttpClient);

    services.AddRefitClient<ICategoryApi>(GetRefitSettings)
         .ConfigureHttpClient(SetHttpClient);


    services.AddRefitClient<IQuizApi>(GetRefitSettings)
         .ConfigureHttpClient(SetHttpClient);

    static void SetHttpClient(HttpClient httpClient)
    => httpClient.BaseAddress = new Uri(ApiBaseUrl);

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
