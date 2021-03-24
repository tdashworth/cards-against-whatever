using CardsAgainstWhatever.Shared.Interfaces;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using SignalR.Strong;
using System;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Client.Stores.Server
{
    public class ServerEffects
    {
        private readonly NavigationManager NavigationManager;
        private readonly IServiceProvider ServiceProvider;

        public ServerEffects(NavigationManager navigationManager, IServiceProvider serviceProvider)
        {
            NavigationManager = navigationManager;
            ServiceProvider = serviceProvider;
        }

        [EffectMethod]
        public async Task Handle(ConnectAction action, IDispatcher dispatcher)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri($"/gamehub"))
                .WithAutomaticReconnect()
                .Build();

            //var server = new GameServerProxy(connection);
            var server = connection.AsGeneratedHub<IGameServer>();

            var gameClient = ServiceProvider.GetService(typeof(IGameClient));
            connection.RegisterSpoke<IGameClient>(gameClient);

            connection.Closed += (Exception ex) => Task.Run(() => dispatcher.Dispatch(new DisconnectedEvent()));
            connection.Reconnected += (string id) => Task.Run(() => dispatcher.Dispatch(new ReconnectedEvent()));
            connection.Reconnecting += (Exception ex) => Task.Run(() => dispatcher.Dispatch(new ReconnectingEvent()));

            await connection.StartAsync();

            dispatcher.Dispatch(new ConnectedEvent(connection, server));
        }
    }
}
