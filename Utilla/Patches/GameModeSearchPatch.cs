using GorillaGameModes;
using HarmonyLib;
using Photon.Pun;
using Utilla.Tools;
using Utilla.Utils;

namespace Utilla.Patches;

[HarmonyPatch(typeof(GameMode), nameof(GameMode.FindGameModeInPropertyString))]
internal class GameModeSearchPatch
{
    public static bool Prefix(string gmString, ref string __result)
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null)
        {
            string propertyMode = RoomPropertyUtils.ReadString(PhotonNetwork.CurrentRoom.CustomProperties,
                    RoomPropertyUtils.GamemodeKey);

            if (GameModeUtils.GetGamemodeFromId(propertyMode) is { } propertyGamemode)
            {
                __result = propertyGamemode.ID;

                return false;
            }
        }

        if (GameModeUtils.FindGamemodeInString(gmString) is { } gamemode)
        {
            __result = gamemode.ID;

            return false;
        }

        Logging.Warning($"No Utilla gamemode was found in '{gmString}'. Falling back to vanilla lookup.");

        return true;
    }
}