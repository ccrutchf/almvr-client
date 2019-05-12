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
        public CardClientSignalRProvider(Action<string> log)
            : base("card", log) { }

        public event EventHandler<CardChangedEventArgs> CardChanged;

        public Task<CardModel> GetCardAsync(string id) =>
            Connection.InvokeAsync<CardModel>("GetCard", id);

        public Task MoveCardAsync(CardModel card, SwimLaneModel targetSwimLane) =>
            Connection.InvokeAsync("MoveCard", card, targetSwimLane);

        protected override void OnConnectionCreated()
        {
            Connection.On<CardModel>("CardChanged", card =>
            {
                CardChanged?.Invoke(this, new CardChangedEventArgs(card));
            });
        }
    }
}
