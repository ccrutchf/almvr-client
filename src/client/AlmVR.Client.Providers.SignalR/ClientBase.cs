using AlmVR.Client.Core;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlmVR.Client.Providers.SignalR
{
    public abstract class ClientBase : IClient
    {
        protected HubConnection Connection { get; private set; }
        protected string Path { get; private set; }

        protected ClientBase(string path)
        {
            this.Path = path;
        }

        public Task ConnectAsync(string hostName, int port)
        {
            Connection = new HubConnectionBuilder()
                .WithUrl($"http://{hostName}:{port}/{Path}", HttpTransportType.LongPolling)
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .Build();

            OnConnectionCreated();

            return Connection.StartAsync();
        }

        protected virtual void OnConnectionCreated() { }

        public virtual async void Dispose()
        {
            await Connection.DisposeAsync();
        }
    }
}
