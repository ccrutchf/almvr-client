using AlmVR.Client;
using AlmVR.Client.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    private const string CARD_TAG = "Card";
    private const float RADIUS = 14f;

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

        List<Task> swimLaneInits = new List<Task>();

        int counter = 0;
        float degrees = Mathf.PI / (float)board.SwimLanes.Count();
        foreach (var lane in board.SwimLanes)
        {
            var go = GameObject.Instantiate(SwimLanePrefab, transform);
            var swimLane = go.GetComponent<SwimLane>();

            swimLane.SwimLaneModel = lane;
            swimLane.CardClient = cardClient;

            var x = transform.position.x + RADIUS * Mathf.Cos(degrees * (float)counter);
            var z = transform.position.z + RADIUS * Mathf.Sin(degrees * (float)counter);

            var ray = new Ray(new Vector3(-x, 100.0f, z), Vector3.down);
            RaycastHit hit = default(RaycastHit);

            if (Island.GetComponent<Collider>().Raycast(ray, out hit, 100.0f))
            {
                go.transform.position = hit.point + new Vector3(0, 1, 0);
            }

            swimLane.AfterPositionUpdate();

            counter++;
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
