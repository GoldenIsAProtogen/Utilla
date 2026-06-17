using System;
using System.Linq;
using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using Utilla.Models;

namespace Utilla.Utils;

public static class RoomPropertyUtils
{
    public const string GamemodeKey     = Constants.RoomGamemodePropertyKey;
    public const string BaseGamemodeKey = Constants.RoomBaseGamemodePropertyKey;
    public const string RoomKindKey     = Constants.RoomKindPropertyKey;

    private static readonly string[] LobbyPropertyKeys = [GamemodeKey, BaseGamemodeKey, RoomKindKey,];

    public static string SelectedGamemodeId =>
            GorillaComputer.instance?.currentGameMode?.Value ?? nameof(GameModeType.Infection);

    public static Gamemode SelectedGamemode => GameModeUtils.GetGamemodeFromId(SelectedGamemodeId);

    public static bool IsUtillaSelectedGamemode => IsRegisteredUtillaGamemode(SelectedGamemodeId);

    public static bool IsRegisteredUtillaGamemode(string id)
    {
        if (string.IsNullOrEmpty(id)) 
            return false;
        
        if (Enum.IsDefined(typeof(GameModeType), id)) 
            return false;

        return GameModeUtils.GetGamemodeFromId(id) != null;
    }

    public static string GetNetworkGameModeId(string id)
    {
        if (string.IsNullOrEmpty(id)) 
            return nameof(GameModeType.Infection);
        
        if (Enum.IsDefined(typeof(GameModeType), id)) 
            return id;

        Gamemode gamemode = GameModeUtils.GetGamemodeFromId(id);

        if (gamemode?.BaseGamemode is { } baseMode) 
            return baseMode.ToString();

        return nameof(GameModeType.Infection);
    }

    public static string GetSelectedNetworkGameModeId() => GetNetworkGameModeId(SelectedGamemodeId);

    public static RoomGamemodeInfo GetCurrentRoomInfo()
    {
        string    networkMode = NetworkSystem.Instance?.GameModeString ?? string.Empty;
        Hashtable properties  = PhotonNetwork.CurrentRoom?.CustomProperties;

        return GetRoomInfo(networkMode, properties);
    }

    public static RoomGamemodeInfo GetRoomInfo(string networkMode, Hashtable properties)
    {
        string utillaMode    = ReadString(properties, GamemodeKey);
        string effectiveMode = !string.IsNullOrEmpty(utillaMode) ? utillaMode : networkMode;
        Gamemode gamemode = GameModeUtils.GetGamemodeFromId(effectiveMode) ??
                            GameModeUtils.FindGamemodeInString(effectiveMode);

        return new RoomGamemodeInfo(networkMode, utillaMode, gamemode, !string.IsNullOrEmpty(utillaMode));
    }

    public static string ReadString(Hashtable properties, string key)
    {
        if (properties == null || string.IsNullOrEmpty(key)) 
            return string.Empty;

        return properties.TryGetValue(key, out object value) ? value as string ?? string.Empty : string.Empty;
    }

    public static bool HasUtillaGamemodeProperty(Hashtable properties) =>
            !string.IsNullOrEmpty(ReadString(properties, GamemodeKey));

    public static bool CurrentRoomMatchesSelectedUtillaGamemode()
    {
        if (!IsUtillaSelectedGamemode) 
            return true;

        return ReadString(PhotonNetwork.CurrentRoom?.CustomProperties, GamemodeKey) == SelectedGamemodeId;
    }

    public static void ApplyExpectedMatchmakingProperties(OpJoinRandomRoomParams joinParams)
    {
        if (joinParams == null || !IsUtillaSelectedGamemode) 
            return;

        joinParams.ExpectedCustomRoomProperties              ??= new Hashtable();
        joinParams.ExpectedCustomRoomProperties[GamemodeKey] =   SelectedGamemodeId;
    }

    public static void ApplyRoomCreationProperties(EnterRoomParams enterParams)
    {
        if (enterParams == null) 
            return;

        enterParams.RoomOptions ??= new RoomOptions();
        RoomOptions roomOptions = enterParams.RoomOptions;
        roomOptions.CustomRoomProperties ??= new Hashtable();

        if (IsUtillaSelectedGamemode)
        {
            roomOptions.CustomRoomProperties[GamemodeKey]     = SelectedGamemodeId;
            roomOptions.CustomRoomProperties[BaseGamemodeKey] = GetSelectedNetworkGameModeId();
            roomOptions.CustomRoomProperties[RoomKindKey]     = Constants.RoomKindUtilla;
        }
        else if (!roomOptions.CustomRoomProperties.ContainsKey(RoomKindKey))
        {
            roomOptions.CustomRoomProperties[RoomKindKey] = Constants.RoomKindVanilla;
        }

        roomOptions.CustomRoomPropertiesForLobby = MergeLobbyKeys(roomOptions.CustomRoomPropertiesForLobby);
    }

    public static bool ApplySelectedGamemodeToCurrentRoom()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null) 
            return false;

        if (HasUtillaGamemodeProperty(PhotonNetwork.CurrentRoom.CustomProperties)) 
            return false;
        
        if (!PhotonNetwork.IsMasterClient) 
            return false;

        string selected           = SelectedGamemodeId;
        bool   selectedUtillaMode = IsRegisteredUtillaGamemode(selected);

        Hashtable properties = new()
        {
                [BaseGamemodeKey] = GetNetworkGameModeId(selected),
                [RoomKindKey]     = selectedUtillaMode ? Constants.RoomKindUtilla : Constants.RoomKindVanilla,
        };

        if (selectedUtillaMode) properties[GamemodeKey] = selected;

        return PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    private static string[] MergeLobbyKeys(string[] existing)
    {
        if (existing == null || existing.Length == 0) 
            return LobbyPropertyKeys.ToArray();

        return existing.Concat(LobbyPropertyKeys).Where(key => !string.IsNullOrEmpty(key)).Distinct().ToArray();
    }
}