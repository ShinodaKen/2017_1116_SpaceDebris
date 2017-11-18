using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class SeqGameResult : MonoBehaviour {

    [SerializeField]
    protected GameObject m_player;
    protected Animator m_playerAnimator;

    [SerializeField]
    protected GameObject[] m_panelRoot;
    [SerializeField]
    protected GameObject[] m_controlRoot;

    [SerializeField]
    protected Toggle m_toggleSaveResult;
    [SerializeField]
    protected InputField m_inputFieldName;
    [SerializeField]
    protected Text m_textNetworkStatus;
    [SerializeField]
    protected Dropdown m_dropdownRanking;
    [SerializeField]
    protected Text m_textRankingNoList;
    [SerializeField]
    protected Text m_textRankingNameList;
    [SerializeField]
    protected Text m_textRankingValueList;
    [SerializeField]
    protected GameObject m_scrollView;
    [SerializeField]
    protected GameObject m_objectMine;
    [SerializeField]
    protected GameObject m_objectAll;
    [SerializeField]
    protected Text m_textMyValue;
    [SerializeField]
    protected Text m_textMyRank;
    [SerializeField]
    protected Text m_textAllValue;

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

        m_inputFieldName.text = "吉田さん";
        m_textNetworkStatus.text = "";

        m_panelRoot[0].SetActive(true);
        m_panelRoot[1].SetActive(false);
        m_controlRoot[0].SetActive(true);
        m_controlRoot[1].SetActive(false);
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
        m_controlRoot[0].SetActive(false);
        StartCoroutine(transitionToRanking());
    }

    public void OnClickRetry()
    {
        Globals.GetInstance().m_bRetryGame = true;
        SceneManager.UnloadSceneAsync("GameResult");
    }

    public void OnClickReturnTitle()
    {
        Globals.GetInstance().m_bRetryGame = false;
        SceneManager.UnloadSceneAsync("GameResult");
        StartCoroutine(returnTitle());
    }

    IEnumerator transitionToRanking()
    {
        if (m_toggleSaveResult.isOn)
        {
            Globals.GetInstance().m_userName = m_inputFieldName.text;
            yield return StartCoroutine(saveResult());
        }

        m_panelRoot[0].SetActive(false);
        m_panelRoot[1].SetActive(true);

        m_textNetworkStatus.text = "ランキングデータ取得中";
        string[] category = { "total", "mass", "difficulty", "rare", "cost", "etc", "totalAll", "All" };

        var query = new GSSA.SpreadSheetQuery("Ranking");
        yield return query.OrderByDescending("total").Limit(50).FindAsync();
        m_controlRoot[1].SetActive(true);
        m_objectMine.SetActive(true);
        m_objectAll.SetActive(false);

        makeRanking(query, 0);

        m_textNetworkStatus.text = "";


        int currentOption = 0;
        while (true)
        {
            if (currentOption != m_dropdownRanking.value)
            {
                currentOption = m_dropdownRanking.value;

                m_textNetworkStatus.text = "ランキングデータ取得中";
                if (currentOption == 7)
                {
                    m_scrollView.SetActive(false);
                    yield return query.Where("total", GSSA.SpreadSheetQuery.CompareData.CompareType.NE, 0).FindAsync();
                    m_objectMine.SetActive(false);
                    m_objectAll.SetActive(true);

                    float total = 0;
                    foreach (var so in query.Result)
                    {
                        total += float.Parse(so["totalAll"] as string);
                    }
                    m_textAllValue.text = "$ " + total as string;
                }
                else
                {
                    m_scrollView.SetActive(true);
                    m_textRankingNoList.text = "";
                    m_textRankingNameList.text = "";
                    m_textRankingValueList.text = "";

                    yield return query.OrderByDescending(category[currentOption]).Limit(50).FindAsync();
                    m_objectMine.SetActive(true);
                    m_objectAll.SetActive(false);

                    makeRanking(query, currentOption);

                    m_textNetworkStatus.text = "";
                }
            }
            yield return null;
        }
    }

    IEnumerator saveResult()
    {
        m_textNetworkStatus.text = "データアップロード中";

        var query = new GSSA.SpreadSheetQuery("Ranking");
        query.Where("id", "=", Globals.GetInstance().m_userId);
        yield return query.FindAsync();

        var so = query.Count > 0 ? query.Result.First() : new GSSA.SpreadSheetObject("Ranking");
        so["id"] = Globals.GetInstance().m_userId;
        so["name"] = Globals.GetInstance().m_userName;
        so["mass"] = Globals.GetInstance().m_savedMass;
        so["difficulty"] = Globals.GetInstance().m_savedDifficulty;
        so["rare"] = Globals.GetInstance().m_savedRare;
        so["cost"] = Globals.GetInstance().m_savedCost;
        so["etc"] = Globals.GetInstance().m_savedEtc;
        so["total"] = Globals.GetInstance().m_savedTotal;
        so["totalAll"] = Globals.GetInstance().m_savedTotalAll;
        yield return so.SaveAsync();

        m_textNetworkStatus.text = "";

        m_panelRoot[0].SetActive(false);
    }

    IEnumerator returnTitle()
    {
        yield return null;
    }

    private void makeRanking(GSSA.SpreadSheetQuery query, int option)
    {
        string[] category = { "total", "mass", "difficulty", "rare", "cost", "etc", "totalAll", "All" };
        string[] prefix = { "$", "", "", "", "$", "$", "$" };
        string[] postfix = { "", "kg", "pts", "pts", "", "", "", "" };
        float[] myValues =
        {
            Globals.GetInstance().m_savedTotal,
            Globals.GetInstance().m_savedMass,
            Globals.GetInstance().m_savedDifficulty,
            Globals.GetInstance().m_savedRare,
            Globals.GetInstance().m_savedCost,
            Globals.GetInstance().m_savedEtc,
            Globals.GetInstance().m_savedTotalAll,
        };

        int lineNum = 0;

        int rank = 1;
        float prevValue = 0;
        int myRank = 0;
        foreach (var so in query.Result)
        {
            lineNum++;
            float value = float.Parse(so[category[option]].ToString());
            if (value < prevValue)
            {
                rank = lineNum;
            }
            if ((myRank == 0) && (value <= myValues[option]))
            {
                myRank = rank;
            }
            prevValue = value;
            m_textRankingNoList.text += rank.ToString() + "\n";
            m_textRankingNameList.text += so["name"] as string + "\n";
            m_textRankingValueList.text += prefix[option] + " " + string.Format("{0:0.00}", value.ToString()) + " " + postfix[option] + "\n";
        }

        m_textMyRank.text = (myRank == 0) ? "ランキング圏外" : myRank.ToString() + "位";
        m_textMyValue.text = prefix[option] + " " + string.Format("{0:0.00}", myValues[option]) + " " + postfix[option];
    }
}
