using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;

namespace KillBoost;

public class KillBoost : BasePlugin, IPluginConfig<KillBoostConfig>
{
    public override string ModuleName => "Kill Boost";
    public override string ModuleAuthor => "BOINK";
    public override string ModuleVersion => "1.0.0";

    public KillBoostConfig Config { get; set; } = new();

    ConVar? TeammatesAreEnemies;
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

            return HookResult.Continue;
        });
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
