using System;
using GorillaGameModes;
using GorillaNetworking;
using Utilla.Models;
using Utilla.Patches;
using Utilla.Utils;

namespace Utilla;

public static class UtillaAPI
{
    public static Gamemode CurrentGamemode => GameModeUtils.CurrentGamemode;

    public static string CurrentGamemodeId => GameModeUtils.GetEffectiveRoomGamemode();

    public static string NetworkGamemodeId => GameModeUtils.GetNetworkRoomGamemode();

    public static string UtillaRoomGamemodeId => GameModeUtils.GetUtillaRoomGamemode();

    public static string SelectedGamemodeId => RoomPropertyUtils.SelectedGamemodeId;

    public static bool IsInUtillaRoom => GameModeUtils.CurrentRoomHasUtillaGamemode();

    public static bool TryGetGamemode(string id, out Gamemode gamemode) =>
            GameModeUtils.TryGetGamemode(id, out gamemode);

    public static bool IsRegisteredGamemode(string id) => RoomPropertyUtils.IsRegisteredUtillaGamemode(id);

    public static bool SetSelectedGamemode(string id)
    {
        if (string.IsNullOrEmpty(id) || GorillaComputer.instance == null)
            return false;

        if (!RoomPropertyUtils.IsRegisteredUtillaGamemode(id) && !Enum.IsDefined(typeof(GameModeType), id))
            return false;

        bool oldState = GorillaComputerPatches.AllowSettingMode;
        GorillaComputerPatches.AllowSettingMode = true;
        GorillaComputer.instance.SetGameModeWithoutButton(id);
        GorillaComputerPatches.AllowSettingMode = oldState;

        return true;
    }
}