using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMain : MonoBehaviour {
    [SerializeField]
    protected GameObject m_playerObject;

    protected Vector3 m_positionOffset = Vector3.zero;
	// Use this for initialization
	void Start ()
    {
        m_positionOffset = transform.position - m_playerObject.transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 pos = m_positionOffset + m_playerObject.transform.position;
        transform.position = pos;
	}
}
