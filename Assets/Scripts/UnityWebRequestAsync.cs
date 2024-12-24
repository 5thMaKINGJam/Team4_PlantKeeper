using UnityEngine.Networking;
using System.Threading.Tasks;

public static class UnityWebRequestAsync
{
    public static Task<UnityWebRequest> SendWebRequestAsync(UnityWebRequest request)
    {
        var tcs = new TaskCompletionSource<UnityWebRequest>();

        request.SendWebRequest().completed += operation =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                tcs.SetResult(request);
            }
            else
            {
                tcs.SetException(new UnityWebRequestException(request));
            }
        };

        return tcs.Task;
    }

    public class UnityWebRequestException : System.Exception
    {
        public UnityWebRequest Request { get; }

        public UnityWebRequestException(UnityWebRequest request)
            : base($"UnityWebRequest Error: {request.error}")
        {
            Request = request;
        }
    }
}
