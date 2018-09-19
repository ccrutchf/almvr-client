using AlmVR.Client.Core;
using AlmVR.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private const string SWIM_LANE_TAG = "SwimLane";

    public event EventHandler CardChanged;

    public string ID;
    public ICardClient CardClient;
    public GameObject Player;
    public GvrLaserPointer GvrLaserPointer;
    public BoardManager BoardManager;

    public bool Selected;

    private CardModel cardModel;
    private float distance;
    private bool wasSelected;

    // Use this for initialization
    async void Start ()
    {
        CardClient.CardChanged += CardClient_CardChanged;

        cardModel = await CardClient.GetCardAsync(ID);

        name = cardModel.Name;

        var text = GetComponentInChildren<Text>();
        if (text != null)
        {
            text.text = cardModel.Name;
        }
    }

    private void OnDestroy()
    {
        // Release memory.
        CardClient.CardChanged -= CardClient_CardChanged;
    }
	
	// Update is called once per frame
	async void Update ()
    {
        GvrPointerInputModule.Pointer = GvrLaserPointer;

        transform.GetChild(0).localScale = Vector3.zero;

        var device = GvrControllerInput.GetDevice(GvrControllerHand.Dominant);

        var otherCardSelected = BoardManager.AnyCardsSelected();

        if (Selected ||
            (GvrLaserPointer.CurrentRaycastResult.gameObject != null &&
            GvrLaserPointer.CurrentRaycastResult.gameObject == gameObject  &&
            !otherCardSelected))
        {
            transform.GetChild(0).localScale = Vector3.one;

            if (device.GetButtonUp(GvrControllerButton.TouchPadButton))
            {
                Selected = true;
            }
        }

        if (Selected)
        {
            var deviceWorldPosition = Player.transform.TransformPoint(device.Position);

            if (!wasSelected)
            {
                distance = Vector3.Distance(deviceWorldPosition, transform.position);

                wasSelected = true;

                GetComponent<Rigidbody>().detectCollisions = false;
            }

            transform.position = GvrLaserPointer.GetPointAlongPointer(distance) + new Vector3(0, -0.5f, 0);

            distance = Mathf.Lerp(distance, 3.0f, Time.deltaTime * 2);

            if (GvrLaserPointer.CurrentRaycastResult.gameObject != null &&
                GvrLaserPointer.CurrentRaycastResult.gameObject.tag == SWIM_LANE_TAG &&
                device.GetButtonUp(GvrControllerButton.TouchPadButton))
            {
                await CardClient.MoveCardAsync(cardModel, GvrLaserPointer.CurrentRaycastResult.gameObject.GetComponent<SwimLane>().SwimLaneModel);
            }
        }
        else if (!otherCardSelected)
        {
            wasSelected = false;
        }
    }

    private void CardClient_CardChanged(object sender, CardChangedEventArgs e)
    {
        if (e.Card.ID != cardModel.ID)
        {
            return;
        }

        UnityMainThreadDispatcher.Instance().Enqueue(() => CardChanged?.Invoke(this, new EventArgs()));
    }
}
