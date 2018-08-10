using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Movement : MonoBehaviour {

    private const string TELEPORT_CAPABLE_TAG = "TeleportReceiver";
    private const string CONTROLLER_TAG = "GameController";

    private bool wasStickHeldLastFrame;
    private Vector3 currentTargetPosition;
    private Vector2 currentStickPosition;
    private NetworkManager networkManager;

    public float TeleportThreshold = 0.5f;
    public float NoHoldThreshold = 0.5f;
    public float RotateThreshold = 0.4f;
    public float RotateExponent = 1.5f;
    public float MaxRotateSpeed = 1.25f;

    public GvrLaserPointer GvrLaserPointer;

    public GameObject NetworkManagerObject;

    // Use this for initialization
    void Start()
    {
        networkManager = NetworkManagerObject.GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        GvrPointerInputModule.Pointer = GvrLaserPointer;

        var device = GvrControllerInput.GetDevice(GvrControllerHand.Dominant);

        //var secondaryThumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        Teleport(device);
        //Rotate(secondaryThumbstick);
    }

    private void Teleport(GvrControllerInputDevice device)
    {
        if (device.GetButtonUp(GvrControllerButton.TouchPadButton) &&
            GvrLaserPointer.CurrentRaycastResult.gameObject != null &&
            GvrLaserPointer.CurrentRaycastResult.gameObject.tag == TELEPORT_CAPABLE_TAG)
        {

            transform.position = GvrLaserPointer.CurrentRaycastResult.worldPosition + new Vector3(0, 1.5f, 0);
        }
    }

    private void Rotate(Vector2 thumbstick)
    {
        if (Mathf.Abs(thumbstick.x) > RotateThreshold)
        {
            var eulerAngles = transform.rotation.eulerAngles;
            var direction = Mathf.Abs(thumbstick.x) / thumbstick.x;
            eulerAngles.y += direction * Mathf.Min(Mathf.Pow(RotateExponent, Mathf.Abs(thumbstick.x)), MaxRotateSpeed);
            transform.eulerAngles = eulerAngles;

            networkManager.RaiseEvent(NetworkManager.EventCode.PlayerRotationChanged, eulerAngles.y);
        }
    }
}
