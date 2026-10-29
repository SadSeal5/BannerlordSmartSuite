using System;
using MCM.Abstractions.Base.Global;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Common;

namespace BannerlordSmartSuite
{
    public class Settings : AttributeGlobalSettings<Settings>
    {
        public override string Id => "BannerlordSmartSuite";
        public override string DisplayName => "Bannerlord Smart Suite";
        public override string FolderName => "BannerlordSmartSuite";
        public override string FormatType => "json2";

        private static Settings _fallbackInstance;
        public static new Settings Instance
        {
            get
            {
                try
                {
                    return AttributeGlobalSettings<Settings>.Instance ?? (_fallbackInstance ??= new Settings());
                }
                catch
                {
                    return _fallbackInstance ??= new Settings();
                }
            }
        }

        #region General

        [SettingPropertyBool("Mod Enabled", Order = 0, RequireRestart = false, HintText = "Master switch to enable or disable the entire suite.")]
        [SettingPropertyGroup("General")]
        public bool ModEnabled { get; set; } = true;

        #endregion

        #region 1. Smart Captain

        [SettingPropertyBool("Smart Captain Enabled", Order = 0, RequireRestart = false, HintText = "Enable or disable intelligent captain assignment.")]
        [SettingPropertyGroup("1. Smart Captain")]
        public bool SmartCaptainEnabled { get; set; } = true;

        [SettingPropertyBool("Optimize Main Hero", Order = 1, RequireRestart = false, HintText = "Whether the player character is evaluated and assigned to captain roles.")]
        [SettingPropertyGroup("1. Smart Captain")]
        public bool OptimizeMainHero { get; set; } = true;

        [SettingPropertyBool("Auto Assign On Deployment", Order = 2, RequireRestart = false, HintText = "Whether Order of Battle automatically assigns optimal captains to formations.")]
        [SettingPropertyGroup("1. Smart Captain")]
        public bool AutoAssignOnDeployment { get; set; } = true;

        [SettingPropertyBool("Mid-Battle Succession", Order = 3, RequireRestart = false, HintText = "Whether captaincy passes to the next best companion if the current formation captain falls in battle.")]
        [SettingPropertyGroup("1. Smart Captain")]
        public bool MidBattleSuccession { get; set; } = true;

        [SettingPropertyBool("Dismount Mounted Infantry Captains", Order = 4, RequireRestart = false, HintText = "Preference for foot captains in infantry formations, dismounting mounted captains who lead infantry.")]
        [SettingPropertyGroup("1. Smart Captain")]
        public bool DismountMountedInfantryCaptains { get; set; } = true;

        #endregion

        #region 2. Auto Settlement Builder

        [SettingPropertyBool("Auto Settlement Builder Enabled", Order = 0, RequireRestart = false, HintText = "Enable or disable automatic settlement building queue.")]
        [SettingPropertyGroup("2. Auto Settlement Builder")]
        public bool AutoSettlementBuilderEnabled { get; set; } = true;

        [SettingPropertyBool("Settlement Notifications Enabled", Order = 1, RequireRestart = false, HintText = "Display notifications when buildings or daily projects are auto-queued.")]
        [SettingPropertyGroup("2. Auto Settlement Builder")]
        public bool SettlementNotificationsEnabled { get; set; } = true;

        [SettingPropertyInteger("Food Deficit Threshold", 0, 100, "0", Order = 2, RequireRestart = false, HintText = "Food stock threshold below which food buildings and daily projects are prioritized.")]
        [SettingPropertyGroup("2. Auto Settlement Builder")]
        public int FoodDeficitThreshold { get; set; } = 10;

        [SettingPropertyFloatingInteger("Loyalty Critical Threshold", 0f, 100f, "#0", Order = 3, RequireRestart = false, HintText = "Loyalty threshold below which festivals or loyalty buildings are prioritized.")]
        [SettingPropertyGroup("2. Auto Settlement Builder")]
        public float LoyaltyCriticalThreshold { get; set; } = 40f;

        [SettingPropertyFloatingInteger("Security Critical Threshold", 0f, 100f, "#0", Order = 4, RequireRestart = false, HintText = "Security threshold below which security buildings are prioritized.")]
        [SettingPropertyGroup("2. Auto Settlement Builder")]
        public float SecurityCriticalThreshold { get; set; } = 40f;

        #endregion

        #region 3. Smart Troop Upgrade

        [SettingPropertyBool("Smart Troop Upgrade Enabled", Order = 0, RequireRestart = false, HintText = "Enable or disable automatic troop upgrades.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public bool SmartTroopUpgradeEnabled { get; set; } = true;

        [SettingPropertyFloatingInteger("Shield Infantry Ratio (%)", 0f, 100f, "#0'%'", Order = 1, RequireRestart = false, HintText = "Target quota percentage for shield infantry.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public float ShieldInfantryPercent { get; set; } = 40f;

        [SettingPropertyFloatingInteger("Shock Infantry Ratio (%)", 0f, 100f, "#0'%'", Order = 2, RequireRestart = false, HintText = "Target quota percentage for shock infantry.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public float ShockInfantryPercent { get; set; } = 10f;

        [SettingPropertyFloatingInteger("Ranged Ratio (%)", 0f, 100f, "#0'%'", Order = 3, RequireRestart = false, HintText = "Target quota percentage for foot archers/crossbowmen.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public float RangedPercent { get; set; } = 25f;

        [SettingPropertyFloatingInteger("Cavalry Ratio (%)", 0f, 100f, "#0'%'", Order = 4, RequireRestart = false, HintText = "Target quota percentage for melee cavalry.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public float CavalryPercent { get; set; } = 15f;

        [SettingPropertyFloatingInteger("Horse Archer Ratio (%)", 0f, 100f, "#0'%'", Order = 5, RequireRestart = false, HintText = "Target quota percentage for mounted archers.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public float HorseArcherPercent { get; set; } = 10f;

        [SettingPropertyInteger("Minimum Gold Reserve", 0, 100000, "0", Order = 6, RequireRestart = false, HintText = "Player gold reserve buffer. Upgrades will stop if gold would fall below this amount.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public int MinGoldReserve { get; set; } = 2000;

        [SettingPropertyInteger("Minimum Horse Reserve", 0, 50, "0", Order = 7, RequireRestart = false, HintText = "Preserve at least this many mount items in inventory before upgrading to cavalry.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public int MinHorseReserve { get; set; } = 5;

        [SettingPropertyBool("Respect Locked Troops", Order = 8, RequireRestart = false, HintText = "Do not auto-upgrade troops that are locked in the party screen.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public bool RespectLockedTroops { get; set; } = true;

        [SettingPropertyBool("Prioritize Noble Troops", Order = 9, RequireRestart = false, HintText = "Give priority weighting to noble/elite troop lines when upgrading.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public bool PrioritizeNobleTroops { get; set; } = true;

        [SettingPropertyDropdown("Shield Infantry Culture Priority", Order = 10, RequireRestart = false, HintText = "Preferred culture for shield infantry.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public Dropdown<string> ShieldCulturePriority { get; set; } = new Dropdown<string>(new string[] { "Sturgia", "Empire", "Vlandia", "Aserai", "Battania", "Khuzait" }, 0);

        [SettingPropertyDropdown("Ranged Foot Culture Priority", Order = 11, RequireRestart = false, HintText = "Preferred culture for ranged foot troops.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public Dropdown<string> RangedCulturePriority { get; set; } = new Dropdown<string>(new string[] { "Battania", "Empire", "Aserai", "Vlandia", "Khuzait", "Sturgia" }, 0);

        [SettingPropertyDropdown("Cavalry Culture Priority", Order = 12, RequireRestart = false, HintText = "Preferred culture for melee cavalry.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public Dropdown<string> CavalryCulturePriority { get; set; } = new Dropdown<string>(new string[] { "Vlandia", "Empire", "Sturgia", "Aserai", "Khuzait", "Battania" }, 0);

        [SettingPropertyDropdown("Horse Archer Culture Priority", Order = 13, RequireRestart = false, HintText = "Preferred culture for horse archers.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public Dropdown<string> HorseArcherCulturePriority { get; set; } = new Dropdown<string>(new string[] { "Khuzait", "Aserai", "Empire" }, 0);

        [SettingPropertyDropdown("Shock Infantry Culture Priority", Order = 14, RequireRestart = false, HintText = "Preferred culture for shock infantry.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public Dropdown<string> ShockCulturePriority { get; set; } = new Dropdown<string>(new string[] { "Sturgia", "Battania", "Empire", "Aserai", "Vlandia", "Khuzait" }, 0);

        #endregion

        #region 4. Auto Dual Perks

        [SettingPropertyBool("Auto Dual Perks Enabled", Order = 0, RequireRestart = false, HintText = "Automatically unlocks alternative perks whenever a perk is chosen or skill threshold is met.")]
        [SettingPropertyGroup("4. Auto Dual Perks")]
        public bool AutoDualPerksEnabled { get; set; } = true;

        [SettingPropertyBool("Apply To Player", Order = 1, RequireRestart = false, HintText = "Grant dual perks to the main hero.")]
        [SettingPropertyGroup("4. Auto Dual Perks")]
        public bool DualPerksApplyToPlayer { get; set; } = true;

        [SettingPropertyBool("Apply To Companions", Order = 2, RequireRestart = false, HintText = "Grant dual perks to companion heroes and clan members.")]
        [SettingPropertyGroup("4. Auto Dual Perks")]
        public bool DualPerksApplyToCompanions { get; set; } = false;

        #endregion
    }
}
