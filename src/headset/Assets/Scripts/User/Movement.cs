using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Movement : MonoBehaviour {

    private const string TELEPORT_CAPABLE_TAG = "TeleportReceiver";
    private const string SWIM_LANE_TAG = "SwimLane";

    private Vector2 prevTouchPos = Vector2.zero;
    private Vector2 swipeDelta = Vector2.zero;
    private bool hasSwiped = false;
    private float targetRotation;
    private float currentRotation;
    private NetworkManager networkManager;

    public float SwipeThreshold = 0.125f;
    public float RotationAmount = 20;

    public GvrLaserPointer GvrLaserPointer;

    public GameObject NetworkManagerObject;
    public BoardManager BoardManager;

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

        Teleport(device);
        Rotate(device);
    }

    private void Teleport(GvrControllerInputDevice device)
    {
        if (device.GetButtonUp(GvrControllerButton.TouchPadButton) &&
            GvrLaserPointer.CurrentRaycastResult.gameObject != null &&
            (GvrLaserPointer.CurrentRaycastResult.gameObject.tag == TELEPORT_CAPABLE_TAG || 
            (GvrLaserPointer.CurrentRaycastResult.gameObject.tag == SWIM_LANE_TAG &&
            !BoardManager.AnyCardsSelected())))
        {
            transform.position = GvrLaserPointer.CurrentRaycastResult.worldPosition + new Vector3(0, 1.5f, 0);

            networkManager.RaiseEvent(NetworkManager.EventCode.PlayerPositionChanged, transform.position);
        }
    }

    private void Rotate(GvrControllerInputDevice device)
    {
        if (device.GetButton(GvrControllerButton.TouchPadTouch) && !device.GetButton(GvrControllerButton.TouchPadButton))
        {
            if (prevTouchPos == Vector2.zero)
                prevTouchPos = device.TouchPos;
            else
            {
                swipeDelta = device.TouchPos - prevTouchPos;
                prevTouchPos = device.TouchPos;

                if (!hasSwiped && swipeDelta.magnitude >= SwipeThreshold)
                {
                    targetRotation += RotationAmount * swipeDelta.x / Mathf.Abs(swipeDelta.x);

                    if (targetRotation > 360)
                        targetRotation -= 360;
                    else if (targetRotation < 0)
                        targetRotation += 360;

                    var quat = transform.rotation;
                    var temp = quat.eulerAngles;
                    temp.y = targetRotation;
                    quat.eulerAngles = temp;

                    networkManager.RaiseEvent(NetworkManager.EventCode.PlayerRotationChanged, quat);

                    hasSwiped = true;
                }
            }
        }
        else
        {
            prevTouchPos = Vector2.zero;
            hasSwiped = false;
        }

        var eulerAngles = transform.eulerAngles;
        eulerAngles.y = Mathf.Lerp(eulerAngles.y, targetRotation, Time.deltaTime * 5);
        transform.eulerAngles = eulerAngles;
    }
}
