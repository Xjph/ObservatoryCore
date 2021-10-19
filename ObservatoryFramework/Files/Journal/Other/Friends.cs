﻿using System.Text.Json.Serialization;
using Observatory.Framework.Files.ParameterTypes;

namespace Observatory.Framework.Files.Journal
{
    public class Friends : JournalBase
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FriendStatus Status { get; init; }
        public string Name { get; init; }
    }
}
