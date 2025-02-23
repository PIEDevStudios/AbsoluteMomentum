using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerPayloadManager : NetworkBehaviour
{
    [SerializeField] private Player player;
    
    // Netcode General
    private NetworkTimer timer;
    private const float KServerTickRate = 60f;
    private const int KBufferSize = 1024;
    
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
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        while (timer.ShouldTick())
        {
            HandleClientTick();
            HandleServerTick();
        }
    }

    void HandleClientTick()
    {
        if (!IsClient) return;

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
        
        //HandleServerReconciliation();
    }
    
    [ServerRpc]
    void SendToServerRpc(InputPayload input)
    {
        serverInputQueue.Enqueue(input);
    }

    void HandleServerTick()
    {
        var bufferIndex = -1;
        while (serverInputQueue.Count > 0)
        {
            InputPayload inputPayload = serverInputQueue.Dequeue();
            
            bufferIndex = inputPayload.tick % KBufferSize;
            
            StatePayload statePayload = SimulateMovement(inputPayload);
            serverStateBuffer.Add(statePayload, bufferIndex);
        }

        if (bufferIndex == -1) return;
        SendToClientRpc(serverStateBuffer.Get(bufferIndex));
    }

    StatePayload SimulateMovement(InputPayload inputPayload)
    {
        Physics.simulationMode = SimulationMode.Script;
        
        // MOVE METHOD HERE
        Physics.Simulate(Time.fixedDeltaTime);
        Physics.simulationMode = SimulationMode.FixedUpdate;

        return new StatePayload()
        {
            tick = inputPayload.tick,
            position = player.transform.position,
            rotation = player.transform.rotation,
            velocity = player.rb.linearVelocity,
            angularVelocity = player.rb.angularVelocity,
        };
    }
    
    [ClientRpc]
    void SendToClientRpc(StatePayload statePayload)
    {
        if(!IsOwner) return;
        lastServerState = statePayload;
    }
    
    
    StatePayload ProcessMovement(InputPayload inputPayload)
    {
        // INSERT MOVE METHOD HERE

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
