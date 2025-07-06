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
using System.Runtime.CompilerServices;

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
            SSOracleBehavior pebblesOracle = null;
            SlugcatStats.Name currentSlug = null;
            Oracle.OracleID oracleID = null;

            _ = new Hook(typeof(RegionGate).GetProperty(nameof(RegionGate.MeetRequirement)).GetGetMethod(), RegionGate_MeetRequirement_get);

            On.Player.ctor += (orig, self, absCreat, world) =>
            {
                orig(self, absCreat, world);
                if (!self.isSlugpup)
                {
                    slugIndex = slugcatToIndex(self.slugcatStats.name);
                    currentSlug = self.slugcatStats.name;
                    
                }
                
            };


            On.Player.UpdateMSC += (orig, self) =>
            {
                orig(self);
                if (self.room.game.session is StoryGameSession && self.slugcatStats.name != MoreSlugcatsEnums.SlugcatStatsName.Slugpup && !self.isSlugpup)
                {
                    if (self.room != null && (self.myRobot == null || self.myRobot.slatedForDeletetion) && self.AI == null && ((self.room.game.session as StoryGameSession).saveState.hasRobo ^ self.slugcatStats.name != MoreSlugcatsEnums.SlugcatStatsName.Artificer) && self.room.game.session is StoryGameSession && self.room.game.FirstAlivePlayer != null && self.room.game.FirstAlivePlayer.realizedCreature != null && self.room.game.FirstAlivePlayer.realizedCreature == self && DroneOptions.usingDrone[slugIndex].Value)
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
                if(self.tiedToObject is Player && DroneOptions.usingDrone[slugIndex].Value)
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
                if(self.tiedToObject is Player && DroneOptions.usingDrone[slugIndex].Value) 
                {
                    self.color = DroneOptions.eyeColors[slugIndex].Value;
                }
                
            };

            On.Oracle.ctor += (orig, self, abstractPhysObj, room) =>
            {
                orig(self, abstractPhysObj, room);
                oracleID = self.ID;
            };

            On.SSOracleBehavior.SeePlayer += (orig, self) =>
            {
                pebblesOracle = self;
                if(currentSlug != MoreSlugcatsEnums.SlugcatStatsName.Artificer)
                {
                    if (self.oracle.ID == Oracle.OracleID.SS && self.action != MoreSlugcatsEnums.SSOracleBehaviorAction.Pebbles_SlumberParty && DroneOptions.usingDrone[slugIndex].Value)
                    {
                        self.SlugcatEnterRoomReaction();
                        self.NewAction(MoreSlugcatsEnums.SSOracleBehaviorAction.Pebbles_SlumberParty);
                        return;
                    }
                }
                orig(self);
            };

            On.SSOracleBehavior.SSSleepoverBehavior.Update += (orig, self) =>
            {
                UnityEngine.Debug.Log(currentSlug);
                if(currentSlug != MoreSlugcatsEnums.SlugcatStatsName.Artificer)
                {
                    var physicalObjects = self.oracle.room.physicalObjects;

                    foreach (var layer in physicalObjects)
                    {
                        foreach (var physicalObject in layer)
                        {
                            if (physicalObject.grabbedBy.Count == 0 && physicalObject is DataPearl && physicalObject is not PebblesPearl && !pebblesOracle.talkedAboutThisSession.Contains((physicalObject as DataPearl).abstractPhysicalObject.ID))
                            {
                                UnityEngine.Debug.Log("YEAH");
                                var pearl = physicalObject as DataPearl;
                                pebblesOracle.inspectPearl = physicalObject as DataPearl;
                                //bruh.conversation.currentSaveFile = MoreSlugcatsEnums.SlugcatStatsName.Artificer;
                                pebblesOracle.StartItemConversation(physicalObject as DataPearl);
                                //bruh.readDataPearlOrbits.Add(pearl.AbstractPearl);
                                pebblesOracle.talkedAboutThisSession.Add(pearl.abstractPhysicalObject.ID);
                            }
                        }
                    }
                }
                
                orig(self);

            };

            On.Conversation.LoadEventsFromFile_int_Name_bool_int += (orig, self, whatever, slug, a, h) =>
            {
                if(currentSlug != MoreSlugcatsEnums.SlugcatStatsName.Artificer)
                {
                    if (DroneOptions.usingDrone[slugIndex].Value && oracleID == Oracle.OracleID.SS)
                    {
                        slug = MoreSlugcatsEnums.SlugcatStatsName.Artificer;
                    }
                }
                orig(self, whatever, slug, a, h);
                
            };

            On.OracleBehavior.AlreadyDiscussedItemString += (orig, self, pearl) => 
            {
                if(currentSlug != MoreSlugcatsEnums.SlugcatStatsName.Artificer)
                {
                    if (oracleID == Oracle.OracleID.SS && pearl)
                    {
                        return "Ah, something to read?";
                    }
                }
                
                return orig(self, pearl);
            };


            IL.MoreSlugcats.AncientBot.InitiateSprites += AncientBot_InitiateSprites;
            IL.MoreSlugcats.AncientBot.DrawSprites += AncientBot_DrawSprites;
            //IL.SSOracleBehavior.Update += SSOracleBehavior_Update;


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

        static bool usingDrone()
        {
            //return DroneOptions.usingDrone[slugIndex].Value;
            return true;
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

        private static void SSOracleBehavior_Update(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                for (int i = 0; i < 2; i++)
                {
                    c.GotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<SSOracleBehavior>(nameof(SSOracleBehavior.currSubBehavior)),
                    x => x.MatchIsinst<SSOracleBehavior.ThrowOutBehavior>()
                    );
                }
                c.MoveAfterLabels();
                var jump4 = c.Next;
                c.Index -= 6;
                c.EmitDelegate(usingDrone);
                c.Emit(OpCodes.Brtrue, jump4);
                //c.Emit(OpCodes.Nop);
                UnityEngine.Debug.Log(il);

            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("HELPPP");
                UnityEngine.Debug.Log(e);
            }
        }

    }
}
