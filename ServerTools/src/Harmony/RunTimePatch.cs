﻿using HarmonyLib;
using KinematicCharacterController;
using System;
using System.Reflection;
using UnityEngine;

namespace ServerTools
{
    public class RunTimePatch
    {

        public static void PatchAll()
        {
            try
            {
                Log.Out(string.Format("[SERVERTOOLS] Runtime patching initialized"));
                Harmony harmony = new Harmony("com.github.servertools.patch");

                MethodInfo original = AccessTools.Method(typeof(GameManager), "PlayerLoginRPC");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager.PlayerLoginRPC Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("PlayerLoginRPC_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: PlayerLoginRPC.prefix"));
                    }
                    MethodInfo postfix = typeof(Injections).GetMethod("PlayerLoginRPC_Postfix");
                    if (postfix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: PlayerLoginRPC_Postfix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
                }

                original = AccessTools.Method(typeof(ConnectionManager), "ServerConsoleCommand");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: ConnectionManager.ServerConsoleCommand Class.Method was not found"));
                }
                else
                {
                    MethodInfo postfix = typeof(Injections).GetMethod("ServerConsoleCommand_Postfix");
                    if (postfix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: ServerConsoleCommand_Postfix"));
                    }
                    harmony.Patch(original, null, new HarmonyMethod(postfix));
                }

                original = AccessTools.Method(typeof(GameManager), "ChangeBlocks");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager.ChangeBlocks Class.Method was not found"));
                }
                else
                {
                    MethodInfo postfix = typeof(Injections).GetMethod("GameManager_ChangeBlocks_Postfix");
                    if (postfix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager_ChangeBlocks_Postfix"));
                    }
                    harmony.Patch(original, null, new HarmonyMethod(postfix));
                }

                original = AccessTools.Method(typeof(World), "AddFallingBlock");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: World.AddFallingBlock Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("AddFallingBlock_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: AddFallingBlock_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(World), "AddFallingBlocks");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: World.AddFallingBlocks Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("AddFallingBlocks_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: AddFallingBlocks_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(GameManager), "Cleanup");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager.Cleanup Class.Method was not found"));
                }
                else
                {
                    MethodInfo finalizer = typeof(Injections).GetMethod("GameManager_Cleanup_Finalizer");
                    if (finalizer == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager_Cleanup_Finalizer"));
                    }
                    harmony.Patch(original, null, null, null, new HarmonyMethod(finalizer));
                }

                original = AccessTools.Method(typeof(GameManager), "CollectEntityServer");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager.CollectEntityServer Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("GameManager_CollectEntityServer_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager_CollectEntityServer_Prefix"));
                        return;
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(GameManager), "OpenTileEntityAllowed");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager.OpenTileEntityAllowed Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("GameManager_OpenTileEntityAllowed_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager_OpenTileEntityAllowed_Prefix"));
                        return;
                    }
                    MethodInfo postfix = typeof(Injections).GetMethod("GameManager_OpenTileEntityAllowed_Postfix");
                    if (postfix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager_OpenTileEntityAllowed_Postfix"));
                        return;
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
                }

                original = AccessTools.Method(typeof(GameManager), "PlayerSpawnedInWorld");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager.PlayerSpawnedInWorld Class.Method was not found"));
                }
                else
                {
                    MethodInfo postfix = typeof(Injections).GetMethod("GameManager_PlayerSpawnedInWorld_Postfix");
                    if (postfix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager_PlayerSpawnedInWorld.postfix"));
                        return;
                    }
                    harmony.Patch(original, null, new HarmonyMethod(postfix));
                }

                original = AccessTools.Method(typeof(EntityAlive), "SetDead");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: EntityAlive.SetDead Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("EntityAlive_SetDead_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: EntityAlive_SetDead.prefix"));
                        return;
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(EntityAlive), "OnAddedToWorld");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: EntityAlive.OnAddedToWorld Class.Method was not found"));
                }
                else
                {
                    MethodInfo postfix = typeof(Injections).GetMethod("EntityAlive_OnAddedToWorld_Postfix");
                    if (postfix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: EntityAlive_OnAddedToWorld.postfix"));
                        return;
                    }
                    harmony.Patch(original, null, new HarmonyMethod(postfix));
                }

                original = AccessTools.Method(typeof(NetPackagePlayerInventory), "ProcessPackage");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackagePlayerInventory.ProcessPackage Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("NetPackagePlayerInventory_ProcessPackage_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackagePlayerInventory_ProcessPackage_Prefix"));
                    }
                    MethodInfo postfix = typeof(Injections).GetMethod("NetPackagePlayerInventory_ProcessPackage_Postfix");
                    if (postfix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackagePlayerInventory_ProcessPackage_Postfix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
                }

                original = AccessTools.Method(typeof(ClientInfoCollection), "GetForNameOrId");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: ClientInfoCollection.GetForNameOrId Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("ClientInfoCollection_GetForNameOrId_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: ClientInfoCollection_GetForNameOrId_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(NetPackagePlayerStats), "ProcessPackage");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackagePlayerStats.ProcessPackage Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("NetPackagePlayerStats_ProcessPackage_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackagePlayerStats_ProcessPackage_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(NetPackageEntityAddScoreServer), "ProcessPackage");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackageEntityAddScoreServer.ProcessPackage Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("NetPackageEntityAddScoreServer_ProcessPackage_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackageEntityAddScoreServer_ProcessPackage_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(NetPackageChat), "ProcessPackage");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackageChat.ProcessPackage Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("NetPackageChat_ProcessPackage_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackageChat_ProcessPackage_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(NetPackageEntityPosAndRot), "ProcessPackage");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackageEntityPosAndRot.ProcessPackage Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("NetPackageEntityPosAndRot_ProcessPackage_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackageEntityPosAndRot_ProcessPackage_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(NetPackagePlayerData), "ProcessPackage");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackagePlayerData.ProcessPackage Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("NetPackagePlayerData_ProcessPackage_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackagePlayerData_ProcessPackage_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(NetPackageDamageEntity), "ProcessPackage");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackageDamageEntity.ProcessPackage Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("NetPackageDamageEntity_ProcessPackage_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackageDamageEntity_ProcessPackage_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(ClientInfo), "SendPackage");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: ClientInfo.SendPackage Class.Method was not found"));
                }
                else
                {
                    MethodInfo postfix = typeof(Injections).GetMethod("ClientInfo_SendPackage_Postfix");
                    if (postfix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: ClientInfo_SendPackage_Postfix"));
                    }
                    harmony.Patch(original, null, new HarmonyMethod(postfix));
                }

                original = AccessTools.Method(typeof(PersistentPlayerList), "PlaceLandProtectionBlock");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: PersistentPlayerList.PlaceLandProtectionBlock Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("PersistentPlayerList_PlaceLandProtectionBlock_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: PersistentPlayerList_PlaceLandProtectionBlock_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(NetPackageEntityAttach), "ProcessPackage");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackageEntityAttach.ProcessPackage Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("NetPackageEntityAttach_ProcessPackage_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackageEntityAttach_ProcessPackage_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(GameManager), "ExplosionServer");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager.ExplosionServer Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("GameManager_ExplosionServer_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager_ExplosionServer_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(LootManager), "LootContainerOpened");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: LootManager.LootContainerOpened Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("LootManager_LootContainerOpened_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: LootManager_LootContainerOpened_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(NetPackageTileEntity), "Setup", new Type[] { typeof(TileEntity), typeof(TileEntity.StreamModeWrite), typeof(byte) });
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackageTileEntity.Setup Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("NetPackageTileEntity_Setup_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: NetPackageTileEntity_Setup_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(GameManager), "DropContentOfLootContainerServer", new Type[] { typeof(BlockValue), typeof(Vector3i), typeof(int) });
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager.DropContentOfLootContainerServer Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("GameManager_DropContentOfLootContainerServer_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager_DropContentOfLootContainerServer_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(GameManager), "SavePlayerData");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager.SavePlayerData Class.Method was not found"));
                }
                else
                {
                    MethodInfo postfix = typeof(Injections).GetMethod("GameManager_SavePlayerData_Postfix");
                    if (postfix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: GameManager_SavePlayerData_Postfix"));
                    }
                    harmony.Patch(original, null, new HarmonyMethod(postfix));
                }
                
                original = AccessTools.Method(typeof(ConsoleCmdChunkReset), "Execute");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: ConsoleCmdChunkReset.Execute Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("ConsoleCmdChunkReset_Execute_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: ConsoleCmdChunkReset_Execute_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(BlockLandClaim), "HandleDeactivatingCurrentLandClaims");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: BlockLandClaim.HandleDeactivatingCurrentLandClaims Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("BlockLandClaim_HandleDeactivatingCurrentLandClaims_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: BlockLandClaim_HandleDeactivatingCurrentLandClaims_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(Log), "Out", new Type[] { typeof(string) });
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: Log.Out Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("Log_Out_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: Log_Out_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(EntityAlive), "OnEntityDeath");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: EntityAlive.OnEntityDeath Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("EntityAlive_OnEntityDeath_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: EntityAlive_OnEntityDeath_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                original = AccessTools.Method(typeof(TileEntity), "setModified");
                if (original == null)
                {
                    Log.Out(string.Format("[SERVERTOOLS] Injection failed: TileEntity.setModified Class.Method was not found"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("TileEntity_setModified_Prefix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Injection failed: TileEntity_setModified_Prefix"));
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                

                Log.Out(string.Format("[SERVERTOOLS] Runtime patching complete"));
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in PatchTools.PatchAll: {0}", e.StackTrace));
            }
        }
    }
}