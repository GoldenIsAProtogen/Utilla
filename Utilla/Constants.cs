using System;

namespace Utilla;

public abstract class Constants
{
    internal const string Guid = "org.legoandmars.gorillatag.utilla",
                          Name = "Utilla",
                          Version = "2.0.1";

    public const string ModdedPrefix                = "MODDED_",
                        RoomGamemodePropertyKey     = "utilla_gamemode",
                        RoomBaseGamemodePropertyKey = "utilla_base_gamemode",
                        RoomKindPropertyKey         = "utilla_room_kind",
                        RoomKindUtilla              = "utilla",
                        RoomKindVanilla             = "vanilla";

    [Obsolete("The legal status toggle was removed. Leaving this here in-case some other mods use it?")]
    public const string LegalStatusKey = "utilla_legal_status";

    internal const string InfoRepositoryURL = "https://raw.githubusercontent.com/GoldenIsAProtogen/Utilla/refs/heads/master/Info";
}