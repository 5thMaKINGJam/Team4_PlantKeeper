using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;

public class RealtimeDB
{
    private static string url = $"https://plantkeeper-202411-default-rtdb.asia-southeast1.firebasedatabase.app/PlantKeeper";

    private static string GetURI()
    {
        return $"{url}/{GameManager.GetVersion()}/ranking.json";
    }

    public static async Task InsertNewData(Ranking data)
    {
        try
        {
            UnityWebRequest request = new UnityWebRequest(GetURI(), "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(data.ToJson());
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            UnityWebRequest response = await UnityWebRequestAsync.SendWebRequestAsync(request);
        }
        catch (UnityWebRequestAsync.UnityWebRequestException ex)
        {
            Debug.LogError("Error: " + ex.Message);
        }
    }

    public static async Task<List<Ranking>> FetchTopRanking(int size)
    {
        try
        {
            UnityWebRequest request = UnityWebRequest.Get($"{GetURI()}?orderBy=\"time\"&limitToFirst={size}");
            UnityWebRequest response = await UnityWebRequestAsync.SendWebRequestAsync(request);
            var result = JsonConvert.DeserializeObject<Dictionary<string, Ranking>>(response.downloadHandler.text);
            return new List<Ranking>(result.Values);
        }
        catch (UnityWebRequestAsync.UnityWebRequestException ex)
        {
            Debug.LogError("Error: " + ex.Message);
            return new List<Ranking>();
        }
    }
}
