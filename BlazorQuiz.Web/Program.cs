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
builder.Services.AddSingleton<AuthenticationStateProvider>(
    sp=> sp.GetRequiredService<QuizAuthStateProvider>());

builder.Services.AddAuthorizationCore();
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


ConfigureRefit(builder.Services);

await builder.Build().RunAsync();

static void ConfigureRefit(IServiceCollection services)
{
    const string ApiBaseUrl = "https://localhost:7270";
    services.AddRefitClient<IAuthApi>()
        .ConfigureHttpClient(httpClient => 
        httpClient.BaseAddress = new Uri(ApiBaseUrl));
}
