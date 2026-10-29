using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace BannerlordSmartSuite
{
    [HarmonyPatch(typeof(OrderOfBattleVM), "SetInitialHeroFormations")]
    public static class OrderOfBattlePatch
    {
        public static bool OrderOfBattleRanThisMission { get; set; } = false;

        [HarmonyPostfix]
        public static void Postfix(OrderOfBattleVM __instance)
        {
            try
            {
                OrderOfBattleRanThisMission = true;
                if (!Settings.Instance.ModEnabled || !Settings.Instance.SmartCaptainEnabled || !Settings.Instance.AutoAssignOnDeployment)
                    return;

                RunSmartCaptainAutoAssign(__instance);
            }
            catch (Exception ex)
            {
                TaleWorlds.Library.Debug.Print("[BannerlordSmartSuite] Exception in OrderOfBattlePatch.Postfix: " + ex);
            }
        }

        public static void RunSmartCaptainAutoAssign(OrderOfBattleVM vm)
        {
            if (vm == null) return;

            var formations = new List<OrderOfBattleFormationItemVM>();
            if (vm.FormationsFirstHalf != null)
            {
                foreach (var f in vm.FormationsFirstHalf)
                {
                    if (f != null && f.HasFormation && f.TroopCount > 0)
                        formations.Add(f);
                }
            }
            if (vm.FormationsSecondHalf != null)
            {
                foreach (var f in vm.FormationsSecondHalf)
                {
                    if (f != null && f.HasFormation && f.TroopCount > 0)
                        formations.Add(f);
                }
            }

            if (formations.Count == 0) return;

            var allHeroes = new List<OrderOfBattleHeroItemVM>();
            if (vm.UnassignedHeroes != null)
            {
                foreach (var h in vm.UnassignedHeroes)
                {
                    if (h != null && !allHeroes.Contains(h))
                        allHeroes.Add(h);
                }
            }
            foreach (var f in formations)
            {
                if (f.Captain != null && !allHeroes.Contains(f.Captain))
                    allHeroes.Add(f.Captain);

                if (f.HeroTroops != null)
                {
                    foreach (var h in f.HeroTroops)
                    {
                        if (h != null && !allHeroes.Contains(h))
                            allHeroes.Add(h);
                    }
                }
            }

            // Also check internal _allHeroes field if available
            try
            {
                var allHeroesField = Traverse.Create(vm).Field<List<OrderOfBattleHeroItemVM>>("_allHeroes")?.Value;
                if (allHeroesField != null)
                {
                    foreach (var h in allHeroesField)
                    {
                        if (h != null && !allHeroes.Contains(h))
                            allHeroes.Add(h);
                    }
                }
            }
            catch {}

            if (allHeroes.Count == 0) return;

            var matched = FormationMatcher.MatchOrderOfBattle(allHeroes, formations);
            foreach (var kvp in matched)
            {
                var formationItem = kvp.Key;
                var heroItem = kvp.Value;
                if (formationItem != null && heroItem != null)
                {
                    AssignCaptainSafe(vm, formationItem, heroItem);
                }
            }
        }

        public static void AssignCaptainSafe(OrderOfBattleVM vm, OrderOfBattleFormationItemVM formationItem, OrderOfBattleHeroItemVM heroItem)
        {
            if (vm == null || formationItem == null || heroItem == null) return;

            try
            {
                // If hero was previously assigned to another formation, unassign them first
                if (heroItem.IsLeadingAFormation && heroItem.CurrentAssignedFormationItem != null && heroItem.CurrentAssignedFormationItem != formationItem)
                {
                    heroItem.CurrentAssignedFormationItem.UnassignCaptain();
                }
                else if (heroItem.IsAssignedToAFormation && heroItem.CurrentAssignedFormationItem != null && heroItem.CurrentAssignedFormationItem != formationItem)
                {
                    heroItem.CurrentAssignedFormationItem.RemoveHeroTroop(heroItem);
                }

                // If formation already has another captain, clear them
                if (formationItem.HasCaptain && formationItem.Captain != heroItem)
                {
                    if (formationItem.Captain != null)
                        formationItem.Captain.IsSelected = false;

                    formationItem.UnassignCaptain();
                }

                // Assign the hero as captain
                if (heroItem.Agent != null)
                {
                    try
                    {
                        Traverse.Create(vm).Method("AssignCaptain", heroItem.Agent, formationItem).GetValue();
                    }
                    catch
                    {
                        formationItem.Captain = heroItem;
                    }
                }
                else
                {
                    formationItem.Captain = heroItem;
                }
            }
            catch (Exception ex)
            {
                TaleWorlds.Library.Debug.Print("[BannerlordSmartSuite] Error in AssignCaptainSafe: " + ex);
            }
        }
    }

    [HarmonyPatch(typeof(OrderOfBattleVM), "AfterAutoDeploy")]
    public static class OrderOfBattleAutoDeployPatch
    {
        [HarmonyPostfix]
        public static void Postfix(OrderOfBattleVM __instance)
        {
            try
            {
                if (!Settings.Instance.ModEnabled || !Settings.Instance.SmartCaptainEnabled || !Settings.Instance.AutoAssignOnDeployment)
                    return;

                OrderOfBattlePatch.RunSmartCaptainAutoAssign(__instance);
            }
            catch (Exception ex)
            {
                TaleWorlds.Library.Debug.Print("[BannerlordSmartSuite] Exception in OrderOfBattleAutoDeployPatch.Postfix: " + ex);
            }
        }
    }
}
