using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGDebrisMain : MonoBehaviour {

    float m_angulerVelocity;
	// Use this for initialization
	void Start () {
        m_angulerVelocity = Random.Range(-10.0f, 10.0f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 euler = transform.eulerAngles;
        euler.y += m_angulerVelocity * Time.deltaTime;
        transform.eulerAngles = euler;
    }
}
