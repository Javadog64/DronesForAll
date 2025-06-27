using MonoMod.Cil;
using Mono.Cecil.Cil;
using MoreSlugcats;
using System;
using UnityEngine;
using Rewired;
using Watcher;
using System.Collections.Generic;
using MonoMod.RuntimeDetour;
using System.Runtime.InteropServices.WindowsRuntime;

namespace DronesForAll
{
    internal class Hooks
    {
        private static int slugcatToIndex(SlugcatStats.Name slug)
        {
            
            if(slug == SlugcatStats.Name.Yellow)
            {
                return 1;
            }
            else if(slug == SlugcatStats.Name.White)
            {
                return 2;
            }
            else if (slug == SlugcatStats.Name.Red)
            {
                return 3;
            }
            else if (slug == MoreSlugcatsEnums.SlugcatStatsName.Gourmand)
            {
                return 4;
            }
            else if (slug == MoreSlugcatsEnums.SlugcatStatsName.Artificer)
            {
                return 5;
            }
            else if (slug == MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
            {
                return 6;
            }
            else if (slug == MoreSlugcatsEnums.SlugcatStatsName.Spear)
            {
                return 7;
            }
            else if (slug == MoreSlugcatsEnums.SlugcatStatsName.Saint)
            {
                return 8;
            }
            else if (slug == MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel)
            {
                return 9;
            }
            else if (ModManager.Watcher && slug == WatcherEnums.SlugcatStatsName.Watcher)
            {
                return 10;
            }
            else
            {
                return 11;
            }
        }

        public static int slugIndex = 11;

        public static void Apply()
        {

            _ = new Hook(typeof(RegionGate).GetProperty(nameof(RegionGate.MeetRequirement)).GetGetMethod(), RegionGate_MeetRequirement_get);

            On.Player.ctor += (orig, self, absCreat, world) =>
            {
                orig(self, absCreat, world);
                if (!self.isSlugpup)
                {
                    slugIndex = slugcatToIndex(self.slugcatStats.name);
                }
                
            };


            On.Player.UpdateMSC += (orig, self) =>
            {
                orig(self);
                if (self.room.game.session is StoryGameSession && self.slugcatStats.name != MoreSlugcatsEnums.SlugcatStatsName.Slugpup && !self.isSlugpup)
                {
                    if (self.room != null && (self.myRobot == null || self.myRobot.slatedForDeletetion) && self.AI == null && ((self.room.game.session as StoryGameSession).saveState.hasRobo ^ self.slugcatStats.name != MoreSlugcatsEnums.SlugcatStatsName.Artificer) && self.room.game.session is StoryGameSession && self.room.game.FirstAlivePlayer != null && self.room.game.FirstAlivePlayer.realizedCreature != null && self.room.game.FirstAlivePlayer.realizedCreature == self)
                    //if (self.room != null && !self.room.game.wasAnArtificerDream && self.room.game.session is StoryGameSession && ((self.AI == null && (self.room.game.session as StoryGameSession).saveState.hasRobo) || (self.AI != null && (self.playerState as PlayerNPCState).Drone)) && (self.myRobot == null || self.myRobot.slatedForDeletetion) && (!ModManager.CoopAvailable || (self.room.game.FirstAlivePlayer != null && self.room.game.FirstAlivePlayer.realizedCreature != null && self.room.game.FirstAlivePlayer.realizedCreature == self)))
                    {
                        self.myRobot = new AncientBot(self.mainBodyChunk.pos, DroneOptions.eyeColors[slugIndex].Value, self, true);
                        self.room.AddObject(self.myRobot);
                    }
                }
                    
            };

            On.MoreSlugcats.AncientBot.ApplyPalette += (orig, self, sLeaser, rcam, pal) =>
            {
                orig(self, sLeaser, rcam, pal);
                if(self.tiedToObject is Player)
                {
                    for (int j = self.BodyIndex; j < self.LightBaseIndex; j++)
                    {
                        if (j == self.LeftAntIndex + 1 || j == self.RightAntIndex + 1)
                        {
                            sLeaser.sprites[j].color = DroneOptions.antColors[slugIndex].Value;
                        }
                        else if (j == self.BodyIndex)
                        {
                            sLeaser.sprites[j].color = DroneOptions.bottomColors[slugIndex].Value;
                        }
                        else
                        {
                            sLeaser.sprites[j].color = DroneOptions.bodyColors[slugIndex].Value;
                        }
                    }
                }
                
            };

            On.MoreSlugcats.AncientBot.ctor += (orig, self, pos, color, rcam, pal) =>
            {
                orig(self, pos, color, rcam, pal);
                if(self.tiedToObject is Player) 
                {
                    self.color = DroneOptions.eyeColors[slugIndex].Value;
                }
                
            };

            On.SSOracleBehavior.Update += (orig, self, eu) =>
            {
                if(self.oracle.ID == Oracle.OracleID.SS)
                {
                    if (DroneOptions.noPebblesKill.Value && DroneOptions.usingDrone[slugcatToIndex(self.oracle.room.game.StoryCharacter)].Value && (self.action == SSOracleBehavior.Action.ThrowOut_ThrowOut || self.action == SSOracleBehavior.Action.ThrowOut_Polite_ThrowOut || self.action == SSOracleBehavior.Action.ThrowOut_SecondThrowOut || self.action == SSOracleBehavior.Action.ThrowOut_KillOnSight))
                    {
                        self.NewAction(MoreSlugcatsEnums.SSOracleBehaviorAction.Pebbles_SlumberParty);
                    }
                }
                
                orig(self, eu);
            };


            IL.MoreSlugcats.AncientBot.InitiateSprites += AncientBot_InitiateSprites;
            IL.MoreSlugcats.AncientBot.DrawSprites += AncientBot_DrawSprites;


        }

        private static bool RegionGate_MeetRequirement_get(Func<RegionGate, bool> orig, RegionGate self) 
        {
            if (ModManager.MSC && self.karmaRequirements[(!self.letThroughDir) ? 1 : 0] == MoreSlugcatsEnums.GateRequirement.RoboLock && DroneOptions.usingDrone[slugcatToIndex(self.ListOfPlayersInZone()[0].slugcatStats.name)].Value)
            {
                if(self.room.abstractRoom.name == "GATE_UW_LC" && DroneOptions.openMetro.Value)
                {
                    return true;
                }
                if (self.room.abstractRoom.name == "GATE_SL_MS" && DroneOptions.openBitter.Value)
                {
                    return true;
                }
            }

            return orig(self);

        }

        static bool wantToUseKingDrone()
        {
            return DroneOptions.kingDroneVars[slugIndex].Value;
        }

        private static void AncientBot_InitiateSprites(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<MoreSlugcats.AncientBot>(nameof(MoreSlugcats.AncientBot.tiedToObject)),
                    x => x.MatchIsinst<Scavenger>()
                    );
                c.MoveAfterLabels();
                //c.Index++;
                var jump = c.Next;
                //c.Emit(OpCodes.Nop);
                c.Index -= 6;
                c.EmitDelegate(wantToUseKingDrone);
                c.Emit(OpCodes.Brtrue, jump);
                //UnityEngine.Debug.Log(il);
            }
            catch (Exception e) {
                UnityEngine.Debug.Log(e);
            }
        }

        private static void AncientBot_DrawSprites(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<MoreSlugcats.AncientBot>(nameof(MoreSlugcats.AncientBot.tiedToObject)),
                    x => x.MatchIsinst<Scavenger>()
                    );
                c.MoveAfterLabels();
                var jump1 = c.Next;
                //c.Emit(OpCodes.Nop);
                c.Index -= 7;
                c.EmitDelegate(wantToUseKingDrone);
                c.Emit(OpCodes.Brtrue, jump1);

                for (int i = 0; i < 2; i++)
                {
                    c.GotoNext(
                        MoveType.After,
                        x => x.MatchLdarg(0),
                        x => x.MatchLdfld<MoreSlugcats.AncientBot>(nameof(MoreSlugcats.AncientBot.tiedToObject)),
                        x => x.MatchIsinst<Scavenger>()
                    );
                }
                c.MoveAfterLabels();
                var jump2 = c.Next;
                c.Index -= 6;
                //c.Emit(OpCodes.Nop);
                c.EmitDelegate(wantToUseKingDrone);
                c.Emit(OpCodes.Brtrue, jump2);

                for (int i = 0; i < 2; i++)
                {
                    c.GotoNext(
                        MoveType.After,
                        x => x.MatchLdarg(0),
                        x => x.MatchLdfld<MoreSlugcats.AncientBot>(nameof(MoreSlugcats.AncientBot.tiedToObject)),
                        x => x.MatchIsinst<Scavenger>()
                    );
                }
                c.MoveAfterLabels();
                var jump3 = c.Next;
                c.Index -= 5;
                //c.Emit(OpCodes.Nop);
                c.EmitDelegate(wantToUseKingDrone);
                c.Emit(OpCodes.Brtrue, jump3);


                //UnityEngine.Debug.Log(il);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
        }



    }
}
