using System;
using System.Globalization;
using System.Linq;
using GorillaGameModes;
using Utilla.Behaviours;
using Utilla.Models;

namespace Utilla.Utils;

public static class GameModeUtils
{
    public static Gamemode CurrentGamemode { get; internal set; }

    public static Gamemode FindGamemodeInString(string gmString)
    {
        if (string.IsNullOrEmpty(gmString)) 
            return null;

        if (!gmString.Contains('|'))
            return GetGamemode(gamemode => gmString == gamemode.ID || gmString.EndsWith(gamemode.ID));

        {
            string[] split        = gmString.Split('|');
            bool     useSeparator = split.Length > 2;

            return useSeparator ? GetGamemode(gamemode => split[^1] == gamemode.ID) : null;
        }
    }

    public static Gamemode GetGamemodeFromId(string id) => GetGamemode(gamemode => gamemode.ID == id);

    public static Gamemode GetGamemode(Func<Gamemode, bool> predicate)
    {
        if (predicate == null) 
            return null;
        
        if (GamemodeManager.HasInstance &&
            GamemodeManager.Instance.Gamemodes.LastOrDefault(predicate) is { } gameMode)
            return gameMode;

        return null;
    }

    public static bool TryGetGamemode(string id, out Gamemode gamemode)
    {
        gamemode = GetGamemodeFromId(id);

        return gamemode != null;
    }

    public static string GetEffectiveRoomGamemode() => RoomPropertyUtils.GetCurrentRoomInfo().EffectiveGameMode;

    public static string GetNetworkRoomGamemode() => RoomPropertyUtils.GetCurrentRoomInfo().NetworkGameMode;

    public static string GetUtillaRoomGamemode() => RoomPropertyUtils.GetCurrentRoomInfo().UtillaGameMode;

    public static bool CurrentRoomHasUtillaGamemode() => RoomPropertyUtils.GetCurrentRoomInfo().IsUtillaRoom;

    public static string GetGameModeName(GameModeType gameModeType)
    {
        string modeName = GetGameModeInstance(gameModeType) is { } gameManager
                                  ? gameManager.GameModeName()
                                  : GameMode.GameModeZoneMapping.GetModeName(gameModeType);

        return string.Equals(modeName, gameModeType.GetName(), StringComparison.CurrentCultureIgnoreCase)
                       ? gameModeType.GetName()
                       : CultureInfo.InvariantCulture.TextInfo.ToTitleCase(modeName.ToLower());
    }

    public static GorillaGameManager GetGameModeInstance(GameModeType gameModeType)
    {
        if (GameMode.GetGameModeInstance(gameModeType) is { } gameManager && gameManager)
            return gameManager;

        return null;
    }

    public static bool IsSuperGameMode(this GameModeType gameMode) =>
            gameMode is GameModeType.SuperInfect or GameModeType.SuperCasual;
}