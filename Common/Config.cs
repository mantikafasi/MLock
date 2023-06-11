﻿using System.Runtime.Serialization;

namespace Common
{
    public class Config
    {
        public static Config INSTANCE;
        [IgnoreDataMember] public string privateKey;

        [IgnoreDataMember] public string publicKey;
        [DataMember(Name = "password")] public string Password { get; set; }

        [DataMember(Name = "enableUSBUnlocking")]
        public bool EnableUSBUnlocking { get; set; }

        [DataMember(Name = "enablePasswordUnlocking")]
        public bool EnablePasswordUnlocking { get; set; }

        [DataMember(Name = "startLocked")] public bool StartLocked { get; set; }
        [DataMember(Name = "enableBlur")] public bool BlurBackground { get; set; }
    }
}