using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Il2Cpp;

namespace PrimitierNOVRDBG.Patches
{
    [HarmonyPatch(typeof(PlayerLife), nameof(PlayerLife.ReceiveDamage))]
    internal static class PlayerLifeReceiveDamagePatch
    {
        private static bool Prefix()
        {
            return !Mod.god;
        }
    }
}
