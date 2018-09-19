using AlmVR.Client.Core;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlmVR.Client.Providers.SignalR
{
    public abstract class ClientBase : IClient
    {
        private readonly Timer pingTimer;
        private bool initialized;

        protected HubConnection Connection { get; private set; }
        protected Action<string> Log { get; private set; }
        protected string Path { get; private set; }

        protected ClientBase(string path, Action<string> log)
        {
            Log = log;
            this.Path = path;
            pingTimer = new Timer(x => PingAsync().Wait());
        }

        public async Task ConnectAsync(string hostName, int port)
        {
            Connection = new HubConnectionBuilder()
                .WithUrl($"http://{hostName}:{port}/{Path}")
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .Build();

            OnConnectionCreated();

            await Connection.StartAsync();

            initialized = true;

            pingTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        public virtual async void Dispose()
        {
            pingTimer.Dispose();
            await Connection.DisposeAsync();
        }

        public Task PingAsync()
        {
            if (!initialized)
                return Task.FromResult(0);

            return Connection.InvokeAsync("Ping");
        }

        protected virtual void OnConnectionCreated() { }
    }
}
