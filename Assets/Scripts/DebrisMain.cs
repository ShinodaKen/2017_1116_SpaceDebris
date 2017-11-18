using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisMain : MonoBehaviour {
    [SerializeField]
    public Vector3 m_velocity = Vector3.zero;
    [SerializeField]
    public Vector3 m_anglarVelocity = Vector3.zero;
    [SerializeField]
    public float m_density;
    [SerializeField]
    public float m_baseRarity;
    protected Rigidbody m_rigidbody;
    protected float m_mass;
    protected float m_distance;
    protected float m_rare;

	// Use this for initialization
	void Start ()
    {
        attachRigidbody();
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.AddRelativeTorque(m_anglarVelocity, ForceMode.VelocityChange);
        m_rigidbody.velocity = m_velocity;

        m_mass = m_rigidbody.mass;
        m_distance = transform.position.magnitude;
        m_rare = m_baseRarity + ((m_distance / 500.0f) * Random.Range(0.5f, 1.0f)) + Mathf.Sqrt(transform.localScale.magnitude);
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

    public float GetMass() { return m_mass; }
    public float GetDistance() { return m_distance; }
    public float GetRare() { return m_rare; }
}
