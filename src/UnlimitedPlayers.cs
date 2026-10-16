using System;
using HarmonyLib;
using MelonLoader;
using IpServerSocket = FishNet.Transporting.Tugboat.Server.ServerSocket;
using SteamServerSocket = FishySteamworks.Server.ServerSocket;

[assembly: MelonInfo(typeof(UnlimitedPlayers.UnlimitedPlayersMod), "Unlimited players", "1.0.0", "VILIK")]
[assembly: MelonGame("CuriousOwlGames", "StellarDrive")]

namespace UnlimitedPlayers
{
    public class UnlimitedPlayersMod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Multiplayer allows up to " + PlayerCap.Limit + " remote players.");
        }
    }

    static class PlayerCap
    {
        // FishySteamworks reserves connection id 32767 for the host and clamps the rest to 32766.
        public const int Limit = 32766;
    }

    [HarmonyPatch(typeof(SteamServerSocket), "SetMaximumClients")]
    static class SteamCap
    {
        static void Prefix(ref int value)
        {
            value = PlayerCap.Limit;
        }
    }

    [HarmonyPatch(typeof(FishySteamworks.FishySteamworks), "StartServer")]
    static class SteamListenCap
    {
        static void Prefix(FishySteamworks.FishySteamworks __instance)
        {
            AccessTools.Field(typeof(FishySteamworks.FishySteamworks), "_maximumClients").SetValue(__instance, (ushort)PlayerCap.Limit);
            Melon<UnlimitedPlayersMod>.Logger.Msg("Steam host allows up to " + PlayerCap.Limit + " remote players.");
        }
    }

    [HarmonyPatch(typeof(IpServerSocket), "StartConnection", new Type[] { typeof(ushort), typeof(int), typeof(string), typeof(string) })]
    static class IpListenCap
    {
        static void Prefix(ref int maximumClients)
        {
            maximumClients = PlayerCap.Limit;
        }
    }

    [HarmonyPatch(typeof(IpServerSocket), "SetMaximumClients")]
    static class IpCap
    {
        static void Prefix(ref int value)
        {
            value = PlayerCap.Limit;
        }
    }
}
