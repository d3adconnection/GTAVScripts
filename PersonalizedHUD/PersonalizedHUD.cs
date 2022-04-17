using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.IO;

public class PersonalizedHUD : Script
{
	/*
		Personalized HUD
		by d3ad connection
		v0 4/17/22
							*/

	// Initialize variables
	int  iPreset;
	
	bool bStatusArmed;            // Holding a weapon
	bool bStatusBusted;           // Is being arrested
	bool bStatusButton;           // Character selection (d-pad down) button pressed
	bool bStatusCutscene;         // Cutscene is playing
	bool bStatusMission;          // Unable to save (considered in mission)
	bool bStatusWaypoint;         // Custom waypoint active
	bool bStatusDamage;           // Has lower health than previous tick
	bool bStatusHealing;	      // Has more health than previous tick
	bool bStatusDead;		      // Is dead
	bool bStatusHasControl;       // Not in ragdoll state
	bool bStatusSneaking;         // Sneaking/crouching
	bool bStatusSprinting;        // Sprinting
	bool bStatusCombat;           // Aiming or shooting
	bool bStatusCover;            // In cover
	bool bStatusReloading;        // Reloading
	bool bStatusPhone;            // Phone is out
	bool bStatusVehicle;          // In vehicle
	bool bStatusDriving;          // Accelerating or reversing
	bool bStatusPassenger;          // Accelerating or reversing
	
	bool bHUDEnabled;
	int  iHUDCooldownTimer;
	int  iHUDShowAmmoTimer;
	int  iHUDSkipOneTick;
	
	bool bRadarEnabled;
	int  iRadarCooldownTimer;
	bool bBigRadarButtonOn;
	
	bool bPoliceBlipEnabled;
	int  iPoliceBlipCooldownTimer;
	
	int  iButtonCooldownMin = 200; // Minimum cooldown timer for triggers that look weird with low cooldown
	
	int  iRadarZoomType;
	
	int  iHealthPrevValue;
	int  iHealthCurrValue;
	int  iHealthNewValue;
	int  iHealthDamageTimer;
	
	int  iHealthRegenDelay;
	bool bHealthRegenEffect;
	bool bHealthRegenEngaged;
	bool bHealthLowThreshold;
	
	double dGameTimescale;        // so we only call the time scale function once every tick
	
	// .ini settings
	
	bool bAudioPoliceScanner; 
	bool bAudioWantedMusic;   
	bool bAudioFlyingMusic;   
	bool bAudioPauseMusic;    
		
	int  iHUDAutohideCooldown;
		
	int  iHUDHideWantedLevel; 
	int  iHUDHideAmmo;
	
	int  iHUDHideVehicleName; 
	int  iHUDHideVehicleClass;
	int  iHUDHideAreaName;    
	int  iHUDHideStreetName;  
		
	int  iHUDShowAmmoCooldown;
	bool bHUDShowAmmoDuringReload;
	bool bHUDShowAmmoInCombat;
	bool bHUDShowCashChanges;
	bool bHUDHideCash;
	bool bHUDHideReticle;     
	bool bHUDHideHelpText;    
	bool bHUDHideSubtitleText;
		
	int  iHUDShowWithWantedLevel;
	bool bHUDShowWithPhone;
	bool bHUDShowWithButton;
	bool bHUDShowInVehicle;   
	bool bHUDShowWhileSprinting;
	bool bHUDShowWhileReloading;
	bool bHUDShowWhileArmed;
	bool bHUDShowInCombat;
	bool bHUDShowInCover;
	bool bHUDShowWhileSneaking;
	bool bHUDShowWhenHit;
	bool bHUDShowDuringRegen; 
	bool bHUDShowInMission; 
	bool bHUDShowWithWaypoint; 
		
	bool bRadarBigMapWithPhone;
	bool bRadarBigMapWithButton;
		
	int  iRadarAutohideCooldown; 
		
	int  iRadarShowWithWantedLevel;
	bool bRadarShowWithPhone; 
	bool bRadarShowWithButton;
	bool bRadarShowInVehicle; 
	bool bRadarShowWhilePassenger; 
	bool bRadarShowWhileSprinting;  
	bool bRadarShowWhileReloading;  
	bool bRadarShowWhileArmed;   
	bool bRadarShowInCombat;   
	bool bRadarShowInCover;   
	bool bRadarShowWhileSneaking;  
	bool bRadarShowWhenHit;   
	bool bRadarShowDuringRegen;  
	bool bRadarShowInMission;  
	bool bRadarShowWithWaypoint;  

	bool bRadarZoomCustomize;
	
	int  iRadarZoomNormal;    
	int  iRadarZoomSprinting; 
	int  iRadarZoomAiming;    
	int  iRadarZoomSneaking;  
	int  iRadarZoomVehicle;   
	int  iRadarZoomAccelerating;
	int  iRadarZoomPhone;
	int  iRadarZoomPhoneVeh;

	int  iPoliceBlipAutohideCooldown;

	int  iPoliceBlipWithWantedLevel;
	bool bPoliceBlipWithPhone;
	bool bPoliceBlipWithButton;     
	bool bPoliceBlipInVehicle;
	bool bPoliceBlipWhileReloading;
	bool bPoliceBlipWhileSprinting;
	bool bPoliceBlipWhileArmed;
	bool bPoliceBlipInCombat;
	bool bPoliceBlipInCover;
	bool bPoliceBlipWhileSneaking;
	bool bPoliceBlipWhenHit;
	bool bPoliceBlipDuringRegen;
	
	bool bHealthUseDamageEffect;
	
	bool   bHealthWastedScreenEffect;
	double dHealthWastedTimescale;
	
	bool   bHealthUseLowEffect;
	bool   bHealthUseCriticalEffect;
	int    iHealthLow; 
	int    iHealthCritical;
	double dHealthLowTimescale; 
	double dHealthCriticalTimescale;
	
	bool  bHealthRegenEnabled;
	int   iHealthDamageDelay;
	int   iHealthRegenDelayNormal;
	int   iHealthRegenDelayCover;
	int   iHealthRegenDelaySneaking;
	float fHealthRegenRate;
	int   iHealthMaxValue;
	int   iHealthMinValue;
	
	bool bDisableTimescaleChanges;
	bool bDisableRecordings;
	bool bRevealWholeMap;
	bool bAllowCutsceneHUD;
	
    public PersonalizedHUD()
	{
		Tick += this.OnTick;
		LoadINI();
	}

		void LoadINI()
		{
			// Fill variables with settings
			ScriptSettings config = ScriptSettings.Load(@"scripts\PersonalizedHUD.ini");
			
			// If preset selected, load preset file
			iPreset	= config.GetValue<int>("PersonalizedHUD", "Preset", 0);
			if (iPreset > 0) { config = ScriptSettings.Load(@"scripts\PersonalizedHUD_" + iPreset + ".ini"); }

			bAudioPoliceScanner         = config.GetValue<bool>("Audio", "DisablePoliceScanner", true);
			bAudioWantedMusic           = config.GetValue<bool>("Audio", "DisableWantedMusic", true);
			bAudioFlyingMusic           = config.GetValue<bool>("Audio", "DisableFlyingMusic", true);
			bAudioPauseMusic            = config.GetValue<bool>("Audio", "DisablePauseMusic", false);
	
			iHUDAutohideCooldown        = config.GetValue<int>("HUD", "AutohideCooldown", 100);
			
			iHUDHideWantedLevel         = config.GetValue<int>("HUD", "HideWantedLevel", 0);
			iHUDHideAmmo                = config.GetValue<int>("HUD", "HideAmmo", 1);
			iHUDHideVehicleName         = config.GetValue<int>("HUD", "HideVehicleName", 0);
			iHUDHideVehicleClass        = config.GetValue<int>("HUD", "HideVehicleClass", 0);
			iHUDHideAreaName            = config.GetValue<int>("HUD", "HideAreaName", 0);
			iHUDHideStreetName          = config.GetValue<int>("HUD", "HideStreetName", 0);
			
			iHUDShowAmmoCooldown        = config.GetValue<int>("HUD", "ShowAmmoCooldown", 125);
			bHUDShowAmmoDuringReload    = config.GetValue<bool>("HUD", "ShowAmmoDuringReload", true);
			bHUDShowAmmoInCombat        = config.GetValue<bool>("HUD", "ShowAmmoInCombat", true);
			bHUDShowCashChanges         = config.GetValue<bool>("HUD", "ShowCashChanges", true);
			bHUDHideCash                = config.GetValue<bool>("HUD", "HideCash", false);
			bHUDHideReticle             = config.GetValue<bool>("HUD", "HideReticle", false);
			bHUDHideHelpText            = config.GetValue<bool>("HUD", "HideHelpText", false);
			bHUDHideSubtitleText        = config.GetValue<bool>("HUD", "HideSubtitleText", false);
			
			iHUDShowWithWantedLevel     = config.GetValue<int>("HUD", "ShowWithWantedLevel", 0);
			bHUDShowWithPhone           = config.GetValue<bool>("HUD", "ShowWithPhone", true);
			bHUDShowWithButton          = config.GetValue<bool>("HUD", "ShowWithButton", true);
			bHUDShowInVehicle           = config.GetValue<bool>("HUD", "ShowInVehicle", false);
			bHUDShowWhileSprinting      = config.GetValue<bool>("HUD", "ShowWhileSprinting", false);
			bHUDShowWhileArmed          = config.GetValue<bool>("HUD", "ShowWhileArmed", false);
			bHUDShowInCombat            = config.GetValue<bool>("HUD", "ShowInCombat", false);
			bHUDShowInCover             = config.GetValue<bool>("HUD", "ShowInCover", false);
			bHUDShowWhileReloading      = config.GetValue<bool>("HUD", "ShowWhileReloading", false);
			bHUDShowWhileSneaking       = config.GetValue<bool>("HUD", "ShowWhileSneaking", false);
			bHUDShowWhenHit             = config.GetValue<bool>("HUD", "ShowWhenHit", false);
			bHUDShowDuringRegen         = config.GetValue<bool>("HUD", "ShowDuringRegen", false);
			bHUDShowInMission        	= config.GetValue<bool>("HUD", "AlwaysShowInMission", false);
			bHUDShowWithWaypoint      	= config.GetValue<bool>("HUD", "ShowWithWaypoint", false);
			
			bRadarBigMapWithPhone       = config.GetValue<bool>("Radar", "BigMapWithPhone", false);
			bRadarBigMapWithButton      = config.GetValue<bool>("Radar", "BigMapWithButton", false);
			iRadarAutohideCooldown      = config.GetValue<int>("Radar", "AutohideCooldown", 300);
			
			iRadarShowWithWantedLevel   = config.GetValue<int>("Radar", "ShowWithWantedLevel", 1);
			bRadarShowWithPhone         = config.GetValue<bool>("Radar", "ShowWithPhone", true);
			bRadarShowWithButton        = config.GetValue<bool>("Radar", "ShowWithButton", true);
			bRadarShowInVehicle         = config.GetValue<bool>("Radar", "ShowWhileDriving", true);
			bRadarShowWhilePassenger    = config.GetValue<bool>("Radar", "ShowWhilePassenger", true);
			bRadarShowWhileSprinting    = config.GetValue<bool>("Radar", "ShowWhileSprinting", false);
			bRadarShowWhileArmed        = config.GetValue<bool>("Radar", "ShowWhileArmed", false);
			bRadarShowInCombat          = config.GetValue<bool>("Radar", "ShowInCombat", false);
			bRadarShowInCover           = config.GetValue<bool>("Radar", "ShowInCover", true);
			bRadarShowWhileReloading    = config.GetValue<bool>("Radar", "ShowWhileReloading", false);
			bRadarShowWhileSneaking     = config.GetValue<bool>("Radar", "ShowWhileSneaking", true);
			bRadarShowWhenHit           = config.GetValue<bool>("Radar", "ShowWhenHit", true);
			bRadarShowDuringRegen       = config.GetValue<bool>("Radar", "ShowDuringRegen", true);
			bRadarShowInMission         = config.GetValue<bool>("Radar", "AlwaysShowInMission", true);
			bRadarShowWithWaypoint      = config.GetValue<bool>("Radar", "ShowWithWaypoint", true);
			
			bRadarZoomCustomize         = config.GetValue<bool>("RadarZoom", "Customize", true);
			
			iRadarZoomNormal            = config.GetValue<int>("RadarZoom", "RadarZoomNormal", 300);
			iRadarZoomSprinting         = config.GetValue<int>("RadarZoom", "RadarZoomSprinting", 500);
			iRadarZoomAiming            = config.GetValue<int>("RadarZoom", "RadarZoomAiming", 400);
			iRadarZoomSneaking          = config.GetValue<int>("RadarZoom", "RadarZoomSneaking", 1);
			iRadarZoomVehicle           = config.GetValue<int>("RadarZoom", "RadarZoomVehicle", 1000);
			iRadarZoomAccelerating      = config.GetValue<int>("RadarZoom", "RadarZoomAccelerating", 900);
			iRadarZoomPhone             = config.GetValue<int>("RadarZoom", "RadarZoomPhone", 900);
			iRadarZoomPhoneVeh          = config.GetValue<int>("RadarZoom", "RadarZoomPhoneVeh", 1100);
			
			iPoliceBlipAutohideCooldown = config.GetValue<int>("PoliceBlips", "AutohideCooldown", 300);
			
			iPoliceBlipWithWantedLevel  = config.GetValue<int>("PoliceBlips", "ShowWithWantedLevel", 0);
			bPoliceBlipWithPhone        = config.GetValue<bool>("PoliceBlips", "ShowWithPhone", true);
			bPoliceBlipWithButton       = config.GetValue<bool>("PoliceBlips", "ShowWithButton", false);
			bPoliceBlipInVehicle        = config.GetValue<bool>("PoliceBlips", "ShowInVehicle", true);
			bPoliceBlipWhileSprinting   = config.GetValue<bool>("PoliceBlips", "ShowWhileSprinting", false);
			bPoliceBlipWhileArmed       = config.GetValue<bool>("PoliceBlips", "ShowWhileArmed", false);
			bPoliceBlipInCombat         = config.GetValue<bool>("PoliceBlips", "ShowInCombat", false);
			bPoliceBlipInCover          = config.GetValue<bool>("PoliceBlips", "ShowInCover", true);
			bPoliceBlipWhileReloading   = config.GetValue<bool>("PoliceBlips", "ShowWhileReloading", false);
			bPoliceBlipWhileSneaking    = config.GetValue<bool>("PoliceBlips", "ShowWhileSneaking", true);
			bPoliceBlipWhenHit          = config.GetValue<bool>("PoliceBlips", "ShowWhenHit", false);
			bPoliceBlipDuringRegen      = config.GetValue<bool>("PoliceBlips", "ShowDuringRegen", false);
			
			bHealthUseDamageEffect      = config.GetValue<bool>("Health", "UseDamageEffect", true);
			
			bHealthUseLowEffect         = config.GetValue<bool>("Health", "UseLowEffect", true);
			bHealthUseCriticalEffect    = config.GetValue<bool>("Health", "UseCriticalEffect", true);
			iHealthLow                  = config.GetValue<int>("Health", "LowHealth", 30);
			iHealthCritical             = config.GetValue<int>("Health", "CriticalHealth", 10);
			dHealthLowTimescale         = config.GetValue<double>("Health", "LowTimescale", 0.0);
			dHealthCriticalTimescale    = config.GetValue<double>("Health", "CriticalTimescale", 0.0);
			
			bHealthWastedScreenEffect   = config.GetValue<bool>("Health", "DisableWastedEffect", false);
			dHealthWastedTimescale      = config.GetValue<double>("Health", "WastedTimescale", 0.0);
			
			bHealthRegenEnabled         = config.GetValue<bool>("Health", "Regeneration", false);
			iHealthDamageDelay          = config.GetValue<int>("Health", "DamageDelay", 600);
			iHealthRegenDelayNormal     = config.GetValue<int>("Health", "RegenDelayNormal", 14);
			iHealthRegenDelayCover      = config.GetValue<int>("Health", "RegenDelayCover", 11);
			iHealthRegenDelaySneaking   = config.GetValue<int>("Health", "RegenDelaySneaking", 13);
			fHealthRegenRate            = config.GetValue<float>("Health", "RegenRate", 1.0f);
			iHealthMaxValue             = config.GetValue<int>("Health", "MaxHealth", 100);
			iHealthMinValue             = config.GetValue<int>("Health", "MinHealth", 5);
			
			bDisableTimescaleChanges    = config.GetValue<bool>("Extra", "DisableStoryTimescaleChanges", false);
			bDisableRecordings          = config.GetValue<bool>("Extra", "DisableRecordings", false);
			bRevealWholeMap           	= config.GetValue<bool>("Extra", "RevealWholeMap", false);
			bAllowCutsceneHUD           = config.GetValue<bool>("Extra", "AllowHUDinCutscenes", false);
			
			if (iHealthLow <= 99 && iHealthLow >= 10) { iHealthLow = iHealthLow + 100; } else { iHealthLow = 120; }
			if (iHealthCritical <= 99 && iHealthCritical >= 6) { iHealthCritical = iHealthCritical + 100; } else { iHealthCritical = 107; }
		}

		void AudioOptions()
		{
			if (bAudioPoliceScanner == true) {
				Audio.SetAudioFlag("PoliceScannerDisabled", true); 
				Audio.SetAudioFlag("EnableHeadsetBeep", false); }
				
			if (bAudioWantedMusic == true) {
				Audio.SetAudioFlag("WantedMusicDisabled", true);
				Audio.SetAudioFlag("WantedMusicOnMission", false); }
				
			if (bAudioFlyingMusic == true) { Audio.SetAudioFlag("DisableFlightMusic", true); }
			
			if (bAudioPauseMusic == true) { Audio.SetAudioFlag("PlayMenuMusic", false); }
		}
		
		void StatusChecks()
		{	
			// Get current health value
			iHealthCurrValue = (int)Math.Round((float)(Function.Call<int>(Hash.GET_ENTITY_HEALTH, Game.Player.Character.Handle))); 
		
			// bStatusDead - am I dead?
			if (Game.Player.Character.IsDead || iHealthCurrValue < 0) { bStatusDead = true; } else { bStatusDead = false; }
			
			// bStatusBusted - am I busted?
			bStatusBusted = Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED);
			
			// bStatusHasControl - do I have control of my character?
			if (bStatusDead || !Game.Player.CanControlCharacter || Game.Player.Character.IsDiving || Game.Player.Character.IsVaulting || Game.Player.Character.IsClimbing || Game.Player.Character.IsJumping || Game.Player.Character.IsFalling || Game.Player.Character.IsGettingIntoAVehicle || Game.Player.Character.IsGettingUp || Game.Player.Character.IsRagdoll || Game.Player.Character.IsJacking || Game.Player.Character.IsBeingJacked || Game.Player.Character.IsBeingStunned)
				{ bStatusHasControl = false; } else { bStatusHasControl = true; }
			
			// bStatusArmed - am I holding a weapon?
			bStatusArmed = Function.Call<bool>(Hash.IS_PED_ARMED, Game.Player.Character, 7);
			
			// bStatusButton - am I pressing the character selection (d-pad down) button?
			bStatusButton = Game.IsControlPressed(2, GTA.Control.CharacterWheel);

			// bStatusSprinting - am I sprinting?
			if (Game.Player.Character.IsSprinting && bStatusSprinting == false) { bStatusSprinting = true; }
			if (!Game.Player.Character.IsSprinting) { bStatusSprinting = false; } else { bStatusSprinting = true; }
			
			// bStatusSneaking - am I sneaking or crouched?
			bStatusSneaking = Function.Call<bool>(Hash.GET_PED_STEALTH_MOVEMENT, Game.Player.Character);
			
			// bStatusCombat - am I aiming or shooting?
			if (Game.Player.IsAiming || Game.Player.Character.IsAimingFromCover || Game.Player.IsTargettingAnything || Game.Player.Character.IsShooting || Game.Player.Character.IsInMeleeCombat || Game.Player.Character.IsDoingDriveBy || Game.Player.Character.IsPerformingStealthKill || Game.Player.Character.IsBeingStunned)
				{ bStatusCombat = true; } else { bStatusCombat = false; }
			
			// bStatusCover - am I in cover?
			bStatusCover = Game.Player.Character.IsInCover();
			
			// bStatusVehicle, bStatusDriving & bStatusPassenger - am I in a vehicle and am I driving?
			if (Game.Player.Character.IsInVehicle()) {
					bStatusPassenger = !(Game.Player.LastVehicle.Driver == Game.Player.Character);
					if (!bStatusPassenger || bRadarShowWhilePassenger) {
						bStatusVehicle = true;
						if ((Game.IsControlPressed(2, GTA.Control.VehicleAccelerate)) || (Game.IsControlPressed(2, GTA.Control.VehicleBrake)) || (Game.IsControlPressed(2, GTA.Control.VehicleHandbrake)))
							{ bStatusDriving = true; } else { bStatusDriving = false; }
					} else { bStatusVehicle = false; }
			} else { bStatusVehicle = false; bStatusPassenger = false; }
			
			 
			// bStatusDamage/bStatusHealing/iHealthDamageDelay - did my health just change?
			if (iHealthDamageDelay < 61) { iHealthDamageDelay = 61; } // Minimum for this value is 61
			if (iHealthDamageTimer > 0) { iHealthDamageTimer = iHealthDamageTimer - 1; }
			if (iHealthCurrValue < iHealthPrevValue) {
				bStatusDamage = true;
				iHealthDamageTimer = iHealthDamageDelay;
			} else { bStatusDamage = false; }
			if (iHealthCurrValue > iHealthPrevValue) { bStatusHealing = true; } else { bStatusHealing = false; }
			iHealthPrevValue = (int)Math.Round((float)(Function.Call<int>(Hash.GET_ENTITY_HEALTH, Game.Player.Character.Handle))); // set the current health as previous value for damage check (rounded to nearest integer)
			
			// bStatusPhone - am I on my phone?
			if (Function.Call<bool>(Hash.IS_PED_RUNNING_MOBILE_PHONE_TASK, Game.Player.Character) || Function.Call<bool>(Hash.IS_PLAYING_PHONE_GESTURE_ANIM, Game.Player.Character)) { bStatusPhone = true; } else { bStatusPhone = false; }
			
			// bStatusReloading - am I reloading my weapon?
			bStatusReloading = Game.Player.Character.IsReloading;

			// bStatusMission - is ON_MISSION=1?
			bStatusMission = (Function.Call<bool>(Hash.GET_MISSION_FLAG) && bStatusHasControl);

			// bStatusCutscene - is a cutscene playing?
			bStatusCutscene = (Function.Call<bool>(Hash.IS_CUTSCENE_PLAYING) || !(Function.Call<bool>(Hash.IS_GAMEPLAY_CAM_RENDERING)));
			
			// bStatusWaypoint - is there a custom waypoint set?
			bStatusWaypoint = Function.Call<bool>(Hash.IS_WAYPOINT_ACTIVE);
			
		}
		
		void HUDCycle()
		{
			// reset boolean
			bHUDEnabled = false;
			
			// If cooldown timer still going or disabled, show HUD
			if ((iHUDCooldownTimer > 0) || (iHUDAutohideCooldown == -1)) { bHUDEnabled = true; } else { bHUDEnabled = false; }

			// if still going, reduce cooldown timer by 1
			if (iHUDCooldownTimer > 0) { iHUDCooldownTimer = iHUDCooldownTimer - 1; } else { iHUDSkipOneTick = 0; }

			// separate ammo timer
			if (iHUDHideAmmo == 1) {
				if (iHUDShowAmmoTimer > 0) { iHUDShowAmmoTimer = iHUDShowAmmoTimer - 1; }
				if ((bStatusReloading && bHUDShowAmmoDuringReload) || (bStatusCombat && bHUDShowAmmoInCombat)) { iHUDShowAmmoTimer = iHUDShowAmmoCooldown; }
			}

			if (!bHUDEnabled) { // hide HUD components
				Function.Call(Hash.DISPLAY_HUD, true); 
				if (iHUDHideWantedLevel == 1)                    { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 1); }  // wanted level
				if (iHUDHideAmmo == 1 && iHUDShowAmmoTimer == 0) { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 2); }  // weapon ammo
				if (bHUDHideCash == true)                        { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 3); }  // cash
				if (!bHUDShowCashChanges)       			     { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 13); } else { Function.Call(Hash.SHOW_HUD_COMPONENT_THIS_FRAME, 13); }
				if (iHUDHideVehicleName == 1)                    { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 6); }  // vehicle name
				if (iHUDHideAreaName == 1)                       { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 7); }  // area name
				if (iHUDHideVehicleClass == 1)                   { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 8); }  // vehicle class
				if (iHUDHideStreetName == 1)                     { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 9); }  // street name
			} else {
				// force show or disable HUD components depending on settings
				if (iHUDHideWantedLevel == 2)       { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 1); } else { Function.Call(Hash.SHOW_HUD_COMPONENT_THIS_FRAME, 1); }
				if (iHUDHideAmmo == 2)              { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 2); } else { Function.Call(Hash.SHOW_HUD_COMPONENT_THIS_FRAME, 2); }
				if (bHUDHideCash == true)           { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 3); } else { Function.Call(Hash.SHOW_HUD_COMPONENT_THIS_FRAME, 3); }
				if (!bHUDShowCashChanges)           { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 13); } else { Function.Call(Hash.SHOW_HUD_COMPONENT_THIS_FRAME, 13); }
				if (iHUDSkipOneTick != 1) { 			// weird shit happening with these, re-hiding for one tick makes them show more consistently
					if (iHUDHideVehicleName == 2)       { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 6); } else { Function.Call(Hash.SHOW_HUD_COMPONENT_THIS_FRAME, 6); }
					if (iHUDHideAreaName == 2)          { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 7); } else { Function.Call(Hash.SHOW_HUD_COMPONENT_THIS_FRAME, 7); }
					if (iHUDHideVehicleClass == 2)      { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 8); } else { Function.Call(Hash.SHOW_HUD_COMPONENT_THIS_FRAME, 8); }
					if (iHUDHideStreetName == 2)        { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 9); } else { Function.Call(Hash.SHOW_HUD_COMPONENT_THIS_FRAME, 9); }
					if (iHUDSkipOneTick == 0) 			{ iHUDSkipOneTick++; }
				}
			}

			if (bStatusHasControl) {
				// Cooldown checks, don't check cooldowns if you're not in control
				if (bStatusArmed && bHUDShowWhileArmed)         { iHUDCooldownTimer = iHUDAutohideCooldown; }
				if (bStatusButton && bHUDShowWithButton)        { if (iHUDAutohideCooldown < iButtonCooldownMin) { iHUDCooldownTimer = iButtonCooldownMin; } else { iHUDCooldownTimer = iHUDAutohideCooldown; } }
				if (bStatusCombat && bHUDShowInCombat)          { iHUDCooldownTimer = iHUDAutohideCooldown; }
				if (bStatusCover && bHUDShowInCover)            { iHUDCooldownTimer = iHUDAutohideCooldown; }
				if (bStatusReloading && bHUDShowWhileReloading) { iHUDCooldownTimer = iHUDAutohideCooldown; }
				if (bStatusSneaking && bHUDShowWhileSneaking)   { iHUDCooldownTimer = iHUDAutohideCooldown; }
				if (bStatusSprinting && bHUDShowWhileSprinting) { iHUDCooldownTimer = iHUDAutohideCooldown; }
			}
			
			if (bStatusPhone && bHUDShowWithPhone)          { iHUDCooldownTimer = iHUDAutohideCooldown; }
			if ((iHUDShowWithWantedLevel > 0) && (Game.Player.WantedLevel >= iHUDShowWithWantedLevel)) { iHUDCooldownTimer = iHUDAutohideCooldown; }
			if (bStatusVehicle && bHUDShowInVehicle)		{ iHUDCooldownTimer = iHUDAutohideCooldown; }
			if (bStatusMission && bHUDShowInMission) 		{ iHUDCooldownTimer = iHUDAutohideCooldown + 300; }
			if (bStatusWaypoint && bHUDShowWithWaypoint) 	{ iHUDCooldownTimer = iHUDAutohideCooldown; }
			
			// always check damage and regen cooldowns, however
			if (bStatusDamage && bHUDShowWhenHit)           { if (iHUDAutohideCooldown < iButtonCooldownMin) { iHUDCooldownTimer = iButtonCooldownMin; } else { iHUDCooldownTimer = iHUDAutohideCooldown; } }
			if (bHealthRegenEngaged && bHUDShowDuringRegen) { iHUDCooldownTimer = iHUDAutohideCooldown; }
			
			// disable during cutscenes
			if (bStatusCutscene)							{ iHUDCooldownTimer = 0; }
			
			// Misc HUD options
			if (bHUDShowCashChanges) { Function.Call(Hash.SHOW_HUD_COMPONENT_THIS_FRAME, 13); }
			if (bHUDHideReticle == true) { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 14); }
			if (bHUDHideHelpText == true) { 
				Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 10);
				Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 11);
				Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 12);
			}
			if (bHUDHideSubtitleText == true) { Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 15); }
		}

		
		void RadarCycle()
		{
			// reset boolean
			bRadarEnabled = false;
			
			// If cooldown timer runs out, show radar (or if set to -1, always show)
			if ((iRadarCooldownTimer > 0) || (iRadarAutohideCooldown == -1)) { bRadarEnabled = true; } else { bRadarEnabled = false; }

			// if still going, reduce cooldown timer by 1
			if (iRadarCooldownTimer > 0) { iRadarCooldownTimer = iRadarCooldownTimer - 1; }

			if (bRadarEnabled) { // show radar
				if (bStatusPhone && bRadarBigMapWithPhone && !Function.Call<bool>(Hash.IS_MOBILE_PHONE_CALL_ONGOING)) { 
					Function.Call(Hash._SET_RADAR_BIGMAP_ENABLED, true, false);
				} else {
					Function.Call(Hash._SET_RADAR_BIGMAP_ENABLED, false, false);
				}
				Function.Call(Hash.DISPLAY_RADAR, true);
			} else {
				Function.Call(Hash.DISPLAY_RADAR, false);
				Function.Call(Hash._DISABLE_RADAR_THIS_FRAME);
			}
			
			// Cooldown checks, don't check cooldowns if you're not in control
			if (bStatusHasControl) {
				
				if (bStatusArmed && bRadarShowWhileArmed)         { iRadarCooldownTimer = iRadarAutohideCooldown; }
				if (bStatusButton && bRadarShowWithButton)        { if (iRadarAutohideCooldown < iButtonCooldownMin) { iRadarCooldownTimer = iButtonCooldownMin; } else { iRadarCooldownTimer = iRadarAutohideCooldown; } }
				if (bStatusCombat && bRadarShowInCombat)          { iRadarCooldownTimer = iRadarAutohideCooldown; }
				if (bStatusCover && bRadarShowInCover)            { iRadarCooldownTimer = iRadarAutohideCooldown; }
				if (bStatusReloading && bRadarShowWhileReloading) { iRadarCooldownTimer = iRadarAutohideCooldown; }
				if (bStatusSneaking && bRadarShowWhileSneaking)   { iRadarCooldownTimer = iRadarAutohideCooldown; }
				if (bStatusSprinting && bRadarShowWhileSprinting) { iRadarCooldownTimer = iRadarAutohideCooldown; }
			}
			if (bStatusPhone && bRadarShowWithPhone)          	{ iRadarCooldownTimer = iRadarAutohideCooldown; }
			if ((iRadarShowWithWantedLevel > 0) && (Game.Player.WantedLevel >= iRadarShowWithWantedLevel)) { iRadarCooldownTimer = iRadarAutohideCooldown; }
			if (bStatusVehicle && bRadarShowInVehicle) 			{ iRadarCooldownTimer = iRadarAutohideCooldown; }
			if (bStatusMission && bRadarShowInMission) 			{ iRadarCooldownTimer = iRadarAutohideCooldown; }
			if (bStatusWaypoint && bRadarShowWithWaypoint) 		{ iRadarCooldownTimer = iRadarAutohideCooldown; }
			if (bStatusDamage && bRadarShowWhenHit)           	{ if (iRadarAutohideCooldown < iButtonCooldownMin) { iRadarCooldownTimer = iButtonCooldownMin; } else { iRadarCooldownTimer = iRadarAutohideCooldown; } }
			if (bHealthRegenEngaged && bRadarShowDuringRegen)	{ iRadarCooldownTimer = iRadarAutohideCooldown; }
			
			// disable during cutscenes
			if (bStatusCutscene) 								{ iRadarCooldownTimer = 0; }
		}
		
		void PoliceBlipCycle()
		{
			// reset boolean
			bPoliceBlipEnabled = false;
			
			// If cooldown timer runs out, show radar
			if ((iPoliceBlipCooldownTimer > 0) || (iPoliceBlipAutohideCooldown == -1)) { bPoliceBlipEnabled = true; } else { bPoliceBlipEnabled = false; }

			// if still going, reduce cooldown timer by 1
			if (iPoliceBlipCooldownTimer > 0) { iPoliceBlipCooldownTimer = iPoliceBlipCooldownTimer - 1; }

			if (bPoliceBlipEnabled) { Game.ShowsPoliceBlipsOnRadar = true; } else { Game.ShowsPoliceBlipsOnRadar = false; }
			
			// Cooldown checks
			if (bStatusArmed && bPoliceBlipWhileArmed)         { iPoliceBlipCooldownTimer = iPoliceBlipAutohideCooldown; }
			if (bStatusButton && bPoliceBlipWithButton)        { if (iPoliceBlipAutohideCooldown < iButtonCooldownMin) { iPoliceBlipCooldownTimer = iButtonCooldownMin; } else { iPoliceBlipCooldownTimer = iPoliceBlipAutohideCooldown; } }
			if (bStatusCombat && bPoliceBlipInCombat)          { iPoliceBlipCooldownTimer = iPoliceBlipAutohideCooldown; }
			if (bStatusCover && bPoliceBlipInCover)            { iPoliceBlipCooldownTimer = iPoliceBlipAutohideCooldown; }
			if (bStatusDamage && bPoliceBlipWhenHit)           { if (iPoliceBlipAutohideCooldown < iButtonCooldownMin) { iPoliceBlipCooldownTimer = iButtonCooldownMin; } else { iPoliceBlipCooldownTimer = iPoliceBlipAutohideCooldown; } }
			if (bStatusPhone && bPoliceBlipWithPhone)          { iPoliceBlipCooldownTimer = iPoliceBlipAutohideCooldown; }
			if (bStatusReloading && bPoliceBlipWhileReloading) { iPoliceBlipCooldownTimer = iPoliceBlipAutohideCooldown; }
			if (bStatusSneaking && bPoliceBlipWhileSneaking)   { iPoliceBlipCooldownTimer = iPoliceBlipAutohideCooldown; }
			if (bStatusSprinting && bPoliceBlipWhileSprinting) { iPoliceBlipCooldownTimer = iPoliceBlipAutohideCooldown; }
			if (bHealthRegenEngaged && bPoliceBlipDuringRegen) { iPoliceBlipCooldownTimer = iPoliceBlipAutohideCooldown; }
			if ((iPoliceBlipWithWantedLevel > 0) && (Game.Player.WantedLevel >= iPoliceBlipWithWantedLevel)) { iPoliceBlipCooldownTimer = iPoliceBlipAutohideCooldown; }
			if ((bStatusVehicle || bStatusDriving) && bPoliceBlipInVehicle) { iPoliceBlipCooldownTimer = iPoliceBlipAutohideCooldown; }
			
		}
		
		void RadarZoomCycle()
		{
			// reset radar zoom
			iRadarZoomType = 0;
			
			// choose radar zoom
			if (bStatusSprinting) { iRadarZoomType = 1; }
			if (bStatusCombat)    { iRadarZoomType = 2; }
			if (bStatusSneaking)  { iRadarZoomType = 3; }
			if (bStatusVehicle)   { iRadarZoomType = 4; }
			if (bStatusDriving)   { iRadarZoomType = 5; }
			if (bStatusPhone)     { iRadarZoomType = 6; }
			if (bStatusVehicle && bStatusPhone) {
				iRadarZoomType = 7;
				//iRadarZoomPhoneVeh = iRadarZoomPhone + iRadarZoomVehicle;
				if (iRadarZoomPhoneVeh > 1400) { iRadarZoomPhoneVeh = 1400; }
			}
			
			// Process radar zoom type
			// MAX RADAR ZOOM POSSIBLE IS 1400
			// RADAR ZOOM TYPES: 0 = normal; 1 = sprinting; 2 = aiming; 3 = sneaking; 4 = vehicle; 5 = accelerating; 6 = phone; 7 = phone in vehicle
			if (iRadarZoomType == 0) { Function.Call(Hash.SET_RADAR_ZOOM, iRadarZoomNormal); }       
			if (iRadarZoomType == 1) { Function.Call(Hash.SET_RADAR_ZOOM, iRadarZoomSprinting); }    
			if (iRadarZoomType == 2) { Function.Call(Hash.SET_RADAR_ZOOM, iRadarZoomAiming); }       
			if (iRadarZoomType == 3) { Function.Call(Hash.SET_RADAR_ZOOM, iRadarZoomSneaking); }     
			if (iRadarZoomType == 4) { Function.Call(Hash.SET_RADAR_ZOOM, iRadarZoomVehicle); }      
			if (iRadarZoomType == 5) { Function.Call(Hash.SET_RADAR_ZOOM, iRadarZoomAccelerating); } 
			if (iRadarZoomType == 6) { Function.Call(Hash.SET_RADAR_ZOOM, iRadarZoomPhone); }        
			if (iRadarZoomType == 7) { Function.Call(Hash.SET_RADAR_ZOOM, iRadarZoomPhoneVeh); }     
			
		}
		
		void HealthRegenCycle()
		{
			bHealthRegenEngaged = false; // reset boolean
			
			if (iHealthMaxValue <= 100 && iHealthMaxValue <= 6) { iHealthMaxValue = iHealthMaxValue + 100; } else { iHealthMaxValue = 200; }
				
            if (iHealthRegenDelay > 0) { iHealthRegenDelay = iHealthRegenDelay - 1; } // Reduce delay by 1 every tick
			
            if (iHealthRegenDelay == 0 && iHealthCurrValue < iHealthMaxValue)
            { // Check conditions for health regeneration
                bHealthRegenEngaged = true;
                if (bStatusSprinting) { bHealthRegenEngaged = false; }
                if (bHealthRegenEngaged == true)
                { // if conditions met, let's calculate and add health
                    iHealthNewValue = (int)Math.Round(((float)iHealthCurrValue) + (fHealthRegenRate));
                    Function.Call(Hash.SET_ENTITY_HEALTH, Game.Player.Character.Handle, iHealthNewValue);
                    // Check if player is in cover, but not aiming
                    if (bStatusCover && !Game.Player.Character.IsAimingFromCover) { iHealthRegenDelay = iHealthRegenDelayCover; }
						else { 
							if (bStatusSneaking) { iHealthRegenDelay = iHealthRegenDelaySneaking; }
								else { iHealthRegenDelay = iHealthRegenDelayNormal; }
						}
                }
                else
				{
					iHealthDamageTimer = iHealthDamageDelay; // delay if conditions not met
					iHealthRegenDelay = iHealthRegenDelayNormal;
				} 
            }
		}
		
		void HealthEffects()
		{
			//iHealthMinValue - here because regen cycle is not always processed.
			if (iHealthMinValue != -1) {
				if (iHealthMinValue <= 99 && iHealthMinValue >= 5) { iHealthMinValue = iHealthMinValue + 100; } else { iHealthMinValue = 105; }
				if (iHealthCurrValue < iHealthMinValue) { Function.Call(Hash.SET_ENTITY_HEALTH, Game.Player.Character.Handle, 0); }  }			
			
			// Damage effect
			if (bHealthUseDamageEffect) {
				if (iHealthDamageTimer < (iHealthDamageTimer - 50)) { Function.Call(Hash._STOP_SCREEN_EFFECT, "RampageOut"); }
				if (bStatusDamage) { Function.Call(Hash._START_SCREEN_EFFECT, "RampageOut", 0, true); }
			}
			
			if (bStatusHealing && (iHealthCurrValue > iHealthLow) && bHealthLowThreshold)
			{
				bHealthLowThreshold = false;
				if (dGameTimescale > 0.0) { dGameTimescale = 1.0; }
				Function.Call(Hash._STOP_SCREEN_EFFECT, "MenuMGHeistIn");
				Function.Call(Hash._STOP_SCREEN_EFFECT, "HeistTripSkipFade");
				Function.Call(Hash._START_SCREEN_EFFECT, "FocusOut", 0, false);
			}
			
			// Low and Critical Health Screen Effects
			if (iHealthCurrValue <= iHealthLow)
			{
				bHealthLowThreshold = true;
				if (iHealthCurrValue <= iHealthCritical) {
					Function.Call(Hash._START_SCREEN_EFFECT, "MenuMGHeistIn", 0, true);
					Function.Call(Hash._START_SCREEN_EFFECT, "HeistTripSkipFade", 0, true);
					if (dHealthCriticalTimescale != 0.0) { dGameTimescale = dHealthCriticalTimescale; }
				} else {
					Function.Call(Hash._STOP_SCREEN_EFFECT, "MenuMGHeistIn");
					Function.Call(Hash._START_SCREEN_EFFECT, "HeistTripSkipFade", 0, true);
				if (dHealthLowTimescale != 0.0 ) { dGameTimescale = dHealthLowTimescale; } }
			}	
        }
	
		void OnTick(object sender, EventArgs e)
		{
			StatusChecks();
			AudioOptions();
			if (!bStatusDead && !bStatusBusted) { HealthEffects(); }
			if (bStatusHasControl) { // don't even process these if you're not in control
				if (bHealthRegenEnabled && iHealthDamageTimer == 0) { HealthRegenCycle(); }
				if (bRadarZoomCustomize && bRadarEnabled)           { RadarZoomCycle(); }
				if (bRadarEnabled)                                  { PoliceBlipCycle(); }
			}
			
			HUDCycle();
			RadarCycle();
			if (bStatusCutscene) { Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME); }
			
			// Wasted effects (applies to both wasted and busted until further notice)
			if (bHealthWastedScreenEffect && (bStatusDead || bStatusBusted)) { Function.Call(Hash._STOP_ALL_SCREEN_EFFECTS); }
			if (dHealthWastedTimescale != 0.0 && (bStatusDead || bStatusBusted)) { dGameTimescale = dHealthWastedTimescale; }
			if (!bDisableTimescaleChanges && !bStatusDead && !bStatusBusted) { dGameTimescale = 1.0; }
			if (dGameTimescale != 0.0) { 
				if (dGameTimescale > 1.0 || dGameTimescale < 0.0) { dGameTimescale = 1.0; } // if for whatever reason this ends up becoming invalid, reset to normal
				Function.Call(Hash.SET_TIME_SCALE, dGameTimescale);
			} 
			if (dGameTimescale == 1.0) { dGameTimescale = 0.0; } // if set to 1.0 disable setting timescale
			
			//UI.ShowSubtitle(dGameTimescale.ToString());
							
			// disable replays/recordings
			if (Function.Call<bool>(Hash._IS_RECORDING) && bDisableRecordings) { Function.Call(Hash._STOP_RECORDING_AND_DISCARD_CLIP); }
			
			// Reveal whole map
			if (bRevealWholeMap) { Function.Call((Hash)0xF8DEE0A5600CBB93, true); } // _SET_MINIMAP_REVEALED
		}
}