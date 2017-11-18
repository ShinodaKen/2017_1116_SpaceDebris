using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals
{
    public string m_userId { get; set; }
    public string m_userName { get; set; }
    public float m_savedMass { get; set; }
    public float m_savedDifficulty { get; set; }
    public float m_savedRare { get; set; }
    public float m_savedCost { get; set; }
    public float m_savedEtc { get; set; }
    public float m_savedTotal { get; set; }
    public float m_savedTotalAll { get; set; }

    public bool m_bPauseGame { get; set; }
    public bool m_bStartGame { get; set; }
    public bool m_bStartResult { get; set; }
    public bool m_bEndResult { get; set; }
    public bool m_bStartGameOver { get; set; }
    public bool m_bEndGameOver { get; set; }
    public bool m_bRetryGame { get; set; }

    public bool m_bFadeIn { get; set; } // true:fade in, false:fade out
    public bool m_bFadeEnd { get; set; }

    public bool m_bGameFail { get; set; }

    public float m_mass { get; set; }
    public float m_difficulty { get; set; }
    public float m_rare { get; set; }
    public float m_cost { get; set; }
    public float m_etc { get; set; }
    public float m_total
    {
        get
        {
            float m = 4.0f;
            float d = 2.0f;
            float r = 1.0f;
            float total = Mathf.Pow(m_mass * m, m_rare * r) + m_difficulty * d - m_cost + m_etc;
            return total;
        }
    }

    static Globals instance;

    public static Globals GetInstance()
    {
        if (instance == null) instance = new Globals();
        return instance;
    }
}