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
        private static SSOracleBehavior pebblesOracle = null;
        private static SlugcatStats.Name currentSlug = null;
        private static Oracle.OracleID oracleID = null;


        public static void Apply()
        {
            

            _ = new Hook(typeof(RegionGate).GetProperty(nameof(RegionGate.MeetRequirement)).GetGetMethod(), RegionGate_MeetRequirement_get);


            On.Player.ctor += Player_ctor;
            On.Player.UpdateMSC += Player_UpdateMSC;
            On.MoreSlugcats.AncientBot.ApplyPalette += MoreSlugcats_AncientBot_ApplyPalette;
            On.MoreSlugcats.AncientBot.ctor += MoreSlugcats_AncientBot_ctor;
            On.Oracle.ctor += Oracle_ctor;
            On.SSOracleBehavior.Update += SSOracleBehavior_Update;
            On.SSOracleBehavior.SeePlayer += SSOracleBehavior_SeePlayer;
            On.SSOracleBehavior.SSSleepoverBehavior.Update += SSOracleBehavior_SSSleepoverBehavior_Update;
            On.Conversation.LoadEventsFromFile_int_Name_bool_int += Conversation_LoadEventsFromFile_int_Name_bool_int;
            On.OracleBehavior.AlreadyDiscussedItemString += OracleBehavior_AlreadyDiscussedItemString;
            IL.MoreSlugcats.AncientBot.InitiateSprites += AncientBot_InitiateSprites;
            IL.MoreSlugcats.AncientBot.DrawSprites += AncientBot_DrawSprites;
            //IL.SSOracleBehavior.Update += SSOracleBehavior_Update;


        }

        private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature absCreat, World world)
        {
            orig(self, absCreat, world);
            if (!self.isSlugpup)
            {
                slugIndex = slugcatToIndex(self.slugcatStats.name);
                currentSlug = self.slugcatStats.name;

            }
        }


        private static void Player_UpdateMSC(On.Player.orig_UpdateMSC orig, Player self)
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
        }


        private static void MoreSlugcats_AncientBot_ApplyPalette(On.MoreSlugcats.AncientBot.orig_ApplyPalette orig, MoreSlugcats.AncientBot self, RoomCamera.SpriteLeaser sLeaser,RoomCamera rcam, RoomPalette pal)
        {
            orig(self, sLeaser, rcam, pal);
            if (self.tiedToObject is Player && DroneOptions.usingDrone[slugIndex].Value)
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
        }


        private static void MoreSlugcats_AncientBot_ctor(On.MoreSlugcats.AncientBot.orig_ctor orig, MoreSlugcats.AncientBot self, UnityEngine.Vector2 pos, UnityEngine.Color color, Creature rcam, bool pal)
        {
            orig(self, pos, color, rcam, pal);
            if (self.tiedToObject is Player && DroneOptions.usingDrone[slugIndex].Value)
            {
                self.color = DroneOptions.eyeColors[slugIndex].Value;
            }
        }


        private static void Oracle_ctor(On.Oracle.orig_ctor orig, Oracle self, AbstractPhysicalObject abstractPhysObj, Room room)
        {
            orig(self, abstractPhysObj, room);
            oracleID = self.ID;
        }


        private static void SSOracleBehavior_Update(On.SSOracleBehavior.orig_Update orig, SSOracleBehavior self, bool eu)
        {
            //UnityEngine.Debug.Log(self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad);
            //if (self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad <= 0 && self.action != MoreSlugcatsEnums.SSOracleBehaviorAction.Pebbles_SlumberParty)
            //{
            //    self.NewAction(SSOracleBehavior.Action.General_Idle);
            //}
            orig(self, eu);
            self.killFac = 0f;
        }


        private static void SSOracleBehavior_SeePlayer(On.SSOracleBehavior.orig_SeePlayer orig, SSOracleBehavior self)
        {
            pebblesOracle = self;
            if (currentSlug != MoreSlugcatsEnums.SlugcatStatsName.Artificer && (currentSlug != MoreSlugcatsEnums.SlugcatStatsName.Saint && self.player.room.world.region.name != "HR"))
            {
                if (self.oracle.ID == Oracle.OracleID.SS && self.action != MoreSlugcatsEnums.SSOracleBehaviorAction.Pebbles_SlumberParty && DroneOptions.usingDrone[slugIndex].Value && DroneOptions.noPebblesKill.Value)
                {
                    if (currentSlug == MoreSlugcatsEnums.SlugcatStatsName.Spear && self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad <= 0)
                    {
                        orig(self);
                    }
                    else
                    {
                        self.SlugcatEnterRoomReaction();
                        self.NewAction(MoreSlugcatsEnums.SSOracleBehaviorAction.Pebbles_SlumberParty);
                        return;
                    }

                }
            }
            orig(self);
        }


        private static void SSOracleBehavior_SSSleepoverBehavior_Update(On.SSOracleBehavior.SSSleepoverBehavior.orig_Update orig, SSOracleBehavior.SSSleepoverBehavior self)
        {
            //UnityEngine.Debug.Log(currentSlug);
            if (self.player != null)
            {
                if (currentSlug != MoreSlugcatsEnums.SlugcatStatsName.Artificer && (currentSlug != MoreSlugcatsEnums.SlugcatStatsName.Saint && self.player.room.world.region.name != "HR"))
                {
                    var physicalObjects = self.oracle.room.physicalObjects;

                    foreach (var layer in physicalObjects)
                    {
                        foreach (var physicalObject in layer)
                        {
                            if (physicalObject.grabbedBy.Count == 0 && physicalObject is DataPearl && physicalObject is not PebblesPearl && !pebblesOracle.talkedAboutThisSession.Contains((physicalObject as DataPearl).abstractPhysicalObject.ID) && physicalObject != null)
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
            }

            orig(self);
        }


        private static void Conversation_LoadEventsFromFile_int_Name_bool_int(On.Conversation.orig_LoadEventsFromFile_int_Name_bool_int orig, Conversation self, int whatever, SlugcatStats.Name slug, bool a, int h)
        {
            if (currentSlug != MoreSlugcatsEnums.SlugcatStatsName.Artificer)
            {
                if (DroneOptions.usingDrone[slugIndex].Value && oracleID == Oracle.OracleID.SS)
                {
                    slug = MoreSlugcatsEnums.SlugcatStatsName.Artificer;
                }
            }
            orig(self, whatever, slug, a, h);
        }


        private static string OracleBehavior_AlreadyDiscussedItemString(On.OracleBehavior.orig_AlreadyDiscussedItemString orig, OracleBehavior self, bool pearl)
        {
            if (currentSlug != MoreSlugcatsEnums.SlugcatStatsName.Artificer)
            {
                if (oracleID == Oracle.OracleID.SS && pearl)
                {
                    return "Ah, something to read?";
                }
            }

            return orig(self, pearl);
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

        //On.SSOracleBehavior.ThrowOutBehavior.Activate += (orig, self, oldAct, newAct) =>
        //{
        //    if (self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad >= 1 && self.action != MoreSlugcatsEnums.SSOracleBehaviorAction.Pebbles_SlumberParty)
        //    {
        //        UnityEngine.Debug.Log("BLAH");
        //        pebblesOracle.NewAction(MoreSlugcatsEnums.SSOracleBehaviorAction.Pebbles_SlumberParty);
        //    }
        //    else
        //    {
        //        orig(self, oldAct, newAct);
        //    }
        //};


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
