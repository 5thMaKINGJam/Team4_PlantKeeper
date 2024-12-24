using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nickname : MonoBehaviour
{
    public InputField nicknameInput;
    public static string nickname = null;

    private void Awake()
    {
        if (nickname != null) gameObject.SetActive(false);
    }

    private void Update()
    {
        nickname = nicknameInput.text;
    }

    //���콺
    public async void InputName()
    {
        if (nickname != null && nickname.Length > 0)
        {
            gameObject.SetActive(false);
            await FirebaseService.AnonymousLogin();
        }
    }
}
