using System.Net.Http;
using System.Text;
using System.Threading;

namespace MoneyGoblinUploader.Utils
{
    public static class Upload
    {
        public static void PostJson(string json, string target, string endpoint = "")
        {
            var client = new HttpClient();

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var cts = new CancellationTokenSource();
            cts.CancelAfter(60000); //60s
            var target_url = target + endpoint;
            client.PostAsync(target_url, content, cts.Token);
        }
    }
}
