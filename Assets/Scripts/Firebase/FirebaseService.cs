using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using System.IO;

public class FirebaseService
{
    private static string AUTH_URL = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=";
    private static string API_KEY;
    private static string DB_URL;
    private static string userToken;

    private static void Initialize()
    {
        string filePath = Path.Combine(Application.dataPath, ".env.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Dictionary<string, string> ENVIRONMENT = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            API_KEY = ENVIRONMENT["API_KEY"];
            DB_URL = ENVIRONMENT["DB_URL"];
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }

    private static string GetURI()
    {
        return $"{DB_URL}/{GameManager.GetVersion()}/ranking.json?auth={userToken}";
    }

    public static async Task<string> InsertNewData(Ranking data)
    {
        if (userToken == null)
        {
            await AnonymousLogin();
        }
        try
        {
            UnityWebRequest request = new UnityWebRequest(GetURI(), "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(data.ToJson());
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            UnityWebRequest response = await UnityWebRequestAsync.SendWebRequestAsync(request);
            var recordId = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.downloadHandler.text)["name"];

            request.Dispose();
            return recordId;
        }
        catch (UnityWebRequestAsync.UnityWebRequestException ex)
        {
            Debug.LogError("Error: " + ex.Message);
            return null;
        }
    }

    public static async Task<List<Ranking>> FetchTopRanking(int size)
    {
        if (API_KEY == null)
        {
            Initialize();
        }

        try
        {
            UnityWebRequest request = UnityWebRequest.Get($"{GetURI()}&orderBy=\"time\"&limitToFirst={size}");
            UnityWebRequest response = await UnityWebRequestAsync.SendWebRequestAsync(request);
            var result = JsonConvert.DeserializeObject<Dictionary<string, Ranking>>(response.downloadHandler.text);
            request.Dispose();

            List<Ranking> ranking = new();
            foreach (var rank in result)
            {
                rank.Value._id = rank.Key;
                ranking.Add(rank.Value);
            }

            return ranking;
        }
        catch (UnityWebRequestAsync.UnityWebRequestException ex)
        {
            Debug.LogError("Error: " + ex.Message);
            return new List<Ranking>();
        }
    }

    public static async Task AnonymousLogin()
    {
        if (API_KEY == null)
        {
            Initialize();
        }
        try
        {
            UnityWebRequest request = new UnityWebRequest($"{AUTH_URL}{API_KEY}", "POST");
            var body = new { returnSecureToken = true };
            byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(body));
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            UnityWebRequest response = await UnityWebRequestAsync.SendWebRequestAsync(request);

            userToken = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.downloadHandler.text)["idToken"];
            request.Dispose();
        }
        catch (UnityWebRequestAsync.UnityWebRequestException ex)
        {
            Debug.LogError("Error: " + ex.Message);
        }
    }
}
