using System;
using Utilla.Models;

namespace Utilla.Events;

public sealed class Events
{
    public static readonly Events Instance = new();

    /// <summary>
    ///     Called whenever the local player joins a room.
    /// </summary>
    public static event EventHandler<RoomJoinedArgs> RoomJoined;

    /// <summary>
    ///     Called whenever the local player leaves a room.
    /// </summary>
    public static event EventHandler<RoomJoinedArgs> RoomLeft;

    /// <summary>
    ///     Called whenever Utilla room properties change while staying in the same room.
    /// </summary>
    public static event EventHandler<RoomJoinedArgs> RoomPropertiesUpdated;

    /// <summary>
    ///     Called after the game has finished initializing.
    /// </summary>
    public static event EventHandler GameInitialized;

    public void TriggerRoomJoin(RoomJoinedArgs e)
    {
        RoomJoined?.SafeInvoke(this, e);
        GorillaLibraryCompat.InvokeRoomJoined(e.Gamemode ?? string.Empty);
    }

    public void TriggerRoomLeft(RoomJoinedArgs e)
    {
        RoomLeft?.SafeInvoke(this, e);
        GorillaLibraryCompat.InvokeRoomLeft(e.Gamemode ?? string.Empty);
    }

    public void TriggerRoomPropertiesUpdated(RoomJoinedArgs e) => RoomPropertiesUpdated?.SafeInvoke(this, e);

    public void TriggerGameInitialized()
    {
        GameInitialized?.SafeInvoke(this, EventArgs.Empty);
        GorillaLibraryCompat.InvokeGameInitialized();
    }

    public class RoomJoinedArgs : EventArgs
    {
        /// <summary>
        ///     Whether the current room is private.
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        ///     Legacy effective gamemode value. For Utilla rooms this is the custom property value.
        /// </summary>
        public string Gamemode { get; set; }

        /// <summary>
        ///     The vanilla network gamemode string used to create or join the room.
        /// </summary>
        public string NetworkGamemode { get; set; }

        /// <summary>
        ///     The Utilla custom room property gamemode value, or empty when the room is vanilla.
        /// </summary>
        public string UtillaGamemode { get; set; }

        public bool IsUtillaRoom { get; set; }

        public Gamemode CurrentGamemode { get; set; }
    }
}