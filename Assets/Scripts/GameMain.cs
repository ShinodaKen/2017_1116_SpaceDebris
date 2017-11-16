using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMain : MonoBehaviour
{
    const int m_GameMainSceneId = 1;
    public bool m_dbgStartGameover = false;
    public bool m_dbgStartResult = false;

    // Use this for initialization
    void Start ()
    {
        SceneManager.LoadSceneAsync("Title", LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update ()
    {
        if (m_dbgStartGameover)
        {
            Globals.GetInstance().m_bStartGameOver = true;
            m_dbgStartGameover = false;
        }
        if (m_dbgStartResult)
        {
            Globals.GetInstance().m_bStartResult = true;
            m_dbgStartResult = false;
        }

        if (Globals.GetInstance().m_bStartGame)
        {
            Globals.GetInstance().m_bStartGame = false;
            StartCoroutine(startGame());
        }
        if (Globals.GetInstance().m_bStartGameOver)
        {
            Globals.GetInstance().m_bPauseGame = true;
            Globals.GetInstance().m_bStartGameOver = false;
            Globals.GetInstance().m_bGameFail = true;
            StartCoroutine(startGameOver());
        }
        else if (Globals.GetInstance().m_bEndGameOver)
        {
            Debug.LogFormat("{0}", "m_bEndGameOver");

            Globals.GetInstance().m_bPauseGame = false;
            Globals.GetInstance().m_bEndGameOver = false;
        }
        if (Globals.GetInstance().m_bStartResult)
        {
            Globals.GetInstance().m_bPauseGame = true;
            Globals.GetInstance().m_bStartResult = false;
            StartCoroutine(startGameResult());
        }
        else if (Globals.GetInstance().m_bEndResult)
        {
            Debug.LogFormat("{0}", "m_bEndResult");

            Globals.GetInstance().m_bPauseGame = false;
            Globals.GetInstance().m_bEndResult = false;
            Globals.GetInstance().m_bGameFail = false;
        }
    }

    private IEnumerator startGame()
    {
        Globals.GetInstance().m_bFadeIn = false;
        Globals.GetInstance().m_bFadeEnd = false;
        yield return SceneManager.LoadSceneAsync("Fade", LoadSceneMode.Additive);

        while (!Globals.GetInstance().m_bFadeEnd)
        {
            yield return null;
        }

        yield return SceneManager.UnloadSceneAsync("Title");

        yield return SceneManager.UnloadSceneAsync("Fade");

        Globals.GetInstance().m_bFadeIn = true;
        Globals.GetInstance().m_bFadeEnd = false;
        yield return SceneManager.LoadSceneAsync("Fade", LoadSceneMode.Additive);

        yield return SceneManager.LoadSceneAsync(m_GameMainSceneId, LoadSceneMode.Additive);

        while (!Globals.GetInstance().m_bFadeEnd)
        {
            yield return null;
        }
    }

    private IEnumerator startGameOver()
    {
        Debug.LogFormat("{0}", "startGameOver");

        Globals.GetInstance().m_bFadeIn = false;
        Globals.GetInstance().m_bFadeEnd = false;
        yield return SceneManager.LoadSceneAsync("Fade", LoadSceneMode.Additive);

        while (!Globals.GetInstance().m_bFadeEnd)
        {
            yield return null;
        }

        if (SceneManager.GetSceneByBuildIndex(m_GameMainSceneId).isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(m_GameMainSceneId);
        }

        yield return SceneManager.UnloadSceneAsync("Fade");

        yield return SceneManager.LoadSceneAsync("GameOver", LoadSceneMode.Additive);
        while (SceneManager.sceneCount > 1)
        {
            yield return null;
        }

        Globals.GetInstance().m_bEndGameOver = true;
        Globals.GetInstance().m_bStartResult = true;
    }

    private IEnumerator startGameResult()
    {
        Debug.LogFormat("{0}", "startGameResult");

        yield return SceneManager.LoadSceneAsync("GameResult", LoadSceneMode.Additive);

        if (SceneManager.GetSceneByBuildIndex(m_GameMainSceneId).isLoaded)
        {
            SceneManager.UnloadSceneAsync(m_GameMainSceneId);
        }

        while (SceneManager.sceneCount > 1)
        {
            yield return null;
        }

        yield return SceneManager.LoadSceneAsync(m_GameMainSceneId, LoadSceneMode.Additive);

        Globals.GetInstance().m_bEndResult = true;
    }
}
