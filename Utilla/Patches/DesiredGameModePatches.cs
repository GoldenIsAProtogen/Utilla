using System;
using GorillaGameModes;
using GorillaNetworking;
using HarmonyLib;
using Utilla.Tools;
using Utilla.Utils;

namespace Utilla.Patches;

[HarmonyPatch(typeof(GorillaNetworkJoinTrigger))]
internal class DesiredGameModePatches
{
    [HarmonyPatch(nameof(GorillaNetworkJoinTrigger.GetDesiredGameType))]
    [HarmonyPrefix]
    public static bool DesiredGameTypePatch(GorillaNetworkJoinTrigger __instance, ref string __result,
                                            ref GTZone                ___zone)
    {
        Type joinTriggerType = __instance.GetType();

        if (joinTriggerType == typeof(GorillaNetworkRankedJoinTrigger) || ___zone == GTZone.ranked)
        {
            __result = nameof(GameModeType.InfectionCompetitive);

            return false;
        }

        string selectedMode = GorillaComputer.instance?.currentGameMode?.Value ?? nameof(GameModeType.Infection);
        string networkMode  = RoomPropertyUtils.GetNetworkGameModeId(selectedMode);

        if (GameModeUtils.GetGamemodeFromId(selectedMode) is { BaseGamemode: not null, } gamemode)
        {
            GameModeType verifiedMode = GameMode.GameModeZoneMapping.VerifyModeForZone(__instance.zone,
                    gamemode.BaseGamemode.Value, NetworkSystem.Instance.SessionIsPrivate);

            __result = verifiedMode == gamemode.BaseGamemode.Value ? networkMode : verifiedMode.ToString();

            return false;
        }

        if (Enum.IsDefined(typeof(GameModeType), selectedMode))
            return true;

        Logging.Message($"Utilla room property mode '{selectedMode}' will use network game mode '{networkMode}'.");
        __result = networkMode;

        return false;
    }

    [HarmonyPatch(nameof(GorillaNetworkJoinTrigger.GetDesiredGameTypeLocalized))]
    [HarmonyPrefix]
    public static bool DesiredLocalizedGameTypePatch(GorillaNetworkJoinTrigger __instance, ref string __result,
                                                     ref GTZone                ___zone) => DesiredGameTypePatch(__instance,
            ref __result, ref ___zone);
}