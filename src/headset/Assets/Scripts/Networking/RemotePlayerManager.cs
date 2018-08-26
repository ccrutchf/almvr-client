using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerManager : MonoBehaviour {

    public NetworkManager NetworkManager;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitializeNetworkManager()
    {
        NetworkManager.NetworkEventReceived += NetworkManager_NetworkEventReceived;
    }

    private void NetworkManager_NetworkEventReceived(object sender, NetworkEventReceivedEventArgs e)
    {
        switch (e.EventCode)
        {
            case NetworkManager.EventCode.PlayerPositionChanged:
                transform.position = (Vector3)e.Content;
                break;
            case NetworkManager.EventCode.PlayerRotationChanged:
                transform.rotation = (Quaternion)e.Content;
                break;
        }
    }
}
