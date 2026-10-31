using System;
using MCM.Abstractions.Base.Global;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Common;

namespace BannerlordSmartSuite.Core.Config
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

        [SettingPropertyBool("Debug Logging", Order = 1, RequireRestart = false, HintText = "Enable detailed logging to Bannerlord debug stream.")]
        [SettingPropertyGroup("General")]
        public bool DebugLogging { get; set; } = false;

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

        [SettingPropertyBool("Dismount Player If Infantry Captain", Order = 5, RequireRestart = false, HintText = "Allow forcibly dismounting the player if player character is captain of an infantry formation.")]
        [SettingPropertyGroup("1. Smart Captain")]
        public bool DismountPlayerIfInfantryCaptain { get; set; } = false;

        [SettingPropertyBool("Exclude Civilian And Arena Missions", Order = 6, RequireRestart = false, HintText = "Skip captain assignment in tournaments, arena practice, tavern brawls, and hideout raids.")]
        [SettingPropertyGroup("1. Smart Captain")]
        public bool ExcludeCivilianAndArenaMissions { get; set; } = true;

        [SettingPropertyBool("Weight Captain Survivability", Order = 7, RequireRestart = false, HintText = "Factor hero armor and durability into captain score so resilient leaders are favored.")]
        [SettingPropertyGroup("1. Smart Captain")]
        public bool WeightCaptainSurvivability { get; set; } = true;

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

        [SettingPropertyDropdown("Default Daily Project", Order = 5, RequireRestart = false, HintText = "Baseline daily project when settlement is healthy and has no crises.")]
        [SettingPropertyGroup("2. Auto Settlement Builder")]
        public Dropdown<string> DefaultDailyProject { get; set; } = new Dropdown<string>(new string[] { "Housing", "Militia", "Irrigation", "Festivals", "KeepCurrent" }, 0);

        [SettingPropertyInteger("Max Building Queue Depth", 1, 3, "0", Order = 6, RequireRestart = false, HintText = "Maximum number of buildings allowed in the construction queue.")]
        [SettingPropertyGroup("2. Auto Settlement Builder")]
        public int MaxBuildingQueueDepth { get; set; } = 1;

        [SettingPropertyBool("Emergency Rebellion Queue Pause", Order = 7, RequireRestart = false, HintText = "Automatically pause construction queues when loyalty drops below 25 to allow Festivals and Games to prevent rebellion.")]
        [SettingPropertyGroup("2. Auto Settlement Builder")]
        public bool EmergencyRebellionQueuePause { get; set; } = true;

        [SettingPropertyDropdown("Castle Default Daily Project", Order = 8, RequireRestart = false, HintText = "Default daily project for castles when healthy.")]
        [SettingPropertyGroup("2. Auto Settlement Builder")]
        public Dropdown<string> CastleDefaultProject { get; set; } = new Dropdown<string>(new string[] { "Militia", "Housing", "Irrigation", "KeepCurrent" }, 0);

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

        [SettingPropertyBool("Allow Bandit Upgrades Without Perk", Order = 15, RequireRestart = false, HintText = "Allow upgrading bandit/outlaw troops without the Veteran's Respect Leadership perk.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public bool AllowBanditUpgradesWithoutPerk { get; set; } = false;

        [SettingPropertyBool("Consume Cheapest Horses First", Order = 16, RequireRestart = false, HintText = "Sort inventory mounts by value and consume lowest-value horses first when upgrading cavalry.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public bool ConsumeCheapestHorsesFirst { get; set; } = true;

        [SettingPropertyInteger("Max Troop Tier", 2, 6, "0", Order = 17, RequireRestart = false, HintText = "Maximum troop tier for automated upgrades to prevent excessive party wage inflation.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public int MaxTroopTier { get; set; } = 6;

        [SettingPropertyBool("Upgrade Clan Parties", Order = 18, RequireRestart = false, HintText = "Automatically upgrade troops in other clan member parties.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public bool UpgradeClanParties { get; set; } = false;

        [SettingPropertyBool("Upgrade Garrisons", Order = 19, RequireRestart = false, HintText = "Automatically upgrade troops in player-owned settlement garrisons.")]
        [SettingPropertyGroup("3. Smart Troop Upgrade")]
        public bool UpgradeGarrisons { get; set; } = false;

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

        [SettingPropertyDropdown("Perk Grant Mode", Order = 3, RequireRestart = false, HintText = "Paired Choice grants the counterpart when a perk is chosen. Full Auto grants all perks matching skill requirement.")]
        [SettingPropertyGroup("4. Auto Dual Perks")]
        public Dropdown<string> PerkGrantMode { get; set; } = new Dropdown<string>(new string[] { "Paired Choice (When Picked)", "Full Auto Unlock (By Skill Level)" }, 0);

        [SettingPropertyBool("Perk Notifications Enabled", Order = 4, RequireRestart = false, HintText = "Show subtle notification when dual perks are granted.")]
        [SettingPropertyGroup("4. Auto Dual Perks")]
        public bool PerkNotificationsEnabled { get; set; } = false;

        #endregion
    }
}
