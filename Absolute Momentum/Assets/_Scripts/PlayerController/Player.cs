using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Utility;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using System.Security.Cryptography;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Player : StateMachineCore
{
    
    // Variables that hold what states the player can be in
    [field: HorizontalLine(color: EColor.Gray)]
    [field: Header("States")]
    [field: SerializeField] public PlayerIdle idle { get; private set; }
    [field: SerializeField] public PlayerMove move { get; private set; }
    [field: SerializeField] public PlayerAirborne airborne { get; private set; }
    [field: SerializeField] public PlayerSlide slide { get; private set; }
    [field: SerializeField] public PlayerWallrun wallrun { get; private set; }
    [field: SerializeField] public PlayerDash dash { get; private set; }
    [field: SerializeField] public PlayerWallSlide wallSlide { get; private set; }

    // Sensor scripts used for ground checks and wall checks
    [field:HorizontalLine(color: EColor.Gray)]
    [field:Header("Sensors")]
    [field:SerializeField] public GroundSensor groundSensor {get; private set;}
    [field:SerializeField] public WallSensor wallSensor {get; private set;}
    [field:SerializeField] public SlopeSensor slopeSensor {get; private set;}

    
    // References to other components on the player
    [field:HorizontalLine(color: EColor.Gray)]
    [Header("Player Components")]
    [field:SerializeField] public Transform graphics { get; private set; }
    [field:SerializeField] public Transform playerObj { get; private set; }
    [field:SerializeField] public Transform orientation { get; private set; }
    
    [SerializeField] private TrailRenderer trail;
    
    [Expandable]
    [SerializeField] public PlayerStats stats;
    [Header("Player Scripts")]
    [field:SerializeField] public PlayerInput playerInput {get; private set;}
    [field:SerializeField] public CapsuleCollider playerCollider {get; private set;}
    [SerializeField] private PlayerJumpManager jumpManager;
    [SerializeField] private PlayerWalljumpManager walljumpManager;
    [SerializeField] private PlayerItemManager itemManager;
    [SerializeField] private PlayerPayloadManager payloadManager;
    [field:SerializeField] public PlayerSpeedManager playerSpeedManager {get; private set;}
    [field:SerializeField] public PlayerRaceTimeManager playerRaceTimeManager {get; private set;}
    [SerializeField] private CinemachineVirtualCameraBase playerCamera;
    [field:SerializeField] public PlayerUIManager playerUI {get; private set;}

    
    [Header("Missle")]
    [SerializeField] private GameObject MissilePrefab;
    private float timeSinceLastMissileFire;

    [ReadOnly] public float wallrunResetTime;
    [ReadOnly] public bool leavingGround;
    [SerializeField] private Vector3 spawnPos;
    
    private float timeSinceLastGrounded;
    

    #region Unity Methods
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            playerCamera.Priority = 0;
            playerUI.gameObject.SetActive(false);
            enabled = false;
            return;
        }
        
        SetupInstances();
        stateMachine.SetState(idle);
        // ResetPlayer();
        
        // Disable gravity and simulate gravity manually (to allow for different gravity scales)
        rb.useGravity = false;
        stats.gravityEnabled = true;
        ChangeGravity(stats.NormalGravity);
        
        Cursor.lockState = CursorLockMode.Locked;
        
        playerCamera.Priority = 100;
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerUI.raceCountdownManager.OnCountdownFinished += EnablePlayerInput;

    }

    public void Update()
    {
        timeSinceLastMissileFire += Time.deltaTime;
        // if (playerInput.FiredMissile)
        // {
        //     TryFireMissile();
        // }            
    }
    


    
    void FixedUpdate() 
    {
        if (rb.linearVelocity.y > 0)
        {
            // Simulate custom gravity
            rb.AddForce(Vector3.down * stats.CurrentGravity, ForceMode.Force);
        }
        else
        {
            rb.AddForce(Vector3.down * stats.CurrentGravity * stats.FallingGravityMultiplier, ForceMode.Force);
        }

        
        // Call FixedUpdate logic
        // stateMachine.currentState.DoFixedUpdateBranch(); 
    }

    #endregion
    
    #region Helper (Private) Methods
    
    /// <summary>
    /// Loads character into race track, then notifies race manager that player is loaded in
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!IsOwner) return;

        if (RaceManager.Instance.levelNames.Contains(scene.name)) 
        {
            Debug.Log("Player Race Scene Loaded, CLIENT ID: " + NetworkManager.Singleton.LocalClientId);
            playerRaceTimeManager.ResetTimer();
            playerUI.HideAllUI();
            itemManager.ClearItems();
            DisablePlayerInput();
            StartCoroutine(NotifyRaceManagerWhenLoaded());
        }
    }
    
    /// <summary>
    /// Fires a missle
    /// </summary>
    void TryFireMissile()
    {
        Debug.Log("missile attempt");
        if (timeSinceLastMissileFire < 1f)
        {
            return;
        }
        timeSinceLastMissileFire = 0;
        
        Vector3 missileForwards = (orientation.forward + orientation.up).normalized;
        Quaternion missileRotation = Quaternion.LookRotation(missileForwards, Vector3.Cross(missileForwards,orientation.right));
        
        

        Missile m = MissilePrefab.GetComponent<Missile>();

        float distSq = float.MaxValue;
        GameObject closestPlayer = null;
        foreach (var player in GameManager.Instance.Players)
        {
            if (closestPlayer is null || Vector3.SqrMagnitude(player.transform.position - gameObject.transform.position) < distSq)
            {
                closestPlayer = player;
                distSq = Vector3.SqrMagnitude(player.transform.position - gameObject.transform.position);
            }
        }
        
        m.target = closestPlayer;
        m.owner = gameObject;
        Debug.Log($"missile fire {playerObj.position}");
        
        GameObject missile = Instantiate(MissilePrefab,  playerObj.position, missileRotation);
    }
    
    /// <summary>
    /// Handles transitions between states in player state machine
    /// </summary>
    private void HandleTransitions(PlayerInput.InputValues inputValues)
    {
        
        // Cache xInput and yInput from inputValues
        float xInput = inputValues.moveVector.x;
        float yInput = inputValues.moveVector.y;
        
        
        // condition for transitioning to a "grounded" state (move or idle) when transitioning from airborne
        bool airborneGroundCheck = stateMachine.currentState == airborne && !leavingGround;
        
        // condition for transitioning to a "grounded" state (move or idle) when transitioning from any state besides airborne
        bool nonAirborneGroundCheck = stateMachine.currentState == idle || stateMachine.currentState == move;
        
        Vector3 flatVel = Vector3.ProjectOnPlane(rb.linearVelocity, slopeSensor.hit.normal);

        // Transition to dash
        // if (playerInput.dashPressedThisFrame && true) // TODO: ensure player has whatever item is required to dash
        // {
        //     stateMachine.SetState(dash);
        //     return;
        // }
        // else if (stateMachine.currentState == dash && !dash.isComplete)
        // {
        //     return;
        // }

        // Transition to slide
        bool groundedSlideTransition = inputValues.slidePressedThisFrame && groundSensor.grounded && flatVel.magnitude > stats.minimumSlideSpeed;
        bool airborneSlideTransition = inputValues.slideHeld && !groundSensor.grounded && timeSinceLastGrounded > stats.MinimumSlideAirTime;
        if (groundedSlideTransition || airborneSlideTransition)
        {
            stateMachine.SetState(slide);
            return;
        }
        
        Vector3 XYVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        
        // Transition to wallrun
        if (!wallSensor.minHeightSensor.grounded && (wallSensor.wallLeft || wallSensor.wallRight) 
            && XYVel.magnitude >= stats.minWallrunEnterSpeed && stateMachine.currentState != slide && stateMachine.currentState != wallrun && stateMachine.currentState != wallSlide 
            && Time.time - wallrunResetTime > stats.wallrunResetTime )
        {
            stateMachine.SetState(wallrun);
            return;
        }
        
        // Transition to wallSlide
        if (!wallSensor.minHeightSensor.grounded && (wallSensor.wallLeft || wallSensor.wallRight || wallSensor.wallForward) 
            && XYVel.magnitude <= stats.minWallrunEnterSpeed && stateMachine.currentState != slide && stateMachine.currentState != wallrun && stateMachine.currentState != wallSlide 
            && Time.time - wallrunResetTime > stats.wallrunResetTime )
        {
            stateMachine.SetState(wallSlide);
            return;
        }
        
        // Transition to airborne
        if (!groundSensor.grounded && !slopeSensor.isOnSlope && ( !(stateMachine.currentState == slide || stateMachine.currentState == wallrun || stateMachine.currentState == wallSlide) || stateMachine.currentState.isComplete))
        {
            stateMachine.SetState(airborne);
            return;
        }
        
        // Transition to move
        if (groundSensor.grounded && (xInput != 0 || yInput != 0) && (nonAirborneGroundCheck || airborneGroundCheck || stateMachine.currentState.isComplete))
        {
            stateMachine.SetState(move);
            return;
        }

        float timeSinceLastMove = Time.time - inputValues.timeOfLastMoveInput;
        
        // Transition to idle
        if (groundSensor.grounded && (nonAirborneGroundCheck && flatVel.magnitude < 1f || airborneGroundCheck && timeSinceLastMove >= 0.1f || stateMachine.currentState.isComplete))
        {
            stateMachine.SetState(idle);
            return;
        }


        
    }

    /// <summary>
    /// Lets race manager know that it is ready
    /// </summary>
    /// <returns></returns>
    private IEnumerator NotifyRaceManagerWhenLoaded()
    {
        // Small delay to make sure everything's initialized (optional, but often useful)
        yield return new WaitForSeconds(0.5f);

        while (RaceManager.Instance == null)
        {
            Debug.Log("Waiting for RaceManager...");
            yield return null;
        }

        if (IsServer)
        {
            RaceManager.Instance.ResetRaceManagerValues(); // Only server resets
        }
        
        RaceManager.Instance.TeleportToStartServerRpc();
        Debug.Log("Player has notified server they are loaded.");
    }
    
    #endregion
    
    #region Public Methods


    /// <summary>
    /// Enables player input
    /// </summary>
    public void EnablePlayerInput()
    {
        playerInput.enabled = true;
    }
    /// <summary>
    /// Disables player input
    /// </summary>
    public void DisablePlayerInput()
    {
        playerInput.enabled = false;
    }

    /// <summary>
    /// Method we use to move so that we can simulate physics
    /// </summary>
    public void Move(PlayerInput.InputValues inputValues)
    {
        // Calls update logic in the currently active state
        stateMachine.currentState.DoUpdateBranch();
        timeSinceLastGrounded += Time.deltaTime;

        if (groundSensor.grounded)
        {
            timeSinceLastGrounded = 0;
        }


        if (!groundSensor.grounded)
        {
            leavingGround = false;
        }
        
        // State transitions
        HandleTransitions(inputValues);
        
        
        // Old FixedUpdate logic
        if (rb.linearVelocity.y > 0)
        {
            // Simulate custom gravity
            rb.AddForce(Vector3.down * stats.CurrentGravity, ForceMode.Force);
        }
        else
        {
            rb.AddForce(Vector3.down * stats.CurrentGravity * stats.FallingGravityMultiplier, ForceMode.Force);
        }
        // Call FixedUpdate logic
        jumpManager.TickUpdate(inputValues);
        walljumpManager.TickUpdate(inputValues);
        stateMachine.currentState.DoTickUpdateBranch(inputValues);
    }
    
    /// <summary>
    /// Changes the custom gravity scale that the player is currently experiencing
    /// </summary>
    /// <param name="gravity"></param>
    public void ChangeGravity(float gravity)
    {
        stats.CurrentGravity = gravity;
    }
    
    public float TriggerDeathScreen(int index)
    {
        if (!IsOwner) return -1f;
        return playerUI.deathScreenManager.PlayDeathScreen(index);
    }

    public void SetSpawnPoint(Vector3 position)
    {
        spawnPos = position;
    }

    public void RespawnPlayer()
    {
        float respawnDelay = TriggerDeathScreen(Random.Range(0, playerUI.deathScreenManager.GetNumberOfDeathScreens()));

        if (respawnDelay < 0f) return;
        
        StopAllCoroutines();
        StartCoroutine(TeleportDuringDeathScreen(respawnDelay));
    }

    private IEnumerator TeleportDuringDeathScreen(float respawnDelay)
    {

        
        yield return new WaitForSeconds(respawnDelay);

        float originalTrailTime = trail.time;
        trail.enabled = false;
        trail.time = -1f;
        trail.emitting = false;
        
        
        payloadManager.TeleportPlayer(spawnPos);
        
        

        // Wait another frame so Unity drops the old geometry
        yield return new WaitForSeconds(0.5f);
        
        // Restore trail
        trail.emitting = true;
        trail.enabled = true;
        trail.time = 0f;
        
        while (trail.time < originalTrailTime)
        {
            yield return null;
            trail.time += Time.deltaTime;
        }
        
    }
    
    #endregion
    
    #region Debug Methods
    /// <summary>
    /// DEBUG METHOD. Resets the player's position
    /// </summary>
    private void ResetPlayer()
    {
        rb.linearVelocity = Vector3.zero;
        rb.transform.position = spawnPos;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
#if UNITY_EDITOR
        if (Application.isPlaying && IsOwner)
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2.25f, rb.linearVelocity + "\n" + flatVel.magnitude, style);
        
        }
#endif
    }
     #endregion
    
}