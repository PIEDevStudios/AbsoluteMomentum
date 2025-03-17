using System;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.Missile
{
    public class TestTarget : MonoBehaviour
    {
        public Vector3 VecStart;
        public Vector3 End;
        [SerializeField] private GameObject self;
        public float Velocity;
        private bool StartToEnd = true;

        private void Start()
        {
            GameManager.instance.Players.Add(self);
        }

        private void FixedUpdate()
        {
            if (Vector3.Distance(self.transform.position, End) < 5 && StartToEnd)
            {
                StartToEnd = false;
            }
            else if (Vector3.Distance(self.transform.position, VecStart) < 5 && !StartToEnd)
            {
                StartToEnd = true;
            }
            
            if (StartToEnd)
            {
                self.GetComponent<Rigidbody>().linearVelocity = (End - self.transform.position).normalized * Velocity;
            }
            else
            {
                self.GetComponent<Rigidbody>().linearVelocity = (VecStart - self.transform.position).normalized * Velocity;
            }
        }
    }
}