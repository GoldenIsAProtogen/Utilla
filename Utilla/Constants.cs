using System;

namespace Utilla;

public abstract class Constants
{
    internal const string Guid = "org.legoandmars.gorillatag.utilla";

    internal const string Name = "Utilla";

    internal const string Version = "2.0.0";

    public const string ModdedPrefix = "MODDED_";

    public const string RoomGamemodePropertyKey = "utilla_gamemode";

    public const string RoomBaseGamemodePropertyKey = "utilla_base_gamemode";

    public const string RoomKindPropertyKey = "utilla_room_kind";

    public const string RoomKindUtilla = "utilla";

    public const string RoomKindVanilla = "vanilla";

    [Obsolete("The legal status toggle was removed. This key is kept only so old plugins still compile.")]
    public const string LegalStatusKey = "utilla_legal_status";

    internal const string InfoRepositoryURL =
            "https://raw.githubusercontent.com/ZlothY29IQ/Utilla/refs/heads/master/Info";
}