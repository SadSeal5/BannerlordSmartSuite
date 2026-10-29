using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BannerlordSmartSuite
{
    public class SmartCaptainMissionBehavior : MissionBehavior
    {
        private bool _initialAutoAssignDone = false;

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public override void OnDeploymentFinished()
        {
            base.OnDeploymentFinished();
            try
            {
                CheckAndAssignCaptains();
                ApplyDismountInfantryCaptains();
            }
            catch (Exception ex)
            {
                Debug.Print("[BannerlordSmartSuite] Exception in OnDeploymentFinished: " + ex);
            }
        }

        public override void AfterStart()
        {
            base.AfterStart();
            try
            {
                CheckAndAssignCaptains();
                ApplyDismountInfantryCaptains();
            }
            catch (Exception ex)
            {
                Debug.Print("[BannerlordSmartSuite] Exception in AfterStart: " + ex);
            }
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
        {
            base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);

            try
            {
                if (!Settings.Instance.ModEnabled || !Settings.Instance.SmartCaptainEnabled || !Settings.Instance.MidBattleSuccession)
                    return;

                if (affectedAgent == null || !affectedAgent.IsHero)
                    return;

                Formation formation = affectedAgent.Formation;
                if (formation == null || formation.Captain != affectedAgent)
                    return;

                // The current captain of this formation fell in battle!
                Agent nextCaptain = null;
                float bestScore = 0f;
                FormationClass fClass = formation.PhysicalClass;

                // 1. Search for next best living hero within the same formation
                formation.ApplyActionOnEachUnit(agent =>
                {
                    if (agent != null && agent.IsActive() && agent.IsHero && agent != affectedAgent)
                    {
                        if (!Settings.Instance.OptimizeMainHero && agent.IsMainAgent)
                            return;

                        float score = CaptainScoringEngine.ScoreAgent(agent, fClass);
                        if (score > bestScore)
                        {
                            bestScore = score;
                            nextCaptain = agent;
                        }
                    }
                });

                // 2. If no living hero is in the formation, check other heroes on the team not currently leading a formation
                if (nextCaptain == null && affectedAgent.Team != null)
                {
                    foreach (Agent teamAgent in affectedAgent.Team.ActiveAgents)
                    {
                        if (teamAgent != null && teamAgent.IsActive() && teamAgent.IsHero && teamAgent != affectedAgent)
                        {
                            if (!Settings.Instance.OptimizeMainHero && teamAgent.IsMainAgent)
                                continue;

                            if (teamAgent.Formation != null && teamAgent.Formation.Captain == teamAgent)
                                continue;

                            float score = CaptainScoringEngine.ScoreAgent(teamAgent, fClass);
                            if (score > bestScore)
                            {
                                bestScore = score;
                                nextCaptain = teamAgent;
                            }
                        }
                    }
                }

                // Assign the successor
                if (nextCaptain != null)
                {
                    formation.Captain = nextCaptain;
                    if (nextCaptain.Formation != formation)
                    {
                        nextCaptain.Formation = formation;
                    }

                    // If infantry and setting requires dismounting, ensure successor is dismounted
                    if (Settings.Instance.DismountMountedInfantryCaptains && fClass == FormationClass.Infantry && nextCaptain.HasMount)
                    {
                        try
                        {
                            var mount = nextCaptain.MountAgent;
                            if (mount != null)
                            {
                                nextCaptain.Mount(mount);
                                mount.FadeOut(false, false);
                            }
                        }
                        catch {}
                    }

                    try
                    {
                        InformationManager.DisplayMessage(new InformationMessage(
                            $"{affectedAgent.Name} fell! {nextCaptain.Name} has assumed command of the {formation.PhysicalClass} formation.",
                            Color.FromUint(0xFFD700FF)
                        ));
                    }
                    catch {}
                }
            }
            catch (Exception ex)
            {
                Debug.Print("[BannerlordSmartSuite] Exception in OnAgentRemoved: " + ex);
            }
        }

        protected override void OnEndMission()
        {
            base.OnEndMission();
            OrderOfBattlePatch.OrderOfBattleRanThisMission = false;
            _initialAutoAssignDone = false;
        }

        private void CheckAndAssignCaptains()
        {
            if (_initialAutoAssignDone)
                return;

            _initialAutoAssignDone = true;

            if (!Settings.Instance.ModEnabled || !Settings.Instance.SmartCaptainEnabled || !Settings.Instance.AutoAssignOnDeployment)
                return;

            // If Order of Battle ran, captains were already assigned
            if (OrderOfBattlePatch.OrderOfBattleRanThisMission)
                return;

            Team playerTeam = Mission?.PlayerTeam;
            if (playerTeam == null)
                return;

            var targetFormations = new List<Formation>();
            foreach (var formation in playerTeam.FormationsIncludingEmpty)
            {
                if (formation != null && formation.CountOfUnits > 0 && formation.Captain == null)
                {
                    targetFormations.Add(formation);
                }
            }

            if (targetFormations.Count == 0)
                return;

            var candidateAgents = new List<Agent>();
            foreach (var agent in playerTeam.ActiveAgents)
            {
                if (agent != null && agent.IsActive() && agent.IsHero)
                {
                    if (agent.Formation != null && agent.Formation.Captain == agent)
                        continue;

                    candidateAgents.Add(agent);
                }
            }

            if (candidateAgents.Count == 0)
                return;

            var matched = FormationMatcher.MatchMission(candidateAgents, targetFormations);
            foreach (var kvp in matched)
            {
                Formation form = kvp.Key;
                Agent captain = kvp.Value;
                if (form != null && captain != null)
                {
                    form.Captain = captain;
                    if (captain.Formation != form)
                    {
                        captain.Formation = form;
                    }
                }
            }
        }

        private void ApplyDismountInfantryCaptains()
        {
            if (!Settings.Instance.ModEnabled || !Settings.Instance.SmartCaptainEnabled || !Settings.Instance.DismountMountedInfantryCaptains)
                return;

            Team playerTeam = Mission?.PlayerTeam;
            if (playerTeam == null)
                return;

            foreach (var formation in playerTeam.FormationsIncludingEmpty)
            {
                if (formation != null && formation.CountOfUnits > 0 && formation.PhysicalClass == FormationClass.Infantry)
                {
                    Agent captain = formation.Captain;
                    if (captain != null && captain.IsActive() && captain.HasMount)
                    {
                        try
                        {
                            var mount = captain.MountAgent;
                            if (mount != null)
                            {
                                captain.Mount(mount);
                                mount.FadeOut(false, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Print("[BannerlordSmartSuite] Exception dismounting infantry captain: " + ex);
                        }
                    }
                }
            }
        }
    }
}
