using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Photon.Realtime;
using Utilla.Utils;

namespace Utilla.Patches;

[HarmonyPatch]
internal static class MatchmakingPropertyPatches
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        MethodInfo joinRandom = AccessTools.Method(typeof(LoadBalancingClient), "OpJoinRandomRoom",
                [typeof(OpJoinRandomRoomParams),]);

        if (joinRandom != null) yield return joinRandom;

        MethodInfo joinRandomOrCreate = AccessTools.Method(typeof(LoadBalancingClient), "OpJoinRandomOrCreateRoom",
                [typeof(OpJoinRandomRoomParams), typeof(EnterRoomParams),]);

        if (joinRandomOrCreate != null) yield return joinRandomOrCreate;

        MethodInfo createRoom = AccessTools.Method(typeof(LoadBalancingClient), "OpCreateRoom",
                [typeof(EnterRoomParams),]);

        if (createRoom != null) yield return createRoom;

        MethodInfo joinOrCreate = AccessTools.Method(typeof(LoadBalancingClient), "OpJoinOrCreateRoom",
                [typeof(EnterRoomParams),]);

        if (joinOrCreate != null) yield return joinOrCreate;
    }

    private static void Prefix(object[] __args)
    {
        if (__args == null) return;

        foreach (object t in __args)
            switch (t) {
                case OpJoinRandomRoomParams joinParams:
                    RoomPropertyUtils.ApplyExpectedMatchmakingProperties(joinParams);

                    continue;

                case EnterRoomParams enterParams:
                    RoomPropertyUtils.ApplyRoomCreationProperties(enterParams);

                    break;
            }
    }
}