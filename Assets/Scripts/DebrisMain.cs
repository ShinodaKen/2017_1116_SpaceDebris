using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisMain : MonoBehaviour {
    [SerializeField]
    protected Vector3 m_anglarVelocity = Vector3.zero;
	// Use this for initialization
	void Start ()
    {
        Rigidbody rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.mass = transform.lossyScale.sqrMagnitude;
        rigidBody.useGravity = false;
        //rigidBody.constraints.
        rigidBody.AddRelativeTorque(m_anglarVelocity, ForceMode.VelocityChange);
	}
	
	// Update is called once per frame
	void Update ()
    {
        Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
        if (rigidBody != null)
        {
            Vector3 velocity = rigidBody.velocity;
            velocity.z = 0;
            rigidBody.velocity = velocity;
        }
    }
}
