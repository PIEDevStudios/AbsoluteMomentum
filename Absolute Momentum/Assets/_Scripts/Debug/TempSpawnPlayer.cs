using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using TMPro;
public class TempSpawnPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<Rigidbody>().position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
