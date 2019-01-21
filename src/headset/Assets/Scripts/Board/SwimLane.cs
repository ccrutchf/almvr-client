using AlmVR.Client.Core;
using AlmVR.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SwimLane : MonoBehaviour {
    private const float RADIUS = 2.0f;

    public event EventHandler ChildCardChanged;

    public SwimLaneModel SwimLaneModel;
    public GameObject CardPrefab;
    public ICardClient CardClient;
    public GvrLaserPointer GvrLaserPointer;
    public GameObject Player;
    public BoardManager BoardManager;

    private int counter = 0;
    private readonly float degrees = Mathf.PI / 8.0f;
    private List<Card> cards = new List<Card>();

    public void AddCard(string id)
    {
        var go = GameObject.Instantiate(CardPrefab, transform);

        var x = RADIUS * Mathf.Cos(degrees * (float)counter);
        var z = RADIUS * Mathf.Sin(degrees * (float)counter);

        counter++;

        go.transform.localPosition = new Vector3(-x, 1, z);

        var cardScript = go.GetComponent<Card>();
        cardScript.ID = id;
        cardScript.CardClient = CardClient;
        cardScript.GvrLaserPointer = GvrLaserPointer;
        cardScript.Player = Player;
        cardScript.BoardManager = BoardManager;
        cardScript.CardChanged += CardScript_CardChanged;

        cards.Add(cardScript);
    }

    private void CardScript_CardChanged(object sender, EventArgs e)
    {
        ChildCardChanged?.Invoke(this, e);
    }

    void Start()
    {
        if (SwimLaneModel == null)
        {
            return;
        }

        name = SwimLaneModel.Name;

        var text = GetComponentInChildren<Text>();
        if (text != null)
        {
            text.text = SwimLaneModel.Name;
        }
    }

    public void AfterPositionUpdate()
    {
        foreach (var card in SwimLaneModel.Cards)
        {
            AddCard(card.ID);
        }
    }

    public bool AnyCardSelected() => cards.Any(x => x.Selected);

    public void ResetSwimLane()
    {
        foreach (var card in cards)
        {
            card.CardChanged -= CardScript_CardChanged;
            GameObject.Destroy(card.gameObject);
        }

        cards.Clear();
        counter = 0;
    }
}
