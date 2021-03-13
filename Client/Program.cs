using BlazorComponentBus;
using CardsAgainstWhatever.Client.Services;
using CardsAgainstWhatever.Shared;
using CardsAgainstWhatever.Shared.Interfaces;
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

            builder.Services.AddSingleton(sp => new HubConnectionBuilder()
                .WithUrl(sp.GetService<NavigationManager>().ToAbsoluteUri("/gamehub"))
                .WithAutomaticReconnect()
                .Build());

            builder.Services.AddScoped<ComponentBus>();

            builder.Services.AddScoped<IGameServer, GameServerProxy>();
            builder.Services.AddScoped<IGameClient, GameClientListener>();

            await builder.Build().RunAsync();
        }
    }
}
