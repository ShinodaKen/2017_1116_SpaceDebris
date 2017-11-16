using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeqFade : MonoBehaviour {
    [SerializeField]
    protected Image m_fadePlane;

    protected float m_startAlpha;
    protected float m_endAlpha;
    protected float m_ratio = 0.0f;

    // Use this for initialization
    void Start ()
    {
        m_startAlpha = Globals.GetInstance().m_bFadeIn ? 1.0f : 0.0f;
        m_endAlpha = 1.0f - m_startAlpha;

        Color c = new Color(0, 0, 0, m_startAlpha);
        m_fadePlane.color = c;
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_ratio += Time.deltaTime;
        if (m_ratio > 1.0f)
        {
            Globals.GetInstance().m_bFadeEnd = true;
            m_ratio = 1.0f;
        }

        Color c = new Color(0, 0, 0, m_startAlpha * (1.0f - m_ratio) + m_endAlpha * m_ratio);
        m_fadePlane.color = c;
    }
}
