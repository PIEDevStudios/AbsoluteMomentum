using Unity.Netcode.Components;
using UnityEngine;


public enum AuthMode {
    Server,
    Client
}

[DisallowMultipleComponent]
public class ClientNetworkTransform : NetworkTransform {
    public AuthMode authorityMode = AuthMode.Client;

    protected override bool OnIsServerAuthoritative() => authorityMode == AuthMode.Server;
}
