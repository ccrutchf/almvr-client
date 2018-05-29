using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkEventReceivedEventArgs : EventArgs
{
    public object Content { get; set; }
    public NetworkManager.EventCode EventCode{ get; set; }
}
