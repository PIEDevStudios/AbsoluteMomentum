using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerPayloadManager : NetworkBehaviour
{
    [SerializeField] private Player player;
    
    // Netcode General
    private NetworkTimer timer;
    private const float KServerTickRate = 60f;
    private const int KBufferSize = 1024;
    [Header("Netcode")]
    [SerializeField] private float reconiliationThreshold = 10f;
    [SerializeField] private GameObject serverCube;
    [SerializeField] private GameObject clientCube;
    // Netcode client specific
    private CircularBuffer<StatePayload> clientStateBuffer;
    private CircularBuffer<InputPayload> clientInputBuffer;
    private StatePayload lastServerState;
    private StatePayload lastProcessedState;
    
    // Netcode server specific
    CircularBuffer<StatePayload> serverStateBuffer;
    private Queue<InputPayload> serverInputQueue;
    public struct InputPayload : INetworkSerializable
    {
        public int tick;
        public Vector2 moveVector;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref moveVector);
        }
    }
    
    public struct StatePayload : INetworkSerializable
    {
        public int tick;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public Vector3 angularVelocity;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref rotation);
            serializer.SerializeValue(ref velocity);
            serializer.SerializeValue(ref angularVelocity);
        }
    }

    private void Awake()
    {
        timer = new NetworkTimer(KServerTickRate);
        clientStateBuffer = new CircularBuffer<StatePayload>(KBufferSize);
        clientInputBuffer = new CircularBuffer<InputPayload>(KBufferSize);
        
        serverStateBuffer = new CircularBuffer<StatePayload>(KBufferSize);
        serverInputQueue = new Queue<InputPayload>();
    }

    void Update()
    {
        timer.Update(Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.position += transform.forward * 20f;
        }
    }

    void FixedUpdate()
    {
        while (timer.ShouldTick())
        {
            HandleClientTick();
            HandleServerTick();
            
        }
    }

    void HandleClientTick()
    {
        if (!IsClient || !IsOwner) return;

        var currentTick = timer.CurrentTick;
        var bufferIndex = currentTick % KBufferSize;

        InputPayload inputPayload = new InputPayload()
        {
            tick = currentTick,
            moveVector = player.playerInput.moveVector
        };
        
        clientInputBuffer.Add(inputPayload, bufferIndex);
        SendToServerRpc(inputPayload);

        StatePayload statePayload = ProcessMovement(inputPayload);
        clientCube.transform.position = new Vector3(statePayload.position.x, statePayload.position.y + 4, statePayload.position.z);
        clientStateBuffer.Add(statePayload, bufferIndex);
        
        HandleServerReconciliation();
    }

    bool ShouldReconcile()
    {
        bool isNewServerState = !lastServerState.Equals(default);
        bool isLastStateUndefinedOrDifferent = lastProcessedState.Equals(default) || !lastProcessedState.Equals(lastServerState);
        
        return isNewServerState && isLastStateUndefinedOrDifferent;
    }

    void HandleServerReconciliation()
    {
        if (!ShouldReconcile()) return;
        int bufferIndex = lastServerState.tick % KBufferSize;
        if (bufferIndex - 1 < 0) return;
        
        StatePayload rewindState = IsHost ? serverStateBuffer.Get(bufferIndex - 1) : lastServerState;
        StatePayload clientState = IsHost ? clientStateBuffer.Get(bufferIndex - 1) : clientStateBuffer.Get(bufferIndex);
        float positionError = Vector3.Distance(rewindState.position, clientState.position);
        Debug.Log("Server trying to reconcile. Position error: " + positionError + " RSTATE: " + rewindState.position + " CSTATE: " + clientState.position);
 
        if (positionError > reconiliationThreshold)
        {
            ReconcileState(rewindState);
            Debug.Log("Server reconciled");
        }
        
        lastProcessedState = rewindState;

    }

    void ReconcileState(StatePayload rewindState)
    {
        player.transform.position = rewindState.position;
        player.transform.rotation = rewindState.rotation;
        player.rb.linearVelocity = rewindState.velocity;
        player.rb.angularVelocity = rewindState.angularVelocity;

        if (!rewindState.Equals(lastServerState)) return;
        
        clientStateBuffer.Add(rewindState, rewindState.tick);
        
        // Replay all inputs from the rewind state to the current state

        int tickToReplay = lastServerState.tick;

        while (tickToReplay != timer.CurrentTick)
        {
            int bufferIndex = tickToReplay % KBufferSize;
            StatePayload statePayload = ProcessMovement(clientInputBuffer.Get(bufferIndex));
            clientStateBuffer.Add(statePayload, bufferIndex);
            tickToReplay++;
        }
        
        
        
        


    }
    
    [ServerRpc]
    void SendToServerRpc(InputPayload input)
    {
        serverInputQueue.Enqueue(input);
    }

    void HandleServerTick()
    {
        if (!IsServer) return;
        StatePayload statePayload;
        var bufferIndex = -1;
        while (serverInputQueue.Count > 0)
        {
            InputPayload inputPayload = serverInputQueue.Dequeue();
            
            bufferIndex = inputPayload.tick % KBufferSize;
            
            if (IsHost) //If we dont check if its host then we will have double input from host. I mean host will move twice faster then he should
            {
                statePayload = new StatePayload()
                {
                    tick = inputPayload.tick,
                    position = player.transform.position,
                    rotation = player.transform.rotation,
                    velocity = player.rb.linearVelocity,
                    angularVelocity = player.rb.angularVelocity
                };
                serverCube.transform.position = new Vector3(statePayload.position.x, statePayload.position.y + 4, statePayload.position.z);
                serverStateBuffer.Add(statePayload, bufferIndex);
                SendToClientRpc(statePayload);
                continue;
            }
            
            statePayload = ProcessMovement(inputPayload);
            serverCube.transform.position = new Vector3(statePayload.position.x, statePayload.position.y + 4, statePayload.position.z);
            serverStateBuffer.Add(statePayload, bufferIndex);
        }

        if (bufferIndex == -1) return;
        SendToClientRpc(serverStateBuffer.Get(bufferIndex));
    }
    
    [ClientRpc]
    void SendToClientRpc(StatePayload statePayload)
    {
        if(!IsOwner) return;
        lastServerState = statePayload;
    }
    
    
    StatePayload ProcessMovement(InputPayload inputPayload)
    {
        
        player.Move(player.playerInput.GetInputValues());

        return new StatePayload()
        {
            tick = inputPayload.tick,
            position = player.transform.position,
            rotation = player.transform.rotation,
            velocity = player.rb.linearVelocity,
            angularVelocity = player.rb.angularVelocity,
        };

    }
    
}
