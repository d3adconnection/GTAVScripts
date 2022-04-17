using GTA;
using GTA.Math;
using GTA.Native;
using System;

public class RoadkillRemake : Script {

/*
	Roadkill Remake
    Originally by ZiPPO RAID
		  by d3ad connection
			      v0 4/17/22
	SETTINGS:				 */
	bool bDamagePedOnFoot = true; // Activate for peds on foot
	bool bDamagePedInVehi = true; // Activate for peds in vehicles
	
	// ========================================================================================
	
	public RoadkillRemake()	{
		Interval = 30;
		Tick += this.OnTick;
	}

	// Coin flip for every event to provide more randomness
	public Random randomGenerator = new Random();
	
	public void OnTick(object sender, EventArgs e)
	{
		if (Game.Player.Character.IsSittingInVehicle() && randomGenerator.NextDouble() >= 0.5))
		{
			Ped[] pedsNearby = World.GetNearbyPeds(Game.Player.Character.Position, 5f);
			for (int i = 0; i < pedsNearby.Length; i++)
			{
				// First make sure the ped is not friendly
				if (pedsNearby != null && pedsNearby[i].RelationshipGroup != Game.Player.Character.RelationshipGroup)
				{
					// If ped is in a vehicle and no mission is active
					if (bDamagePedInVehi && pedsNearby[i].IsSittingInVehicle() && Function.Call<bool>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_ENTITY, pedsNearby[i].CurrentVehicle, Game.Player.Character.CurrentVehicle, 1) && pedsNearby[i].CurrentVehicle.ClassType != VehicleClass.Motorcycles && pedsNearby[i].CurrentVehicle.ClassType != VehicleClass.Cycles)
					{				
						if (Game.Player.Character.CurrentVehicle.Speed > 7.0) {
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "Car_Crash_Heavy", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "RunOverByVehicle", 0, 1); }
						}
						if (Game.Player.Character.CurrentVehicle.Speed > 12.5) {
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "HOSPITAL_0", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "HOSPITAL_1", 0, 1); }
						}
						if (Game.Player.Character.CurrentVehicle.Speed > 19.0) {
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "SCR_Michael_Finale", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "BigRunOverByVehicle", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "SCR_Shark", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "HOSPITAL_2", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "HOSPITAL_3", 0, 1); }
							pedsNearby[i].ApplyDamage(1);
						}
						if (Game.Player.Character.CurrentVehicle.Speed > 25.0) {
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "HOSPITAL_6", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "HOSPITAL_7", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "HOSPITAL_8", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "HOSPITAL_9", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "Explosion_Med", 0, 1); }
							pedsNearby[i].ApplyDamage(5);
						}
					}
					
					// If ped is ragdoll
					if (bDamagePedOnFoot && (Game.Player.Character.CurrentVehicle.IsTouching(pedsNearby[i])) && pedsNearby[i].IsRagdoll && Game.Player.Character.CurrentVehicle.Speed > 0.4)
					{	
						if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "Fall", 0, 1); }
						if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "Fall_Low", 0, 1); }
						if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "Dirt_Dry", 0, 1); }
						if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "Car_Crash_Light", 0, 1); }

						if ((Game.Player.Character.CurrentVehicle.Speed > 7.0) || (pedsNearby[i].IsDead && randomGenerator.NextDouble() >= 0.5)) {
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "Car_Crash_Heavy", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "RunOverByVehicle", 0, 1); }
							pedsNearby[i].ApplyDamage(1);
						}
						if ((Game.Player.Character.CurrentVehicle.Speed > 12.5) || (pedsNearby[i].IsDead && randomGenerator.NextDouble() >= 0.5)) {
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "HOSPITAL_0", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "HOSPITAL_1", 0, 1); }
							pedsNearby[i].ApplyDamage(1);
						}
						if ((Game.Player.Character.CurrentVehicle.Speed > 19.0) || (pedsNearby[i].IsDead && randomGenerator.NextDouble() >= 0.5)) {
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "SCR_Michael_Finale", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "BigRunOverByVehicle", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "SCR_Shark", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "HOSPITAL_2", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "HOSPITAL_3", 0, 1); }
							pedsNearby[i].ApplyDamage(15);
						}
						if ((Game.Player.Character.CurrentVehicle.Speed > 25.0) || (pedsNearby[i].IsDead && randomGenerator.NextDouble() >= 0.5)) {
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "HOSPITAL_6", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "HOSPITAL_7", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "HOSPITAL_8", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "HOSPITAL_9", 0, 1); }
							if (randomGenerator.NextDouble() >= 0.5) { Function.Call(Hash.APPLY_PED_DAMAGE_PACK, pedsNearby[i].Handle, "Explosion_Med", 0, 1); }
							pedsNearby[i].ApplyDamage(35);
						}
					}
				}
			}
		}
	}
}
