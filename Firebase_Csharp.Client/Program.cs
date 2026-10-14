using Firebase_Csharp.Client;
using Firebase_Csharp.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient to target the Firebase_Csharp.Api backend
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7137/") });
builder.Services.AddScoped<FirebaseInterop>();

await builder.Build().RunAsync();
