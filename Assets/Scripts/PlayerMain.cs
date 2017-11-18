using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    protected Slider m_airSlider;
    [SerializeField]
    protected Image m_airSliderFill;
    [SerializeField]
    protected Text m_airText;

    [SerializeField]
    protected Slider m_hightSlider;

    [SerializeField]
    protected ParticleSystem m_ptclBack;
    [SerializeField]
    protected ParticleSystem m_ptclFront;
    [SerializeField]
    protected ParticleSystem m_ptclTop;
    [SerializeField]
    protected ParticleSystem m_ptclBottom;
    [SerializeField]
    protected ParticleSystem m_ptclLeft;
    [SerializeField]
    protected ParticleSystem m_ptclRight;

    protected float m_air = 100.0f;
    protected float m_airSupply = 0;
    protected float m_gameoverTimer = 0;
    protected float m_playTimer = 0;

    protected float m_orgMass;

    protected bool m_isPause = false;

    const float minVelocity = 0.1f;
    const float minAngularVelocity = 0.1f;

    [SerializeField]
    GameObject m_titaniumDebrisPrefab;
    [SerializeField]
    GameObject m_goldDebrisPrefab;
    [SerializeField]
    GameObject m_copperDebrisPrefab;
    [SerializeField]
    GameObject m_ironDebrisPrefab;

    // Use this for initialization
    void Start()
    {
        attachRigidbody();
        m_orgMass = GetComponent<Rigidbody>().mass;
        m_playerAnimator = m_player.GetComponent<Animator>();

        createDebris();
    }

    // Update is called once per frame
    void Update()
    {
        if (Globals.GetInstance().m_bPauseGame) return;

        float v = 1.0f - Mathf.Clamp01((transform.position.y + 50.0f) / 100.0f);
        m_hightSlider.value = v;
        m_playTimer += Time.deltaTime;

        float air = 0.1f;
        float f = 1.0f;

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

        if (m_air == 0)
        {
            //GameOver
            m_gameoverTimer += Time.deltaTime;
            if ( m_gameoverTimer > 1.0f)
            {
                Globals.GetInstance().m_cost = (100 - m_air) * 10.0f + m_playTimer;
                Globals.GetInstance().m_bStartGameOver = true;
            }
            return;
        }

        if (Input.GetMouseButton(0))
        {
            d_force += forward * f * 2;
            //rigidbody.AddForceAtPosition(forward * f, f_at);
            air += 2.0f;
            m_ptclBack.gameObject.SetActive(true);
            if (!m_ptclBack.isPlaying) m_ptclBack.Play();
        }
        if (Input.GetMouseButton(1))
        {
            d_force -= forward * f;
            //rigidbody.AddForceAtPosition(-forward * f, f_at);
            air += 1.0f;
            m_ptclFront.gameObject.SetActive(true);
            if (!m_ptclFront.isPlaying) m_ptclFront.Play();
        }
        if (Input.GetKey(KeyCode.W))
        {
            d_force -= up * f;
            //rigidbody.AddForceAtPosition(-up * f, f_at);
            air += 1.0f;
            m_ptclTop.gameObject.SetActive(true);
            if (!m_ptclTop.isPlaying) m_ptclTop.Play();
        }
        if (Input.GetKey(KeyCode.S))
        {
            d_force += up * f;
            //rigidbody.AddForceAtPosition(up * f, f_at);
            air += 1.0f;
            m_ptclBottom.gameObject.SetActive(true);
            if (!m_ptclBottom.isPlaying) m_ptclBottom.Play();
        }
        if (Input.GetKey(KeyCode.A))
        //if (Input.GetKey(KeyCode.Q))
        {
            d_force -= right * f;
            //rigidbody.AddForceAtPosition(-right * f, f_at);
            air += 1.0f;
            m_ptclRight.gameObject.SetActive(true);
            if (!m_ptclRight.isPlaying) m_ptclRight.Play();
        }
        if (Input.GetKey(KeyCode.D))
        //if (Input.GetKey(KeyCode.E))
        {
            d_force += right * f;
            //rigidbody.AddForceAtPosition(right * f, f_at);
            air += 1.0f;
            m_ptclLeft.gameObject.SetActive(true);
            if (!m_ptclLeft.isPlaying) m_ptclLeft.Play();
        }
        rigidbody.AddForceAtPosition(d_force, f_at);

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
            air = -1.0f; //増やす
            m_airSupply = Time.deltaTime * 0.4f;
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
            if ((angularVelocity.magnitude < minAngularVelocity) && (velocity.magnitude < minVelocity))
            {
                if (m_grabedObject)
                {
                    DebrisMain debris = m_grabedObject.GetComponent<DebrisMain>();
                    Globals.GetInstance().m_mass = debris.GetMass();
                    Globals.GetInstance().m_difficulty = debris.GetDistance();
                    Globals.GetInstance().m_rare = debris.GetRare();
                    Globals.GetInstance().m_cost = (100 - m_air + m_airSupply) * 10.0f + m_playTimer;
                    Globals.GetInstance().m_bStartResult = true;

                    Destroy(gameObject.GetComponent<Rigidbody>());
                    m_grabedObject.transform.SetParent(m_sceneRoot);
                    m_grabedObject = null;
                }
                m_isGrabing = false;
            }
        }

        m_air -= (air * Time.deltaTime * 0.4f);
        m_air = Mathf.Clamp(m_air, 0.0f, 100);

        m_airSlider.value = m_air / 100.0f;
        m_airText.text = string.Format("{0:0.00} %", m_air);

        if ( m_air < 20 )
        {
            m_airSliderFill.color = new Color(192.0f/255.0f, 0, 0);
        }

        if (m_air == 0)
        {
            if (m_grabedObject)
            {
                //StartCoroutine(ungrabDebris());
            }
            m_playerAnimator.SetBool("isFail", true);
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
        //if (collision.gameObject.tag != "Debris") return;
        //if (m_isGrabing) return;

        //Debug.LogFormat("{0} {1}", "Collide", collision);

        //Rigidbody myRigidbody = GetComponent<Rigidbody>();

        //if (myRigidbody.velocity.magnitude > 1) return;
        //if (myRigidbody.angularVelocity.magnitude > 1) return;

        //Rigidbody opRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        //if (opRigidbody == null) return;

        //Vector3 angularVelocity = opRigidbody.angularVelocity;

        //myRigidbody.mass += opRigidbody.mass;
        //myRigidbody.AddTorque(angularVelocity * opRigidbody.mass / myRigidbody.mass, ForceMode.VelocityChange);

        //m_grabedObject = collision.gameObject;
        //m_grabedObject.transform.SetParent(transform);
        //m_grabedObject.GetComponent<DebrisMain>().Grabbed();

        //m_isGrabing = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Debris")
        {
            if (m_isGrabing) return;

            Debug.LogFormat("{0} {1}", "Collide", other);

            Rigidbody myRigidbody = GetComponent<Rigidbody>();

            Rigidbody opRigidbody = other.gameObject.GetComponent<Rigidbody>();
            if (opRigidbody == null) return;

            if (Mathf.Abs((myRigidbody.velocity- opRigidbody.velocity).magnitude) > 1) return;
            if (myRigidbody.angularVelocity.magnitude > 1) return;

            Vector3 angularVelocity = opRigidbody.angularVelocity;

            myRigidbody.mass += opRigidbody.mass;
            myRigidbody.AddTorque(angularVelocity * opRigidbody.mass / myRigidbody.mass, ForceMode.VelocityChange);

            m_grabedObject = other.gameObject;
            m_grabedObject.transform.SetParent(transform);
            m_grabedObject.GetComponent<DebrisMain>().Grabbed();

            m_isGrabing = true;
        }
        else if (other.gameObject.tag != "Floor")
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            Vector3 angularVelocity = rigidbody.angularVelocity;
            Vector3 velocity = rigidbody.velocity;
            if ((angularVelocity.magnitude < minAngularVelocity) && (velocity.magnitude < minVelocity))
            {
                m_isFlying = false;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Floor") return;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        Vector3 angularVelocity = rigidbody.angularVelocity;
        Vector3 velocity = rigidbody.velocity;
        if ((angularVelocity.magnitude < minAngularVelocity) && (velocity.magnitude < minVelocity))
        {
            m_isFlying = false;
        }

        angularVelocity *= (1.0f - Time.deltaTime);
        rigidbody.angularVelocity = angularVelocity;
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

    protected void createDebris()
    {
        int num = Random.Range(50, 100);
        for ( int i = 0; i < num; ++i )
        {
            float distance = 0;
            GameObject pref = null;
            float rare_random = Random.Range(0, 100);
            if (rare_random < 5)
            {
                distance = Random.Range(150.0f, 500.0f);
                pref = m_titaniumDebrisPrefab;
            }
            else if (rare_random < 15)
            {
                distance = Random.Range(100.0f, 400.0f);
                pref = m_goldDebrisPrefab;
            }
            else if (rare_random < 45)
            {
                distance = Random.Range(100.0f, 200.0f);
                pref = m_copperDebrisPrefab;
            }
            else
            {
                distance = Random.Range(20.0f, 200.0f);
                pref = m_ironDebrisPrefab;
            }

            float theta = Random.Range(-15.0f, 15.0f) + (Random.Range(0, 100) < 50 ? 0 : 180.0f);
            Vector3 pos = new Vector3(Mathf.Cos(theta / 180.0f * Mathf.PI), Mathf.Sin(theta / 180.0f * Mathf.PI), 0) * distance;
            Vector3 euler = new Vector3(Random.Range(0, 30), Random.Range(0, 30), 0);
            Vector3 velocity = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            Vector3 scale = new Vector3(Random.Range(0.5f, 1.0f), Random.Range(0.5f, 2f), Random.Range(0.5f, 1.0f));
            Vector3 angularVelocity = new Vector3(0, Random.Range(0, 2), 0);

            Quaternion rot = Quaternion.Euler(euler);
            GameObject obj = Instantiate(pref, pos, rot, transform.parent);
            obj.transform.localScale = scale;
            DebrisMain debris = obj.GetComponent<DebrisMain>();
            debris.m_anglarVelocity = angularVelocity;
            debris.m_velocity = velocity;
        }
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
