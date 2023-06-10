using System.IO;
using System.Text.Json.Serialization;

namespace MLock
{
    public class Config
    {

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("enableUSBUnlocking")]
        public bool EnableUSBUnlocking { get; set; }

        [JsonPropertyName("enablePasswordUnlocking")]
        public bool EnablePasswordUnlocking { get; set; }

        [JsonPropertyName("startLocked")]
        public bool StartLocked { get; set; }

        [JsonIgnore]
        public string privateKey;

    }
}