using AlmVR.Client;
using AlmVR.Client.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    private const string CARD_TAG = "Card";

    public GameObject CardPrefab;

    private IBoardClient boardClient;
    private ICardClient cardClient;

    public string HostName;
    public int Port;
    public GvrLaserPointer GvrLaserPointer;

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
        GvrPointerInputModule.Pointer = GvrLaserPointer;

        //var device = GvrControllerInput.GetDevice(GvrControllerHand.Dominant);

        foreach (var go in GameObject.FindGameObjectsWithTag(CARD_TAG))
        {
            go.transform.GetChild(0).localScale = Vector3.zero;
        }

        if (GvrLaserPointer.CurrentRaycastResult.gameObject != null &&
            GvrLaserPointer.CurrentRaycastResult.gameObject.tag == CARD_TAG)
        {
            GvrLaserPointer.CurrentRaycastResult.gameObject.transform.GetChild(0).localScale = Vector3.one;
        }
    }

    private void OnDestroy() {
        boardClient.Dispose();
        cardClient.Dispose();
    }
}
