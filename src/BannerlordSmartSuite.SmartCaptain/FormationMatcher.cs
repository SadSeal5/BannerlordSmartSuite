using BannerlordSmartSuite.Core.Config;
using BannerlordSmartSuite.Core.Safe;
using BannerlordSmartSuite.Core.Logging;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace BannerlordSmartSuite.SmartCaptain
{
    public static class FormationMatcher
    {
        public static Dictionary<OrderOfBattleFormationItemVM, OrderOfBattleHeroItemVM> MatchOrderOfBattle(
            List<OrderOfBattleHeroItemVM> allHeroes,
            List<OrderOfBattleFormationItemVM> allFormations)
        {
            var result = new Dictionary<OrderOfBattleFormationItemVM, OrderOfBattleHeroItemVM>();
            if (allHeroes == null || allFormations == null || allHeroes.Count == 0 || allFormations.Count == 0)
                return result;

            bool optimizeMainHero = Settings.Instance.OptimizeMainHero;

            var targetFormations = new List<OrderOfBattleFormationItemVM>();
            var availableHeroes = new List<OrderOfBattleHeroItemVM>();

            // If not optimizing main hero, protect any formation currently led by MainHero
            var lockedFormations = new HashSet<OrderOfBattleFormationItemVM>();
            var lockedHeroes = new HashSet<OrderOfBattleHeroItemVM>();

            if (!optimizeMainHero)
            {
                foreach (var formation in allFormations)
                {
                    if (formation != null && formation.Captain != null && formation.Captain.IsMainHero)
                    {
                        lockedFormations.Add(formation);
                        lockedHeroes.Add(formation.Captain);
                    }
                }
            }

            foreach (var formation in allFormations)
            {
                if (formation != null && formation.HasFormation && formation.TroopCount > 0 && !lockedFormations.Contains(formation))
                {
                    targetFormations.Add(formation);
                }
            }

            foreach (var heroItem in allHeroes)
            {
                if (heroItem == null || lockedHeroes.Contains(heroItem))
                    continue;

                if (!optimizeMainHero && heroItem.IsMainHero)
                    continue;

                if (!availableHeroes.Contains(heroItem))
                    availableHeroes.Add(heroItem);
            }

            int nFormations = targetFormations.Count;
            int nHeroes = availableHeroes.Count;
            if (nFormations == 0 || nHeroes == 0)
                return result;

            float[,] scores = new float[nFormations, nHeroes];
            for (int f = 0; f < nFormations; f++)
            {
                var formItem = targetFormations[f];
                FormationClass fClass = formItem.Formation != null
                    ? formItem.Formation.PhysicalClass
                    : FormationClass.Infantry;

                for (int h = 0; h < nHeroes; h++)
                {
                    var heroItem = availableHeroes[h];
                    Hero hero = (heroItem.Agent?.Character as CharacterObject)?.HeroObject;
                    scores[f, h] = CaptainScoringEngine.ScoreHeroOrAgent(hero, heroItem.Agent, fClass);
                }
            }

            int[] assignment = FindOptimalAssignment(scores, nFormations, nHeroes);
            for (int f = 0; f < nFormations; f++)
            {
                int heroIdx = assignment[f];
                if (heroIdx >= 0 && heroIdx < nHeroes && scores[f, heroIdx] >= 0f)
                {
                    result[targetFormations[f]] = availableHeroes[heroIdx];
                }
            }

            return result;
        }

        public static Dictionary<Formation, Agent> MatchMission(
            List<Agent> availableAgents,
            List<Formation> targetFormations)
        {
            var result = new Dictionary<Formation, Agent>();
            if (availableAgents == null || targetFormations == null || availableAgents.Count == 0 || targetFormations.Count == 0)
                return result;

            bool optimizeMainHero = Settings.Instance.OptimizeMainHero;

            var filteredFormations = new List<Formation>();
            var filteredAgents = new List<Agent>();

            var lockedFormations = new HashSet<Formation>();
            var lockedAgents = new HashSet<Agent>();

            if (!optimizeMainHero)
            {
                foreach (var formation in targetFormations)
                {
                    if (formation != null && formation.Captain != null && formation.Captain.IsMainAgent)
                    {
                        lockedFormations.Add(formation);
                        lockedAgents.Add(formation.Captain);
                    }
                }
            }

            foreach (var formation in targetFormations)
            {
                if (formation != null && formation.CountOfUnits > 0 && !lockedFormations.Contains(formation))
                {
                    filteredFormations.Add(formation);
                }
            }

            foreach (var agent in availableAgents)
            {
                if (agent == null || !agent.IsActive() || !agent.IsHero || lockedAgents.Contains(agent))
                    continue;

                if (!optimizeMainHero && agent.IsMainAgent)
                    continue;

                if (!filteredAgents.Contains(agent))
                    filteredAgents.Add(agent);
            }

            int nFormations = filteredFormations.Count;
            int nAgents = filteredAgents.Count;
            if (nFormations == 0 || nAgents == 0)
                return result;

            float[,] scores = new float[nFormations, nAgents];
            for (int f = 0; f < nFormations; f++)
            {
                var form = filteredFormations[f];
                FormationClass fClass = form.PhysicalClass;

                for (int a = 0; a < nAgents; a++)
                {
                    var agent = filteredAgents[a];
                    Hero hero = (agent.Character as CharacterObject)?.HeroObject;
                    scores[f, a] = CaptainScoringEngine.ScoreHeroOrAgent(hero, agent, fClass);
                }
            }

            int[] assignment = FindOptimalAssignment(scores, nFormations, nAgents);
            for (int f = 0; f < nFormations; f++)
            {
                int agentIdx = assignment[f];
                if (agentIdx >= 0 && agentIdx < nAgents && scores[f, agentIdx] >= 0f)
                {
                    result[filteredFormations[f]] = filteredAgents[agentIdx];
                }
            }

            return result;
        }

        public static int[] FindOptimalAssignment(float[,] scoreMatrix, int nRows, int nCols)
        {
            if (nRows == 0 || nCols == 0) return new int[nRows];
            int dim = Math.Max(nRows, nCols);
            float maxVal = 0f;
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    if (scoreMatrix[i, j] > maxVal)
                        maxVal = scoreMatrix[i, j];
                }
            }

            float[,] cost = new float[dim + 1, dim + 1];
            for (int i = 1; i <= dim; i++)
            {
                for (int j = 1; j <= dim; j++)
                {
                    if (i <= nRows && j <= nCols)
                    {
                        float s = scoreMatrix[i - 1, j - 1];
                        cost[i, j] = s > 0 ? (maxVal - s) : (maxVal + 100000f);
                    }
                    else
                    {
                        cost[i, j] = maxVal + 100000f;
                    }
                }
            }

            float[] u = new float[dim + 1];
            float[] v = new float[dim + 1];
            int[] p = new int[dim + 1];
            int[] way = new int[dim + 1];

            for (int i = 1; i <= dim; i++)
            {
                p[0] = i;
                int j0 = 0;
                float[] minv = new float[dim + 1];
                bool[] used = new bool[dim + 1];
                for (int j = 1; j <= dim; j++)
                {
                    minv[j] = float.MaxValue;
                    used[j] = false;
                }

                do
                {
                    used[j0] = true;
                    int i0 = p[j0];
                    float delta = float.MaxValue;
                    int j1 = 0;

                    for (int j = 1; j <= dim; j++)
                    {
                        if (!used[j])
                        {
                            float cur = cost[i0, j] - u[i0] - v[j];
                            if (cur < minv[j])
                            {
                                minv[j] = cur;
                                way[j] = j0;
                            }
                            if (minv[j] < delta)
                            {
                                delta = minv[j];
                                j1 = j;
                            }
                        }
                    }

                    for (int j = 0; j <= dim; j++)
                    {
                        if (used[j])
                        {
                            u[p[j]] += delta;
                            v[j] -= delta;
                        }
                        else
                        {
                            minv[j] -= delta;
                        }
                    }

                    j0 = j1;
                } while (p[j0] != 0);

                do
                {
                    int j1 = way[j0];
                    p[j0] = p[j1];
                    j0 = j1;
                } while (j0 != 0);
            }

            int[] result = new int[nRows];
            for (int j = 1; j <= dim; j++)
            {
                int row = p[j] - 1;
                int col = j - 1;
                if (row >= 0 && row < nRows)
                {
                    if (col < nCols && scoreMatrix[row, col] >= 0f)
                    {
                        result[row] = col;
                    }
                    else
                    {
                        result[row] = -1;
                    }
                }
            }

            return result;
        }
    }
}
