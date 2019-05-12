using AlmVR.Client.Core;
using AlmVR.Common.Models;
using Prism.Commands;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlmVR.Client.TestClient.Modules.DefaultModule.ViewModels
{
    public class BoardTestClientViewModel : TabControlBindableBase
    {
        private readonly IBoardClient boardClient;
        private readonly ICardClient cardClient;

        public ObservableCollection<BoardModel> BoardModels { get; private set; } = new ObservableCollection<BoardModel>();
        public DelegateCommand GetBoardAsyncCommand { get; private set; }

        public BoardTestClientViewModel(IBoardClient boardClient, ICardClient cardClient)
            : base("Board")
        {
            this.boardClient = boardClient;
            this.cardClient = cardClient;

            GetBoardAsyncCommand = new DelegateCommand(GetBoardAsyncCommandExecute);
        }

        private async void GetBoardAsyncCommandExecute()
        {
            var results = await boardClient.GetBoardAsync();

            BoardModels.Clear();
            BoardModels.Add(new BoardModel
            {
                ID = results.ID,
                SwimLanes = await Task.WhenAll(from s in results.SwimLanes
                                               select GetUpdatedSwimLaneAsync(s)),
            });
        }

        private async Task<SwimLaneModel> GetUpdatedSwimLaneAsync(SwimLaneModel swimLane) =>
            new SwimLaneModel
            {
                ID = swimLane.ID,
                Name = swimLane.Name,
                Cards = await GetCardsAsync(swimLane),
            };

        private Task<CardModel[]> GetCardsAsync(SwimLaneModel swimLane) =>
            Task.WhenAll(from c in swimLane.Cards
                         select cardClient.GetCardAsync(c.ID));
    }
}
