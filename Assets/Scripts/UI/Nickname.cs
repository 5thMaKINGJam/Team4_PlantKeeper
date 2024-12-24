using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Nickname : MonoBehaviour
{
    public TMP_InputField nicknameInput;
    public static string nickname = null;
    [SerializeField]
    private TMP_Text warningText;

    private void Awake()
    {
        if (nickname != null) gameObject.SetActive(false);
        else
        {
            var randomNumber = $"{Random.Range(1, 1000)}".PadLeft(3, '0');
            nicknameInput.text = $"익명{randomNumber}";
        }
    }

    private void Update()
    {
        if (nicknameInput.text.Length > 5)
        {
            nicknameInput.text = nicknameInput.text.Substring(0, 5);
            warningText.color = Color.red;
        }
        nickname = nicknameInput.text;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ConfirmNickName();
        }
    }

    //���콺
    public async void ConfirmNickName()
    {
        if (nickname != null && nickname.Length > 0)
        {
            gameObject.SetActive(false);
            await FirebaseService.AnonymousLogin();
        }
    }
}
