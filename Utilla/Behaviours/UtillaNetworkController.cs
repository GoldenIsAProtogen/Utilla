using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Utilla.Models;
using Utilla.Tools;
using Utilla.Utils;

namespace Utilla.Behaviours;

internal class UtillaNetworkController : MonoBehaviourPunCallbacks
{
    private       Events.Events.RoomJoinedArgs lastRoom;
    public static UtillaNetworkController      Instance { get; private set; }

    public override void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);

            return;
        }

        Instance = this;
        base.OnEnable();

        if (NetworkSystem.Instance is not { } netSys || netSys is not NetworkSystemPUN ||
            PhotonNetwork.NetworkingClient is not { } client)
            return;

        client.UpdateCallbackTargets();
        MatchMakingCallbacksContainer callbackContainer = client.MatchMakingCallbackTargets;

        for (int i = 0; i < callbackContainer.Count; i++)
        {
            IMatchmakingCallbacks individualCallback = callbackContainer[i];

            if ((object)individualCallback is not MonoBehaviour behaviour || behaviour.gameObject != netSys.gameObject)
                continue;

            if (callbackContainer.Contains(this)) callbackContainer.Remove(this);
            callbackContainer.Insert(i, this);

            break;
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (Instance == this) Instance = null;
    }

    public override void OnJoinedRoom()
    {
        if (ApplicationQuittingState.IsQuitting) return;

        if (RoomPropertyUtils.IsUtillaSelectedGamemode && !RoomPropertyUtils.CurrentRoomMatchesSelectedUtillaGamemode())
        {
            Logging.Warning(
                    $"Joined a room without the selected Utilla gamemode property. Leaving instead of converting it into '{RoomPropertyUtils.SelectedGamemodeId}'.");

            NetworkSystem.Instance.ReturnToSinglePlayer();

            return;
        }

        RoomPropertyUtils.ApplySelectedGamemodeToCurrentRoom();
        RefreshRoomState(true);
    }

    public override void OnLeftRoom()
    {
        if (ApplicationQuittingState.IsQuitting) return;

        GameModeUtils.CurrentGamemode = null;

        if (lastRoom == null)
            return;

        Events.Events.Instance.TriggerRoomLeft(lastRoom);
        lastRoom = null;
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (ApplicationQuittingState.IsQuitting || !(NetworkSystem.Instance?.InRoom ?? false)) return;
        if (!propertiesThatChanged.ContainsKey(RoomPropertyUtils.GamemodeKey)     &&
            !propertiesThatChanged.ContainsKey(RoomPropertyUtils.BaseGamemodeKey) &&
            !propertiesThatChanged.ContainsKey(RoomPropertyUtils.RoomKindKey)) return;

        RefreshRoomState(false);
        Events.Events.Instance.TriggerRoomPropertiesUpdated(lastRoom);
    }

    private void RefreshRoomState(bool joined)
    {
        RoomGamemodeInfo info = RoomPropertyUtils.GetCurrentRoomInfo();
        GameModeUtils.CurrentGamemode = info.Gamemode;

        Events.Events.RoomJoinedArgs nextRoom = new()
        {
                IsPrivate       = NetworkSystem.Instance.SessionIsPrivate,
                Gamemode        = info.EffectiveGameMode,
                NetworkGamemode = info.NetworkGameMode,
                UtillaGamemode  = info.UtillaGameMode,
                IsUtillaRoom    = info.IsUtillaRoom,
                CurrentGamemode = info.Gamemode,
        };

        if (joined || lastRoom == null)
        {
            Events.Events.Instance.TriggerRoomJoin(nextRoom);
            lastRoom = nextRoom;

            return;
        }

        if (lastRoom.Gamemode == nextRoom.Gamemode && lastRoom.IsPrivate == nextRoom.IsPrivate) return;

        Events.Events.Instance.TriggerRoomLeft(lastRoom);
        Events.Events.Instance.TriggerRoomJoin(nextRoom);
        lastRoom = nextRoom;
    }
}