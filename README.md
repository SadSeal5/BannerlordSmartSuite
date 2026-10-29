# Bannerlord Smart Suite

An all-in-one consolidated quality-of-life suite for **Mount & Blade II: Bannerlord**.

Bannerlord Smart Suite combines four indispensable automation and optimization mods into a single, cohesive, high-performance module configurable in-game via **MCM v5 (Mod Configuration Menu)**:

1. **Smart Captain**: Automated optimal captain assignments for formations during deployment and mid-battle succession if a commander falls.
2. **Auto Settlement Builder**: Autonomous settlement construction queue management prioritizing urgent needs (Loyalty, Food, Security, Fortifications).
3. **Smart Troop Upgrade**: Intelligent army composition-based troop upgrading respecting party quotas, gold/mount safeguards, and culture preferences.
4. **Auto Dual Perks**: Automatically grants both branches of perk choices for the player and optionally companion heroes upon reaching skill thresholds.

---

## Features

### 1. Smart Captain
- Evaluates combat skills, command skills (Tactics/Leadership), equipment (mounts, ranged weapons), and captain perks.
- Uses the Hungarian Assignment Algorithm to solve optimal global captain-to-formation assignments.
- Supports battle deployment (`OrderOfBattle`) and mission start auto-assignment.
- Mid-battle succession: when a captain falls, the best qualified living hero assumes command.
- Option to dismount mounted heroes assigned to foot infantry formations.

### 2. Auto Settlement Builder
- Monitors all player-owned fiefs (towns and castles).
- Analyzes crises (loyalty dips, food shortages, low security) and queues the most vital building project automatically.
- Adjusts continuous daily projects (e.g. Festival and Games during low loyalty, Irrigation during food deficits).
- Clean save-game compatibility (no serialized state).

### 3. Smart Troop Upgrade
- Configurable target army composition percentages (Shield Infantry, Shock Infantry, Ranged, Melee Cavalry, Horse Archers).
- Upgrades eligible ready troops to maintain ideal troop ratios.
- Gold reserve buffer: prevents auto-upgrades from draining funds below your chosen gold threshold.
- Mount reserve buffer: protects inventory mounts/war horses before upgrading to mounted troops.
- Respects locked troops and locked items in the party screen.
- Noble troop prioritization and cultural preference dropdowns.
- Manual hotkey: `Ctrl + Shift + U` triggers an instant upgrade pass.

### 4. Auto Dual Perks
- Unlocks both perk options on perk trees when skill levels are achieved or when a perk is selected.
- Configurable targeting: Player hero (default: enabled) and/or Clan Companions (default: disabled).
- Clean visual patches for perk trees.

---

## Requirements
- Mount & Blade II: Bannerlord (v1.2.x - v1.3.x+)
- Harmony (v2.2+)
- MCM v5 (Mod Configuration Menu)

---

## Installation
Extract `BannerlordSmartSuite` into your Bannerlord `Modules/` directory:
`Mount & Blade II Bannerlord\Modules\BannerlordSmartSuite`

Ensure it is activated in the Bannerlord launcher after Harmony and MCM.

---

## License
MIT License. Created by SadSeal5.
