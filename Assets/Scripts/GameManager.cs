using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void StartGame()
    {
        StartCoroutine(WaitForSeconds(1f));
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
        SceneManager.LoadScene("StartScene");
    }

    private IEnumerator WaitForSeconds(float t)
    {
        yield return new WaitForSeconds(t);
        SceneManager.LoadScene("MainScene");
    }

    void OnApplicationQuit()
    {
        //System.Diagnostics.Process.GetCurrentProcess().Kill();
    }
}
