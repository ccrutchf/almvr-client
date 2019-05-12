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
        public BoardClientSignalRProvider(Action<string> log)
            : base("board", log) { }

        public Task<BoardModel> GetBoardAsync() =>
            Connection.InvokeAsync<BoardModel>("GetBoard");
    }
}
