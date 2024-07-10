using Newtonsoft.Json;

namespace MoneyGoblinUploader.Util
{
    public class UploaderVersionPacket
    {
        [JsonProperty]
        public string Version;

        public UploaderVersionPacket()
        {
            Version = "0.3.4-alpha";
        }

        public string getJSONString()
        {
            return $"\"Version\":{Version}";
        }

    }
}
