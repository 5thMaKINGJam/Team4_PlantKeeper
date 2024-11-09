using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void ShowGameManual()
    {
        Debug.Log("manual");
    }

    public static void ExitGame()
    {
#if UNITY_EDITOR // develop
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static void StartGame()
    {
        Debug.Log("start");
        SceneManager.LoadScene("SampleScene");
    }

    public static void ReturnToTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
