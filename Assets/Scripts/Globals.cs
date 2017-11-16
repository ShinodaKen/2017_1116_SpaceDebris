﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals
{
    public bool m_bPauseGame { get; set; }
    public bool m_bStartGame { get; set; }
    public bool m_bStartResult { get; set; }
    public bool m_bEndResult { get; set; }
    public bool m_bStartGameOver { get; set; }
    public bool m_bEndGameOver { get; set; }

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
            float m = 1.0f;
            float d = 1.0f;
            float r = 1.0f;
            float total = m_mass * m + m_difficulty * d + m_rare * r - m_cost + m_etc;
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