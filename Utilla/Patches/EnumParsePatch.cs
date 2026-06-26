using System;
using System.Collections.Generic;
using System.Reflection;
using GorillaGameModes;
using HarmonyLib;
using Utilla.Utils;

namespace Utilla.Patches;

[HarmonyPatch]
internal class EnumParsePatch
{
    public static MethodBase TargetMethod() =>
            typeof(Enum)
                   .GetMethod(nameof(Enum.Parse),            BindingFlags.Public | BindingFlags.Static, null,
                            [typeof(string), typeof(bool),], null)
                  ?.MakeGenericMethod(typeof(GameModeType));

    public static bool Prefix(string value, ref object __result)
    {
        if (GameModeUtils.GetGamemodeFromId(value) is { } gamemode)
        {
            __result = gamemode.BaseGamemode.GetValueOrDefault(GameModeType.Infection);

            return false;
        }

        EnumData<GameModeType> shared = EnumData<GameModeType>.Shared;
        __result = shared.NameToEnum.GetValueOrDefault(value, GameModeType.Infection);

        return false;
    }
}