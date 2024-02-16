# Counter-Strike 2 Kill Boost Effect

Applies the blue healthshot screen effect on player kill.

## Requirements
- Metamod
- CounterStrikeSharp
  
## Config
Edit the plugin's config found in `addons/counterstrikesharp/configs/plugins/KillBoost/KillBoost.json`.\
<sub>(Note: config will be auto-generated after the plugin is loaded for the first time)</sub>

#### Defaults:
```json
{
  "EffectDuration": 0.8,
  "KillsRequired": 0,
  "KillCountFrequency": 1,
  "ApplyOnHeadshotsOnly": false,
  "IgnoreTeamKill": true
}
```
`EffectDuration`: How long the blue effect lasts (in seconds) before entirely fading out.

`KillsRequired`: Minimum amount of kills required before the effect is applied. In Competitive/Casual game modes this is based off kills per round, in Deathmatch this will be per player spawn.

`KillCountFrequency`: How often the effect is applied. Eg. `1` will apply the effect on every kill, `5` will apply the effect every 5th kill.

`ApplyOnHeadshotsOnly`: Setting this to `true` will only apply the effect if the kill was a headshot.

`IgnoreTeamKill`: Setting this to `false` will also apply the effect on team kills.\
<sub>This setting is ignored when the `mp_teammates_are_enemies` ConVar is set to true in Deathmatch game modes, applying the effect on all kills.</sub>
