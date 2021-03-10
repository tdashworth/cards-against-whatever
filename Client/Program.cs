using BlazorComponentBus;
using CardsAgainstWhatever.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSingleton(sp => new HubConnectionBuilder()
                .WithUrl(sp.GetService<NavigationManager>().ToAbsoluteUri("/gamehub"))
                .WithAutomaticReconnect()
                .Build());
            builder.Services.AddScoped<ComponentBus>();

            await builder.Build().RunAsync();
        }
    }
}
