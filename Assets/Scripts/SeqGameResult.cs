using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SeqGameResult : MonoBehaviour {

    [SerializeField]
    protected GameObject m_player;
    protected Animator m_playerAnimator;

    [SerializeField]
    protected Text m_textValueMass;
    [SerializeField]
    protected Text m_textValueDifficulty;
    [SerializeField]
    protected Text m_textValueRare;
    [SerializeField]
    protected Text m_textValueCost;
    [SerializeField]
    protected Text m_textValueEtc;
    [SerializeField]
    protected Text m_textValueTotal;

    // Use this for initialization
    void Start ()
    {
        m_playerAnimator = m_player.GetComponent<Animator>();
        int animationId = Globals.GetInstance().m_bGameFail ? 3 : Random.Range(0, 100) % 2;
        m_playerAnimator.SetInteger("AnimationId", animationId);

        m_textValueMass.text = string.Format("{0:0.00} pts", Globals.GetInstance().m_mass);
        m_textValueDifficulty.text = string.Format("{0:0.00} pts", Globals.GetInstance().m_difficulty);
        m_textValueRare.text = string.Format("{0:0.00} pts", Globals.GetInstance().m_rare);
        m_textValueCost.text = string.Format("$ {0:0.00}", Globals.GetInstance().m_cost);
        m_textValueEtc.text = string.Format("$ {0:0.00}", Globals.GetInstance().m_etc);
        m_textValueTotal.text = string.Format("$ {0:0.00}", Globals.GetInstance().m_total);
    }

    // Update is called once per frame
    void Update ()
    {
        if (!m_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            m_playerAnimator.SetLayerWeight(1, 1.0f);
        }
    }

    public void OnClickNext()
    {
        SceneManager.UnloadSceneAsync("GameResult");
    }
}
