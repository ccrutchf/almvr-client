using AlmVR.Client.Core;
using AlmVR.Common.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlmVR.Client.Providers.SignalR
{
    public class BoardClientSignalRProvider : ClientBase, IBoardClient
    {
        public event EventHandler ThingHappenedToMe;

        public BoardClientSignalRProvider()
            : base("board") { }

        protected override void OnConnectionCreated()
        {
            Connection.On("DoThingToClients", () =>
            {
                Console.WriteLine("raising event");
                ThingHappenedToMe?.Invoke(this, new EventArgs());
            });
        }

        public Task<BoardModel> GetBoardAsync()
        {
            return Connection.InvokeAsync<BoardModel>("GetBoard");
        }
    }
}
