using AlmVR.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlmVR.Client.Core
{
    public interface ICardClient : IClient
    {
        event EventHandler<CardChangedEventArgs> CardChanged;

        Task<CardModel> GetCardAsync(string id);
        Task MoveCardAsync(CardModel card, SwimLaneModel targetSwimLane);
    }
}
