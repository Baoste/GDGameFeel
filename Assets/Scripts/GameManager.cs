using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void StartGame()
    {
        StartCoroutine(WaitForSecondsToStartGame(1f));
    }
    private IEnumerator WaitForSecondsToStartGame(float t)
    {
        yield return new WaitForSeconds(t);
        SceneManager.LoadScene("MapScene");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }


    public void StartLevel(string number)
    {
        StartCoroutine(WaitForSecondsToStartLevel(1f, number));
    }
    private IEnumerator WaitForSecondsToStartLevel(float t, string number)
    {
        yield return new WaitForSeconds(t);
        SceneManager.LoadScene("Level" + number + "Scene");
    }

    void OnApplicationQuit()
    {
        //System.Diagnostics.Process.GetCurrentProcess().Kill();
    }
}
