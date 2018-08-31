using AlmVR.Client.Core;
using AlmVR.Common.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour {
    public string ID;
    public ICardClient CardClient;

    private CardModel cardModel;

	// Use this for initialization
	async void Start ()
    {
        cardModel = await CardClient.GetCardAsync(ID);

        name = cardModel.Name;

        var text = GetComponentInChildren<Text>();
        if (text != null)
        {
            text.text = cardModel.Name;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
