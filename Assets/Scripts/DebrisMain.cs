using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisMain : MonoBehaviour {
    [SerializeField]
    protected Vector3 m_anglarVelocity = Vector3.zero;
    protected Rigidbody m_rigidbody;
	// Use this for initialization
	void Start ()
    {
        attachRigidbody();
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.AddRelativeTorque(m_anglarVelocity, ForceMode.VelocityChange);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_rigidbody != null)
        {
            Vector3 velocity = m_rigidbody.velocity;
            velocity.z = 0;
            m_rigidbody.velocity = velocity;
        }
    }

    public void Grabbed()
    {
        Destroy(m_rigidbody);
        m_rigidbody = null;
    }

    public void UnGrabbed()
    {
        attachRigidbody();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    protected void attachRigidbody()
    {
        Rigidbody rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.mass = transform.lossyScale.sqrMagnitude;
        rigidBody.useGravity = false;
        //rigidBody.constraints.
    }
}
