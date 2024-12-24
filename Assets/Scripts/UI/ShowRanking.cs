using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowRanking : MonoBehaviour
{
    static int SIZE = 10;
    private GameObject[] ranks;
    private GameObject highlight;

    private void Awake()
    {
        DisplayRank();
        highlight = transform.GetChild(0).gameObject;
    }

    async void DisplayRank()
    {
        List<Ranking> result = await FirebaseService.FetchTopRanking(SIZE);
        result.Sort();

        for (int i = 0; i < result.Count; i++)
        {
            var target = transform.GetChild(i + 1);
            target.GetChild(1).GetComponent<TMP_Text>().text = result[i].name;
            target.GetChild(2).GetComponent<TMP_Text>().text = ConvertMillisecondsToTimeFormat(result[i].time);
            if (result[i]._id == GameManager.GetRecordId())
            {
                var pos = highlight.transform.position;
                pos.y = target.transform.position.y;
                highlight.transform.position = pos;
                highlight.SetActive(true);
            }
        }
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }

    public static string ConvertMillisecondsToTimeFormat(int milliseconds)
    {
        // 1분 = 60000ms, 1초 = 1000ms
        int minutes = milliseconds / 60000;  // 분
        int seconds = (milliseconds % 60000) / 1000;  // 초
        int ms = milliseconds % 1000;  // 밀리초

        // "mm:ss.mmm" 형식으로 반환
        return string.Format("{0:D2}:{1:D2}.{2:D3}", minutes, seconds, ms);
    }
}
