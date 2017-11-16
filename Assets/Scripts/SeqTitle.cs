using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SeqTitle : MonoBehaviour
{
    [SerializeField]
    protected Animator m_animator;

    [SerializeField]
    protected GameObject m_mainMenu;

    // Use this for initialization
    void Start ()
    {
        StartCoroutine(switchAnimation());
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    IEnumerator switchAnimation()
    {
        float time = 0;
        while (true)
        {
            time += Time.deltaTime;

            if ((time > 5) && (Random.Range(0, 100) > 97) && m_animator.GetCurrentAnimatorStateInfo(0).IsName("Sit Down Hold"))
            {
                Debug.Log("switch");
                m_animator.SetBool("ToIdle", true);
            }

            if (m_animator.GetBool("ToIdle") && m_animator.GetCurrentAnimatorStateInfo(0).IsName("Sit Down Idle"))
            {
                Debug.Log("to false");
                m_animator.SetBool("ToIdle", false);
                time = 0;
            }
            yield return null;
        }
    }

    public void OnClickGameStart()
    {
        m_mainMenu.SetActive(false);

        Globals.GetInstance().m_bStartGame = true;
    }
}
