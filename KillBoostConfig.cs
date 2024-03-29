﻿using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace KillBoost;

public class KillBoostConfig : BasePluginConfig
{
    [JsonPropertyName("EffectDuration")]
    public float EffectDuration { get; set; } = 0.8f;

    [JsonPropertyName("SpeedBoost")]
    public float SpeedBoost { get; set; } = 1.2f;

    [JsonPropertyName("SpeedBoostDuration")]
    public float SpeedBoostDuration { get; set; } = 1f;

    [JsonPropertyName("KillCountRequired")]
    public int KillCountRequired { get; set; } = 0;

    [JsonPropertyName("KillCountFrequency")]
    public int KillCountFrequency { get; set; } = 1;

    [JsonPropertyName("ApplyOnHeadshotsOnly")]
    public bool ApplyOnHeadshotsOnly { get; set; } = false;

    [JsonPropertyName("IgnoreTeamKill")]
    public bool IgnoreTeamKill { get; set; } = true;
}