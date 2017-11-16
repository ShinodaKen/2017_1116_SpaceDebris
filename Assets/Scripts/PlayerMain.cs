using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    [SerializeField]
    protected GameObject m_player;
    protected Animator m_playerAnimator;

    [SerializeField]
    protected Transform m_sceneRoot;

    protected GameObject m_grabedObject;

    [SerializeField]
    protected bool m_isGrabing = false;

    [SerializeField]
    protected bool m_isFlying = false;

    protected float m_orgMass;

    protected bool m_isPause = false;

    // Use this for initialization
    void Start()
    {
        attachRigidbody();
        m_orgMass = GetComponent<Rigidbody>().mass;
        m_playerAnimator = m_player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Globals.GetInstance().m_bPauseGame) return;

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

        if (rigidbody == null)
        {
            attachRigidbody();
            return;
        }

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (m_grabedObject)
            {
                StartCoroutine(ungrabDebris());
            }
        }

        Vector3 angularVelocity = rigidbody.angularVelocity;
        angularVelocity.x = angularVelocity.z = 0;

        Vector3 velocity = rigidbody.velocity;
        velocity.z = 0;

        if (!m_isFlying)
        {
            const float minVelocity = 0.1f;
            const float minAngularVelocity = 0.1f;

            velocity *= (1.0f - Time.deltaTime);
            angularVelocity *= (1.0f - Time.deltaTime);

            //if (velocity.magnitude < minVelocity)
            //{
            //    velocity = Vector3.zero;
            //}
            //if (angularVelocity.magnitude < minAngularVelocity)
            //{
            //    angularVelocity = Vector3.zero;
            //}
            if ((angularVelocity.magnitude < minVelocity) && (velocity.magnitude < minAngularVelocity))
            {
                if (m_grabedObject)
                {
                    Destroy(gameObject.GetComponent<Rigidbody>());
                    m_grabedObject.transform.SetParent(m_sceneRoot);
                    m_grabedObject = null;
                }
                m_isGrabing = false;
            }
        }

        rigidbody.velocity = velocity;
        rigidbody.angularVelocity = angularVelocity;

        //Vector3 angles = transform.eulerAngles;
        //angles.x = angles.z = 0;
        //transform.eulerAngles = angles;

        //Vector3 position = transform.position;
        //position.z = 0;
        //transform.position = position;

        m_playerAnimator.SetBool("isFlying", m_isFlying);
        m_playerAnimator.SetBool("isGrabing", m_isGrabing);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Debris") return;
        if (m_isGrabing) return;

        Debug.LogFormat("{0} {1}", "Collide", collision);

        Rigidbody myRigidbody = GetComponent<Rigidbody>();

        if (myRigidbody.velocity.magnitude > 1) return;
        if (myRigidbody.angularVelocity.magnitude > 1) return;

        Rigidbody opRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        if (opRigidbody == null) return;

        Vector3 angularVelocity = opRigidbody.angularVelocity;

        myRigidbody.mass += opRigidbody.mass;
        myRigidbody.AddTorque(angularVelocity * opRigidbody.mass / myRigidbody.mass, ForceMode.VelocityChange);

        m_grabedObject = collision.gameObject;
        m_grabedObject.transform.SetParent(transform);
        m_grabedObject.GetComponent<DebrisMain>().Grabbed();

        m_isGrabing = true;
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

    protected Rigidbody attachRigidbody()
    {
        Rigidbody rigid = gameObject.AddComponent<Rigidbody>();
        if (rigid)
        {
            rigid.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rigid.useGravity = false;
        }
        return rigid;
    }

    protected IEnumerator ungrabDebris()
    {
        GameObject grabbed = m_grabedObject;
        m_grabedObject.transform.SetParent(m_sceneRoot);
        m_grabedObject.GetComponent<DebrisMain>().UnGrabbed();
        m_grabedObject = null;

        Destroy(gameObject.GetComponent<Rigidbody>());

        Rigidbody myRigidbody = gameObject.GetComponent<Rigidbody>();
        while (myRigidbody != null)
        {
            Debug.LogFormat("{0}", "Wait Destroy rigidbody");
            myRigidbody = gameObject.GetComponent<Rigidbody>();
            yield return null;
        }

        myRigidbody = gameObject.GetComponent<Rigidbody>();
        while (myRigidbody == null)
        {
            Debug.LogFormat("{0}", "Wait Attach rigidbody");
            yield return null;
        }

        Vector3 forward = m_player.transform.rotation * Vector3.forward;
        Vector3 explode_at = m_player.transform.position + forward * 0.5f;

        myRigidbody.AddExplosionForce(5.0f, explode_at, 1.0f);

        Rigidbody opRigidbody = grabbed.GetComponent<Rigidbody>();
        opRigidbody.AddExplosionForce(5.0f, explode_at, 1.0f);

        yield return null;

        m_isGrabing = false;
    }
}
