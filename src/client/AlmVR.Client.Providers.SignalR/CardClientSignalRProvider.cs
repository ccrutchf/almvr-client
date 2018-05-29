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
    public class CardClientSignalRProvider : ClientBase, ICardClient
    {
        public CardClientSignalRProvider()
            : base("card") { }

        public Task<CardModel> GetCardAsync(string id)
        {
            return Connection.InvokeAsync<CardModel>("GetCard", id);
        }
    }
}
