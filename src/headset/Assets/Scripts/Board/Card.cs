using AlmVR.Client.Core;
using AlmVR.Common.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Card : MonoBehaviour {
    public string ID;
    public ICardClient CardClient;

    private CardModel cardModel;

	// Use this for initialization
	void Start ()
    {
        transform.position = new Vector3(Random.value * 10.0f - 5, 50 + Random.value * 10.0f, Random.value * 10.0f - 5);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public async Task InitializeAsync()
    {
        cardModel = await CardClient.GetCardAsync(ID);

        name = cardModel.Name;
    }
}
