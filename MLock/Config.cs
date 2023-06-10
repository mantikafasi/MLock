using System.IO;
using System.Runtime.Serialization;

namespace MLock
{
    public class Config
    {
        public static Config INSTANCE;

        [IgnoreDataMember] public string publicKey;
        [DataMember(Name = "password")] public string Password { get; set; }
        [DataMember(Name = "enableUSBUnlocking")] public bool EnableUSBUnlocking { get; set; }

        [DataMember( Name= "enablePasswordUnlocking")]
        public bool EnablePasswordUnlocking { get; set; }

        [DataMember(Name= "startLocked")] public bool StartLocked { get; set; }
    }
}