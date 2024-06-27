using HarmonyLib;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace Zombs_R_Cute_Community_Vehicles
{
    [HarmonyPatch]
    public class CommunityVehicles : RocketPlugin<CommunityVehiclesConfiguration>
    {
        private static CommunityVehiclesConfiguration _configurationInstance;

        public override void LoadPlugin()
        {
            _configurationInstance = Configuration.Instance;
            new Harmony("Community Vehicles").PatchAll();

            //Prevent locking of community vehicles
            VehicleManager.OnToggleVehicleLockRequested += (InteractableVehicle vehicle, ref bool allow) =>
                allow = !Configuration.Instance.VehicleIds.Contains(vehicle.id);


            //Prevent Siphoning of gas
            VehicleManager.onSiphonVehicleRequested += (InteractableVehicle vehicle, Player player, ref bool allow,
                    ref ushort amount) =>
                allow = !Configuration.Instance.VehicleIds.Contains(vehicle.id);
        }

        //Prevent Battery theft
        [HarmonyPrefix]
        [HarmonyPatch(nameof(VehicleManager), nameof(VehicleManager.ReceiveStealVehicleBattery))]
        public static bool ReceiveStealVehicleBattery(in ServerInvocationContext context)
        {
            var uPlayer = UnturnedPlayer.FromPlayer(context.GetPlayer());
            return !_configurationInstance.VehicleIds.Contains(uPlayer.CurrentVehicle.id);
        }

        //Prevent placing barricades
        [HarmonyPrefix]
        [HarmonyPatch(nameof(BarricadeManager), nameof(BarricadeManager.dropPlantedBarricade))]
        public static bool dropPlantedBarricade(
            Transform parent,
            Barricade barricade,
            Vector3 point,
            Quaternion rotation,
            ulong owner,
            ulong group)
        {
            if (UnturnedPlayer.FromCSteamID((CSteamID)owner).IsAdmin) //only allow admin to build on community vehicles
                return true;
            
            if (parent.CompareTag("Vehicle"))
            {
                var vehicle = parent.GetComponent<InteractableVehicle>();
                return vehicle != null && !_configurationInstance.VehicleIds.Contains(vehicle.id);
            }

            return true;
        }
    }
}