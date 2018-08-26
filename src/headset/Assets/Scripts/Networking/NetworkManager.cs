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
    public GameObject LocalPlayer;
    public GameObject RemotePlayer;
    public GameObject PUNVoice;

    private PhotonVoiceRecorder _voiceRecorder;

	void Start () {
        PhotonNetwork.ConnectUsingSettings("1.0");
    }

    private void Update()
    {
    }

    public void OnDestroy()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        RoomOptions roomOptions = new RoomOptions
        {
            IsVisible = false,
            MaxPlayers = 8
        };
        PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        var punVoice = GameObject.Instantiate(PUNVoice);
        _voiceRecorder = punVoice.GetComponent<PhotonVoiceRecorder>();

        int viewId = PhotonNetwork.AllocateViewID();

        var data = new Dictionary<string, object>
        {
            { "ViewID", viewId },
            { "Position", LocalPlayer.transform.position }
        };

        RaiseEvent(EventCode.InstantiateVRAvatar, data);
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

            if (PhotonNetwork.player.ID != senderid)
            {
                var data = (Dictionary<string, object>)content;

                go = Instantiate(RemotePlayer) as GameObject;
                go.transform.position = (Vector3)data["Position"];

                var remotePlayerManager = go.GetComponent<RemotePlayerManager>();
                remotePlayerManager.NetworkManager = this;
                remotePlayerManager.InitializeNetworkManager();

                PhotonView pView = go.GetComponent<PhotonView>();

                if (pView != null)
                {
                    pView.viewID = (int)data["ViewID"];
                }

                if (_voiceRecorder != null)
                {
                    _voiceRecorder.Transmit = false;
                    _voiceRecorder.Transmit = true;
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
