using AlmVR.Client;
using AlmVR.Client.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    private const string CARD_TAG = "Card";
    private const float RADIUS = 14f;

    public GameObject CardPrefab;
    public GameObject SwimLanePrefab;

    public GameObject Island;

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

        int counter = 0;
        float degrees = Mathf.PI / (float)board.SwimLanes.Count();
        foreach (var lane in board.SwimLanes)
        {
            var go = GameObject.Instantiate(SwimLanePrefab, transform);

            var x = transform.position.x + RADIUS * Mathf.Cos(degrees * (float)counter);
            var z = transform.position.z + RADIUS * Mathf.Sin(degrees * (float)counter);

            var ray = new Ray(new Vector3(x, 100.0f, z), Vector3.down);
            RaycastHit hit = default(RaycastHit);

            if (Island.GetComponent<Collider>().Raycast(ray, out hit, 100.0f))
            {
                go.transform.position = hit.point;

                var centerPos = go.transform.position;
                centerPos.y = hit.point.y;

                go.transform.forward = go.transform.position - centerPos;
            }

            counter++;
        }

        var cards = board.SwimLanes.SelectMany(s => s.Cards)
                        .ToDictionary(c => c.ID, c => GameObject.Instantiate(CardPrefab, transform));

        foreach (var key in cards.Keys)
        {
            var cardScript = cards[key].GetComponent<Card>();
            cardScript.ID = key;
            cardScript.CardClient = cardClient;

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
