using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowRanking : MonoBehaviour
{
    static int SIZE = 10;
    public Text[] rankText;

    private void Awake()
    {
        for (int i = 0; i < SIZE; i++)
        {
            rankText[i].text = CenterAlign("-", 19);
        }

        DisplayRank();
    }

    async void DisplayRank()
    {
        List<Ranking> result = await FirebaseService.FetchTopRanking(SIZE);
        result.Sort();

        for (int i = 0; i < result.Count; i++)
        {
            rankText[i].text = $"{i + 1:D2}위 {CenterAlign(result[i].name, 5)} {ConvertMillisecondsToTimeFormat(result[i].time)}";
        }

        for (int i = result.Count; i < SIZE; i++)
        {
            rankText[i].text = CenterAlign("-", 19);
        }
    }

    public static string CenterAlign(string input, int length)
    {
        // 입력 문자열이 5자 이상이면 그대로 반환
        if (input.Length >= length)
        {
            return input;
        }

        // 남는 공간을 양쪽에 균등하게 나누어 가운데 정렬
        int totalPadding = length - input.Length;
        int leftPadding = totalPadding / 2;
        int rightPadding = totalPadding - leftPadding;

        // PadLeft와 PadRight로 가운데 정렬
        return input.PadLeft(input.Length + leftPadding).PadRight(length);
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
