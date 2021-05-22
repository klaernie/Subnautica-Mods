﻿namespace BuilderModule.Patches
{
    using BuilderModule.Module;
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch]
    internal class OnUpgradeModuleChange_Patch
    {
        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.OnUpgradeModuleChange))]
#if BZ
        [HarmonyPatch(typeof(SeaTruckUpgrades), nameof(SeaTruckUpgrades.OnUpgradeModuleChange))]
        [HarmonyPatch(typeof(Hoverbike), nameof(Hoverbike.OnUpgradeModuleChange))]
#endif
        [HarmonyPostfix]
        public static void Postfix(object __instance, int slotID, TechType techType, bool added)
        {
            bool validTech = Main.builderModules.Contains(techType);
            if(!validTech)
                return;

            bool noneEquipped = true;
            bool foundMono = false;
            BuilderModuleMono builderModule = null;

            switch(__instance)
            {
                case Vehicle vehicle:
                    noneEquipped = !vehicle.modules.equippedCount.TryGetValue(techType, out int count);
                    foundMono = vehicle.gameObject.TryGetComponent(out builderModule);
                    if(added && !foundMono)
                    {
                        builderModule = vehicle.gameObject.AddComponent<BuilderModuleMono>();
                        builderModule.vehicle = vehicle;
                        builderModule.energyInterface = vehicle.energyInterface;
                        builderModule.ModuleSlotID = slotID;
                        vehicle.onToggle += builderModule.OnToggle;
                    }
                    break;
#if BZ
                case SeaTruckUpgrades seaTruck:
                    noneEquipped = !seaTruck.modules.equippedCount.TryGetValue(techType, out count);
                    foundMono = seaTruck.gameObject.TryGetComponent(out builderModule);
                    if(added && !foundMono)
                    {
                        builderModule = seaTruck.gameObject.AddComponent<BuilderModuleMono>();
                        builderModule.seaTruck = seaTruck;
                        builderModule.lights = seaTruck.GetComponentInChildren<SeaTruckLights>();
                        builderModule.powerRelay = seaTruck.relay;
                        builderModule.ModuleSlotID = slotID;
                    }
                    break;
                case Hoverbike hoverbike:
                    noneEquipped = !hoverbike.modules.equippedCount.TryGetValue(techType, out count);
                    foundMono = hoverbike.gameObject.TryGetComponent(out builderModule);
                    if(added && !foundMono)
                    {
                        builderModule = hoverbike.gameObject.AddComponent<BuilderModuleMono>();
                        builderModule.hoverbike = hoverbike;
                        builderModule.energyMixin = hoverbike.energyMixin;
                        builderModule.ModuleSlotID = slotID;
                    }
                    break;
#endif
            }

            if(!added && noneEquipped && foundMono)
                GameObject.Destroy(builderModule);
        }
    }
}
