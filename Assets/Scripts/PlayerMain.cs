using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour 
{
    [SerializeField]
    protected GameObject m_player;
    private Animator m_playerAnimator;

    [SerializeField]
    protected bool m_bGrab = false;

    [SerializeField]
    protected bool m_isFlying = false;

    // Use this for initialization
    void Start()
    {
        m_playerAnimator = m_player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float mass = 1.0f;
        float f = 1.0f;
        float t = 1.0f;

        Vector3 d_force = Vector3.zero;
        Vector3 r_force = Vector3.zero;

        Vector3 forward = m_player.transform.rotation * Vector3.forward;
        Vector3 right = m_player.transform.rotation * Vector3.right;
        Vector3 up = m_player.transform.rotation * Vector3.up;

        Vector3 g_pos = transform.position;
        Vector3 f_at = m_player.transform.position - forward * 0.5f;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (Input.GetKey(KeyCode.A))
        {
            d_force += forward * f;
            //            rigidbody.AddForce(forward * f);
            rigidbody.AddForceAtPosition(forward * f, f_at);
        }
        if (Input.GetKey(KeyCode.D))
        {
            d_force -= forward * f;
            //            rigidbody.AddForce(-forward * f);
            rigidbody.AddForceAtPosition(-forward * f, f_at);
        }
        if (Input.GetKey(KeyCode.W))
        {
            d_force -= up * f;
            //            rigidbody.AddForce(-up * f);
            rigidbody.AddForceAtPosition(-up * f, f_at);
        }
        if (Input.GetKey(KeyCode.S))
        {
            d_force += up * f;
            //            rigidbody.AddForce(up * f);
            rigidbody.AddForceAtPosition(up * f, f_at);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            //rigidbody.AddRelativeTorque(0, t, 0);
            //Vector3 dir = transform.rotation * Vector3.right;
            rigidbody.AddForceAtPosition(-right * f, f_at);
        }
        if (Input.GetKey(KeyCode.E))
        {
            //rigidbody.AddRelativeTorque(0, -t, 0);
            rigidbody.AddForceAtPosition(right * f, f_at);
        }

        Vector3 anglarVelocity = rigidbody.angularVelocity;
        anglarVelocity.x = anglarVelocity.z = 0;
        rigidbody.angularVelocity = anglarVelocity;

        Vector3 velocity = rigidbody.velocity;
        velocity.z = 0;
        rigidbody.velocity = velocity;

        Vector3 angles = transform.eulerAngles;
        angles.x = angles.z = 0;
        //transform.eulerAngles = angles;

        Vector3 position = transform.position;
        position.z = 0;
        //transform.position = position;

        if (m_isFlying && (anglarVelocity.magnitude < 0.1f) && (velocity.magnitude < 0.1f))
        {
            m_bGrab = false;
        }
        m_playerAnimator.SetBool("isFlying", m_isFlying);
        m_playerAnimator.SetBool("isGrabing", m_bGrab);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Debris") return;
        if (m_bGrab) return;

        Debug.LogFormat("{0}", "Collide");

        Rigidbody myRigidbody = GetComponent<Rigidbody>();

        if (myRigidbody.velocity.magnitude > 1) return;
        if (myRigidbody.angularVelocity.magnitude > 1) return;

        Rigidbody opRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        Vector3 angularVelocity = opRigidbody.angularVelocity;

        myRigidbody.mass += opRigidbody.mass;
        //opRigidbody.isKinematic = true;
        Destroy(opRigidbody);
        collision.gameObject.transform.SetParent(transform);

        myRigidbody.AddTorque(angularVelocity * opRigidbody.mass / myRigidbody.mass, ForceMode.VelocityChange);

        m_bGrab = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Floor") return;
        m_isFlying = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Floor") return;
        m_isFlying = false;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Floor") return;
        m_isFlying = true;
    }
}
