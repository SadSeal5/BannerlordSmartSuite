using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BannerlordSmartSuite.Core.Safe
{
    public static class CampaignSafe
    {
        public static Hero MainHero
        {
            get
            {
                try
                {
                    if (Game.Current == null) return null;
                    return CharacterObject.PlayerCharacter?.HeroObject;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static Clan PlayerClan
        {
            get
            {
                try
                {
                    if (Campaign.Current == null) return null;
                    return Clan.PlayerClan;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static bool IsPlayer(Hero hero)
        {
            if (hero == null) return false;
            Hero main = MainHero;
            return main != null && hero == main;
        }

        public static bool IsCompanionOrClan(Hero hero)
        {
            if (hero == null || IsPlayer(hero)) return false;
            Clan playerClan = PlayerClan;
            if (playerClan == null) return false;
            return hero.Clan == playerClan || hero.CompanionOf == playerClan;
        }
    }
}
