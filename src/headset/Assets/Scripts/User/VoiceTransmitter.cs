using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceTransmitter : MonoBehaviour {
    private const string PERMISSION_NAME = "android.permission.RECORD_AUDIO";

    private bool _hasPermission = false;

	// Use this for initialization
	void Start () {
        RequestMicrophone();
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void RequestMicrophone()
    {
        GvrPermissionsRequester permissionRequester = GvrPermissionsRequester.Instance;
        
        if (permissionRequester?.IsPermissionGranted(PERMISSION_NAME) ?? false)
        {
            permissionRequester.RequestPermissions(new string[] { PERMISSION_NAME }, permissionStatuses =>
            {
                foreach (var permissionStatus in permissionStatuses)
                {
                    _hasPermission = permissionStatus.Granted;
                }
            });
        }
    }
}
