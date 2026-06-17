namespace Utilla.Models
{
    public readonly struct RoomGamemodeInfo(
            string   networkGameMode,
            string   utillaGameMode,
            Gamemode gamemode,
            bool     isUtillaRoom)
    {
        public readonly string   NetworkGameMode = networkGameMode ?? string.Empty;
        public readonly string   UtillaGameMode  = utillaGameMode  ?? string.Empty;
        public readonly Gamemode Gamemode        = gamemode;
        public readonly bool     IsUtillaRoom    = isUtillaRoom;

        public string EffectiveGameMode => !string.IsNullOrEmpty(UtillaGameMode) ? UtillaGameMode : NetworkGameMode;
    }
}
