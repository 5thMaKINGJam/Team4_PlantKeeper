using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowRanking : MonoBehaviour
{
    string[] ranks;
    public Text[] rankText;

    private void Start()
    {
        ranks= new string[10];
        //��ũ �޾ƿ���
        for(int i = 0; i < 10; i++)
        {
            ranks[i] = "X�� XXX 00:00:00";
        }

        DisplayRank();
    }

    void DisplayRank()
    {
        for(int i = 0; i < 10; i++)
        {
            rankText[i].text = ranks[i];
        }
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }
}
