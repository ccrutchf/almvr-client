using Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : PunBehaviour {

    public enum EventCode : byte
    {
        Unknown,

        InstantiateVRAvatar,
        PlayerPositionChanged,
        PlayerRotationChanged
    }

    public event EventHandler<NetworkEventReceivedEventArgs> NetworkEventReceived;

    public string RoomName;
    public GameObject LocalAvatar;

	void Start () {
        PhotonNetwork.ConnectUsingSettings("1.0");
    }

    public void OnDestroy()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = false;
        roomOptions.MaxPlayers = 8;
        PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        int viewId = PhotonNetwork.AllocateViewID();

        RaiseEvent(EventCode.InstantiateVRAvatar, viewId);
    }

    public void OnEnable()
    {
        PhotonNetwork.OnEventCall += OnEvent;
    }

    public void OnDisable()
    {
        PhotonNetwork.OnEventCall -= OnEvent;
    }

    public bool RaiseEvent(EventCode eventCode, object eventContent)
    {
        return PhotonNetwork.RaiseEvent((byte)eventCode, eventContent, true, new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All });
    }

    private void OnEvent(byte eventcode, object content, int senderid)
    {
        EventCode @event = (EventCode)eventcode;

        if (@event == EventCode.InstantiateVRAvatar)
        {
            GameObject go = null;

            if (PhotonNetwork.player.ID == senderid)
            {
                go = LocalAvatar;
            }
            else
            {
                go = Instantiate(Resources.Load("RemoteAvatar")) as GameObject;
                go.GetComponent<RemoteAvatarSync>().NetworkManager = this;
            }

            if (go != null)
            {
                PhotonView pView = go.GetComponent<PhotonView>();

                if (pView != null)
                {
                    pView.viewID = (int)content;
                }
            }
        }

        if (PhotonNetwork.player.ID != senderid)
        {
            NetworkEventReceived?.Invoke(this, new NetworkEventReceivedEventArgs
            {
                Content = content,
                EventCode = @event
            });
        }
    }
}
