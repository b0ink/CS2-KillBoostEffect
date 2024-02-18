using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;

namespace KillBoost;

public class KillBoost : BasePlugin, IPluginConfig<KillBoostConfig>
{
    public override string ModuleName => "Kill Boost";
    public override string ModuleAuthor => "BOINK";
    public override string ModuleVersion => "1.0.1";

    public KillBoostConfig Config { get; set; } = new();

    ConVar? TeammatesAreEnemies;

    public Dictionary<int, float> LastSpeedBoost = new();
    public override void Load(bool hotReload)
    {
        FindConvars();

        RegisterEventHandler<EventPlayerDeath>((@event, info) =>
        {

            var attacker = @event.Attacker;
            var victim = @event.Userid;

            if (attacker == null || !attacker.IsValid)
                return HookResult.Continue;

            if (victim == null || !victim.IsValid)
                return HookResult.Continue;

            var pawn = attacker.PlayerPawn.Value;
            if (pawn == null || !pawn.IsValid)
                return HookResult.Continue;

            if (pawn.LifeState != (byte)LifeState_t.LIFE_ALIVE)
                return HookResult.Continue;

            var ActionTrackingServices = attacker.ActionTrackingServices;
            if (ActionTrackingServices == null)
                return HookResult.Continue;

            var numKills = ActionTrackingServices.NumRoundKills;

            if (numKills < Config.KillCountRequired)
                return HookResult.Continue;

            if (Config.KillCountFrequency > 1 && numKills % Config.KillCountFrequency != 0)
                return HookResult.Continue;

            if (Config.ApplyOnHeadshotsOnly && !@event.Headshot)
                return HookResult.Continue;

            if (TeammatesAreEnemies != null || FindConvars())
            {
                if (Config.IgnoreTeamKill && !TeammatesAreEnemies!.GetPrimitiveValue<bool>() && attacker.Team == victim.Team)
                {
                    return HookResult.Continue;
                }
            }

            pawn.HealthShotBoostExpirationTime = Server.CurrentTime + Config.EffectDuration;
            Utilities.SetStateChanged(pawn, "CCSPlayerPawn", "m_flHealthShotBoostExpirationTime");


            if(Config.SpeedBoost != 1.0f)
            {
                SetPlayerSpeed(pawn, Config.SpeedBoost);
                int userId = -1;
                if (attacker.UserId != null)
                {
                    userId = (int)attacker.UserId;
                    LastSpeedBoost[userId] = Server.CurrentTime;
                }

                AddTimer(Config.SpeedBoostDuration, () =>
                {
                    if(userId != -1)
                    {
                        // Cancel previous timer on consecutive kills
                        float timeSinceSpeedBoost = (Server.CurrentTime - LastSpeedBoost[userId]) + 0.1f;

                        if (timeSinceSpeedBoost < Config.SpeedBoostDuration)
                            return;
                        else
                            LastSpeedBoost.Remove(userId);
                    }

                    SetPlayerSpeed(pawn, 1f);
                });
            }


            return HookResult.Continue;
        });
    }

    public void SetPlayerSpeed(CCSPlayerPawn? pawn, float speed)
    {
        if (pawn == null || !pawn.IsValid) return;
        pawn.VelocityModifier = speed;
        Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_flVelocityModifier");
    }

    public bool FindConvars()
    {
        TeammatesAreEnemies = ConVar.Find("mp_teammates_are_enemies");
        return TeammatesAreEnemies != null;
    }

    public override void Unload(bool hotReload)
    {

    }

    public void OnConfigParsed(KillBoostConfig config)
    {
        this.Config = config;
        if (config.KillCountFrequency <= 0)
        {
            config.KillCountFrequency = 1;
        }
    }
}
