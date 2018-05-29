using AlmVR.Client;
using AlmVR.Client.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    public GameObject CardPrefab;

    private IBoardClient boardClient;
    private ICardClient cardClient;

    public string HostName;
    public int Port;
    
	// Use this for initialization
	async void Start () {
        boardClient = ClientFactory.GetInstance<IBoardClient>();
        cardClient = ClientFactory.GetInstance<ICardClient>();

        await boardClient.ConnectAsync(HostName, Port);
        await cardClient.ConnectAsync(HostName, Port);

        var board = await boardClient.GetBoardAsync();

        var cards = board.SwimLanes.SelectMany(s => s.Cards)
                        .ToDictionary(s => s.ID, s => GameObject.Instantiate(CardPrefab));

        foreach (var key in cards.Keys)
        {
            var cardScript = cards[key].GetComponent<Card>();
            cardScript.ID = key;
            cardScript.CardClient = cardClient;

            cards[key].transform.parent = transform;

#pragma warning disable
            // We do not need to wait for this to finish.
            var task = cardScript.InitializeAsync();
#pragma warning restore
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy() {
        boardClient.Dispose();
        cardClient.Dispose();
    }
}
