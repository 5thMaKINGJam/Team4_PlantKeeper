using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    Transform child;
    AudioSource audioPlayer;
    // Start is called before the first frame update
    void Start()
    {
        child = transform.GetChild(0).transform;
        audioPlayer = GetComponent<AudioSource>();
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
        SceneManager.LoadScene("Play");
    }

    public static void ReturnToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void ChangeButtonOn()
    {
        transform.localScale = new Vector3(1.2f, 1.2f);
        child.localScale = new Vector3(1.2f, 1.2f);
        audioPlayer.Play();
    }

    public void ChangeButtonOff()
    {
        transform.localScale = Vector3.one;
        child.localScale = Vector3.one;
    }
}
