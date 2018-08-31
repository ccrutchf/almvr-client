using AlmVR.Client.Core;
using AlmVR.Common.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SwimLane : MonoBehaviour {
    private const float RADIUS = 2.0f;

    static bool test = false;

    public BoardModel.SwimLaneModel SwimLaneModel;
    public GameObject CardPrefab;
    public ICardClient CardClient;

    private int counter = 0;
    private readonly float degrees = Mathf.PI / 8.0f;

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
    }

    void Start()
    {
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
}
