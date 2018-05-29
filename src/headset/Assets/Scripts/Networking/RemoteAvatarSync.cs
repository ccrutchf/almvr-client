using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteAvatarSync : MonoBehaviour {

    public NetworkManager NetworkManager;

	// Use this for initialization
	void Start () {
        NetworkManager.NetworkEventReceived += NetworkManager_NetworkEventReceived;
	}

    private void OnDestroy()
    {
        NetworkManager.NetworkEventReceived -= NetworkManager_NetworkEventReceived;
    }

    private void NetworkManager_NetworkEventReceived(object sender, NetworkEventReceivedEventArgs e)
    {
        switch (e.EventCode)
        {
            case NetworkManager.EventCode.PlayerPositionChanged:
                transform.position = (Vector3)e.Content;
                break;
            case NetworkManager.EventCode.PlayerRotationChanged:
                var eulerAngles = transform.rotation.eulerAngles;
                eulerAngles.y += (float)e.Content;
                transform.eulerAngles = eulerAngles;
                break;
        }
    }
}
