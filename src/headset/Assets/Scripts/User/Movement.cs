using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Movement : MonoBehaviour {

    private const string TELEPORT_CAPABLE_TAG = "TeleportReceiver";

    private bool wasStickHeldLastFrame;
    private Vector3 currentTargetPosition;
    private Vector2 currentStickPosition;
    private NetworkManager networkManager;

    public float TeleportThreshold = 0.5f;
    public float NoHoldThreshold = 0.5f;
    public float RotateThreshold = 0.4f;
    public float RotateExponent = 1.5f;
    public float MaxRotateSpeed = 1.25f;

    public GameObject NetworkManagerObject;

	// Use this for initialization
	void Start () {
        networkManager = NetworkManagerObject.GetComponent<NetworkManager>();
	}

    // Update is called once per frame
    void Update () {
        OVRInput.Update();

        var secondaryThumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        Teleport(secondaryThumbstick);
        Rotate(secondaryThumbstick);
    }

    private void Teleport(Vector2 thumbstick)
    {
        var lineRender = GetComponent<LineRenderer>();
        if (thumbstick.y > TeleportThreshold)
        {
            lineRender.enabled = true;

            var rTouchLocalPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            var rTouchWorldPosition = transform.TransformPoint(rTouchLocalPosition);

            var rTouchLocalForward = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;
            var rTouchWorldForward = transform.TransformDirection(rTouchLocalForward);

            var ray = new Ray(rTouchWorldPosition, rTouchWorldForward);
            RaycastHit hit = default(RaycastHit);
            if (GameObject.FindGameObjectsWithTag(TELEPORT_CAPABLE_TAG).Any(x => x.GetComponent<Collider>().Raycast(ray, out hit, 100.0f)))
            {
                currentTargetPosition = hit.point;
                currentStickPosition = thumbstick;

                lineRender.SetPositions(new Vector3[]
                {
                   rTouchWorldPosition,
                   currentTargetPosition
                });
            }
            else
            {
                lineRender.enabled = false;
            }

            wasStickHeldLastFrame = true;
        }
        else
        {
            lineRender.enabled = false;

            if (wasStickHeldLastFrame && thumbstick.magnitude < NoHoldThreshold)
            {
                transform.position = currentTargetPosition + new Vector3(0, 1.5f, 0);
                networkManager.RaiseEvent(NetworkManager.EventCode.PlayerPositionChanged, transform.position);
            }

            wasStickHeldLastFrame = false;
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
