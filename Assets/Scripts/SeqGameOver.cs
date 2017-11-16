using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SeqGameOver : MonoBehaviour
{
    [SerializeField]
    protected Animator m_animator;

    protected float m_timer = 0;
	// Use this for initialization
	IEnumerator Start ()
    {
        while (m_timer < 4)
        {
            m_timer += Time.deltaTime;
            yield return null;
        }
        SceneManager.UnloadSceneAsync("GameOver");
    }
}
