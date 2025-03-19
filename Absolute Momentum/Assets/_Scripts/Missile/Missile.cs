using System;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class Missile : MonoBehaviour
{
    /// <summary>
    /// Units of degrees per second
    /// </summary>
    [Header("Missile Stats")]
    [SerializeField] private AnimationCurve GuidanceCurve;
    [SerializeField] private AnimationCurve AccelerationCurve;
    public float StartingVelocity;
    /// <summary>
    /// Smart guidance has the missile account for target velocity
    /// </summary>
    [SerializeField] private bool HasSmartGuidance;

    [SerializeField] private float ExplosionRadius;
    
    /// <summary>
    /// Units of seconds
    /// </summary>
    [SerializeField] private float MissileLifeTime;
    private float CurrentLifeTime;
    
    [Header("Missile Properties")]
    [SerializeField] private GameObject missile;
    private Rigidbody missileRigidbody;
    private Rigidbody targetRigidbody;
    public GameObject target;
    public GameObject owner;

    [SerializeField] private Vector3 targetOffsetPosition = Vector3.up * 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentLifeTime = 0;
        missileRigidbody = missile.GetComponent<Rigidbody>();

        if (target == null || !target.TryGetComponent(out targetRigidbody))
        {
            // kill self;
            Debug.Log($"no target: {target == null} || {!target.TryGetComponent(out targetRigidbody)}");
            Destroy(missile);
        }
        missileRigidbody.useGravity = false;
        missileRigidbody.linearVelocity = missile.transform.forward * StartingVelocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CurrentLifeTime += Time.fixedDeltaTime;
        if (CurrentLifeTime > MissileLifeTime)
        {
            Explode();
        }
        
        UpdateDirection(GetTargetPosition());
    }

    private Vector3 GetTargetPosition()
    {
        if (target == null || targetRigidbody == null)
        {
            return missile.transform.position + missile.transform.forward;
        }
        
        if (HasSmartGuidance)
        {
            Vector3 relativePosition = targetRigidbody.position + targetOffsetPosition - missile.transform.position;
            Vector3 relativeVelocity = targetRigidbody.linearVelocity - missileRigidbody.linearVelocity;

            float timeToIntercept = relativePosition.magnitude / relativeVelocity.magnitude;

            return targetRigidbody.position + targetRigidbody.linearVelocity * timeToIntercept;
        }
        
        return targetRigidbody.position;
    }

    private void UpdateDirection(Vector3 targetPredictionLocation)
    {
        Quaternion idealRotation = Quaternion.LookRotation(targetPredictionLocation - transform.position);
        float AimSpeed = Time.fixedDeltaTime * GuidanceCurve.Evaluate(Time.time);
        
        missile.transform.rotation = Quaternion.RotateTowards(missile.transform.rotation, idealRotation, AimSpeed); // why is it all degrees aaaaaaaaaaaaaaaaaa
        
        missileRigidbody.linearVelocity = missile.transform.forward * (missileRigidbody.linearVelocity.magnitude + AccelerationCurve.Evaluate(Time.time) * Time.fixedDeltaTime);
    }
    

    private void OnCollisionStay(Collision other)
    {
        // ignore owner and other projectiles
        if (other.rigidbody != owner.GetComponent<Rigidbody>() && !other.gameObject.CompareTag("Projectile"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        // todo: particle effect
        
        Collider[] colliders = Physics.OverlapSphere(missile.transform.position, ExplosionRadius);

        foreach (var c in colliders)
        {
            if (c.gameObject.CompareTag("Player") &&
                c.gameObject.GetComponent<Rigidbody>() != owner.GetComponent<Rigidbody>())
            {
                // todo: punish hit players
            }
        }
        
        
        
        Destroy(missile);
    }
}
