using GTA;
using GTA.Math;
using GTA.Native;
using System;

public class ArcadeCollisions : Script {
	
/*
	Arcade Collisions
		by d3ad connection
				v0 4/17/22
	SETTINGS:				 */
	bool bSeatbeltBuckled = true; // Prevent player from flying through windshield
	
	// ========================================================================================
	
	Vehicle	vehPlayer;
	
	public ArcadeCollisions() {	Tick += OnTick; }

	void OnTick(object sender, EventArgs e)	{
		bool bPlayerInVehicle	= Game.Player.Character.IsSittingInVehicle();
		bool bPlayerDriver 		= Game.Player.LastVehicle.Driver == Game.Player.Character;
		bool bPlayerDead	   	= Game.Player.Character.IsDead || ((int)Math.Round((float)(Function.Call<int>(Hash.GET_ENTITY_HEALTH, Game.Player.Character.Handle))) < 0);
		
		if (bPlayerInVehicle) {
			// Player is in vehicle
			vehPlayer = Game.Player.Character.CurrentVehicle;
			
			if (bPlayerDriver && vehPlayer.ClassType != VehicleClass.Planes && vehPlayer.ClassType != VehicleClass.Helicopters) {
				// Player is driving land vehicle, make vehicle stronger
				
				if (bSeatbeltBuckled) { Game.Player.Character.CanFlyThroughWindscreen = false; }
				vehPlayer.CanWheelsBreak = false;
				vehPlayer.IsAxlesStrong = true;
				Function.Call(Hash.SET_VEHICLE_STRONG, vehPlayer, true);
				Function.Call(Hash.SET_VEHICLE_REDUCE_GRIP, vehPlayer, false);
				Function.Call(Hash.SET_DISABLE_VEHICLE_PETROL_TANK_DAMAGE, vehPlayer, true);
				Function.Call(Hash.SET_DISABLE_VEHICLE_PETROL_TANK_FIRES, vehPlayer, true);
				Function.Call(Hash.SET_VEHICLE_EXPLODES_ON_HIGH_EXPLOSION_DAMAGE, vehPlayer, false);
				Function.Call(Hash.SET_VEHICLE_WHEELS_CAN_BREAK_OFF_WHEN_BLOW_UP, vehPlayer, true);
				Function.Call((Hash)0x4E20D2A627011E8E, vehPlayer, 0.045f);  // _SET_VEHICLE_DAMAGE_MODIFIER
				
				// Find nearby vehicles
				Vehicle[] vehNPCs = World.GetNearbyVehicles(vehPlayer.Position, 10f);
				if (vehNPCs != null) {
					for (int i = 0; i < vehNPCs.Length; i++)	{
						if (vehNPCs[i] != vehPlayer && vehNPCs[i].ClassType != VehicleClass.Cycles) { NPCVehicleCycle(vehNPCs[i]); }
					}
				}
			}
		}
		else if (Game.Player.LastVehicle != null && (bPlayerDead || !bPlayerDriver)) {
			// Player is not in vehicle, make last vehicle vulnerable
			vehPlayer = Game.Player.LastVehicle;
			vehPlayer.CanWheelsBreak = true;
			vehPlayer.IsAxlesStrong = false;
			Function.Call((Hash)0x4E20D2A627011E8E, vehPlayer, 1f); // _SET_VEHICLE_DAMAGE_MODIFIER
			Function.Call(Hash.SET_VEHICLE_STRONG, vehPlayer, false);
			Function.Call(Hash.SET_DISABLE_VEHICLE_PETROL_TANK_DAMAGE, vehPlayer, false);
			Function.Call(Hash.SET_DISABLE_VEHICLE_PETROL_TANK_FIRES, vehPlayer, false);
			Function.Call(Hash.SET_VEHICLE_EXPLODES_ON_HIGH_EXPLOSION_DAMAGE, vehPlayer, true);
			Function.Call(Hash.SET_VEHICLE_WHEELS_CAN_BREAK_OFF_WHEN_BLOW_UP, vehPlayer, true);
		}
	}

	void NPCVehicleCycle(Vehicle vehNPC) {
		bool  bPlayerShooting			= Game.Player.Character.IsShooting;
		bool  bVehPlayerOnAllWheels		= vehPlayer.IsOnAllWheels;
		bool  bVehPlayerTouchingVehNPC	= vehPlayer.IsTouching(vehNPC);
		bool  bVehNPCDamaged			= Function.Call<bool>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_ENTITY, vehNPC, vehPlayer, 1);
		bool  bVehNPCFriendly 			= vehNPC.Driver.RelationshipGroup == Game.Player.Character.RelationshipGroup;
		
		bool  bSetVehNPCStrong			= false;
		bool  bSetVehNPCAxlesStrong		= false;
		bool  bSetVehNPCReduceGrip		= false;
		float fSetVehNPCDamageModifier 	= 1;						float fSetVehNPCFrictionModifier = 1;
		
		float fVehPlayerSpeed	 		= vehPlayer.Speed;
		float fVehNPCSpeed	 			= vehNPC.Speed;
		float fVehNPCBoostBonus			= fVehNPCSpeed / 128;
		float fVehNPCFrictionBonus 	 	= fVehPlayerSpeed / 52; 	float fVehNPCDamageBonus  		 = fVehPlayerSpeed / 24;
		Vector3 v3VehPlayerVelocity 	= vehPlayer.Velocity;
		Vector3 v3VehNPCVelocity 		= vehNPC.Velocity;
		
		float fVehNPCEngineHealth 		= vehNPC.EngineHealth;		float fVehNPCEngineHealthChk	 = fVehNPCEngineHealth;
		float fVehNPCTankHealth 		= vehNPC.PetrolTankHealth;	float fVehNPCTankHealthChk 	 	 = fVehNPCTankHealth;

		if (bVehNPCFriendly) { bSetVehNPCStrong = true; bSetVehNPCAxlesStrong = true; }
		else {
			if (bVehPlayerTouchingVehNPC && bVehPlayerOnAllWheels) {
				bSetVehNPCStrong = false;
				bSetVehNPCReduceGrip = true;
				if (!bPlayerShooting) { fSetVehNPCDamageModifier = 1 + fVehNPCDamageBonus; }
				if (!bVehNPCDamaged)  {
					fSetVehNPCFrictionModifier = 0; 
					if (fVehNPCSpeed < 8) { vehNPC.Velocity = (v3VehNPCVelocity + (v3VehNPCVelocity * fVehNPCBoostBonus)) + Vector3.Divide(v3VehPlayerVelocity, 4f); }
				} else { 
					fSetVehNPCFrictionModifier = 0.32f - fVehNPCFrictionBonus;
					if (fVehNPCSpeed > 0.3 && fVehNPCSpeed < 8) { vehNPC.Velocity = (v3VehNPCVelocity + (v3VehNPCVelocity * fVehNPCBoostBonus)) + Vector3.Divide(v3VehPlayerVelocity, 64f); }
				}
			}
			else { bSetVehNPCAxlesStrong = true; }
			
			// If vehicle gets damaged enough make it unstable
			if (fVehNPCTankHealth < 100 && fVehNPCEngineHealth > 100) { fVehNPCEngineHealth = fVehNPCTankHealth; }
			if (fVehNPCTankHealth > 100 && fVehNPCEngineHealth < 100) { fVehNPCTankHealth = fVehNPCEngineHealth; }
			if (fVehNPCEngineHealth < 100 || fVehNPCTankHealth < 100) { vehNPC.CanWheelsBreak = true; } else { vehNPC.CanWheelsBreak = false; }
			
			Function.Call(Hash.SET_DISABLE_VEHICLE_PETROL_TANK_FIRES, vehNPC, false);
			Function.Call(Hash.SET_VEHICLE_WHEELS_CAN_BREAK_OFF_WHEN_BLOW_UP, vehNPC, true);
			
			// Ugly fix for fires randomly going out
			if (fVehNPCTankHealth < -1000)   { fVehNPCTankHealth = -1; }
			if (fVehNPCEngineHealth < -1999) { fVehNPCEngineHealth = -1; }
		}
	
		// Set values on vehNPC
		Function.Call(Hash.SET_VEHICLE_STRONG, vehNPC, bSetVehNPCStrong);
		vehNPC.IsAxlesStrong = bSetVehNPCAxlesStrong;
		
		if (!bVehNPCFriendly) {
			// Don't process these on friendlies
			
			if (fVehNPCEngineHealthChk != fVehNPCEngineHealth)	{ vehNPC.EngineHealth = fVehNPCEngineHealth; }
			if (fVehNPCTankHealthChk != fVehNPCTankHealth)		{ vehNPC.PetrolTankHealth = fVehNPCTankHealth; }
			// Only process health changes if values change
			
			Function.Call(Hash.SET_VEHICLE_EXPLODES_ON_HIGH_EXPLOSION_DAMAGE, vehNPC, false);
			Function.Call(Hash.SET_DISABLE_VEHICLE_PETROL_TANK_DAMAGE, vehNPC, true);
			Function.Call(Hash.SET_VEHICLE_REDUCE_GRIP, vehNPC, bSetVehNPCReduceGrip);
			Function.Call(Hash.SET_VEHICLE_FRICTION_OVERRIDE, vehNPC, fSetVehNPCFrictionModifier);
			Function.Call((Hash)0x4E20D2A627011E8E, vehNPC, fSetVehNPCDamageModifier); // _SET_VEHICLE_DAMAGE_MODIFIER
		}
	}
}