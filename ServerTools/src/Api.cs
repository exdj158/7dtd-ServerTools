﻿using Platform.Steam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServerTools
{
    public class API : IModApi
    {
        public static string GamePath = Directory.GetCurrentDirectory();
        public static string ConfigPath = string.Format("{0}/ServerTools", GamePath);
        public static List<string> Verified = new List<string>();

        public void InitMod(Mod _modInstance)
        {
            ModEvents.GameAwake.RegisterHandler(GameAwake);
            ModEvents.GameStartDone.RegisterHandler(GameStartDone);
            ModEvents.GameShutdown.RegisterHandler(GameShutdown);

            ModEvents.PlayerLogin.RegisterHandler(PlayerLogin);
            ModEvents.PlayerSpawning.RegisterHandler(PlayerSpawning);
            ModEvents.PlayerSpawnedInWorld.RegisterHandler(PlayerSpawnedInWorld);
            ModEvents.ChatMessage.RegisterHandler(ChatMessage);
            ModEvents.GameMessage.RegisterHandler(GameMessage);
            ModEvents.SavePlayerData.RegisterHandler(SavePlayerData);
            ModEvents.PlayerDisconnected.RegisterHandler(PlayerDisconnected);

            //ModEvents.EntityKilled.RegisterHandler(EntityKilled);
        }

        private void GameAwake()
        {
            OutputLog.Exec();
        }

        private static void GameStartDone()
        {
            try
            {
                LoadProcess.Load();
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in API.GameStartDone: {0}", e.Message));
            }
        }

        private static void GameShutdown()
        {
            try
            {
                PersistentOperations.Shutdown_Initiated = true;
                if (WebAPI.IsEnabled && WebAPI.IsRunning)
                {
                    WebAPI.Unload();
                }
                Timers.CoreTimerStop();
                RegionReset.Exec();
                Phrases.Unload();
                CommandList.Unload();
                OutputLog.Shutdown();
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in API.GameShutdown: {0}", e.Message));
            }
        }

        private static bool PlayerLogin(ClientInfo _cInfo, string _message, StringBuilder _stringBuild)//Initiating player login
        {
            try
            {
                if (_cInfo != null && _cInfo.CrossplatformId != null)
                {
                    if (Shutdown.NoEntry)
                    {
                        Phrases.Dict.TryGetValue("Shutdown4", out string phrase);
                        ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                        SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync(string.Format("kick {0} \"{1}\"", _cInfo.CrossplatformId.CombinedString, phrase), null);
                    }
                    if (NewPlayer.Block_During_Bloodmoon && PersistentOperations.IsBloodmoon() && ConnectionManager.Instance.ClientCount() > 1)
                    {
                        Phrases.Dict.TryGetValue("NewPlayer1", out string phrase);
                        PlayerDataFile pdf = PersistentOperations.GetPlayerDataFileFromUId(_cInfo.CrossplatformId);
                        if (pdf != null)
                        {
                            if (pdf.totalTimePlayed < 5)
                            {
                                SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync(string.Format("kick {0} \"{1}\"", _cInfo.CrossplatformId.CombinedString, phrase), null);
                            }
                        }
                        else
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync(string.Format("kick {0} \"{1}\"", _cInfo.CrossplatformId.CombinedString, phrase), null);
                        }
                    }
                    if (FamilyShare.IsEnabled && _cInfo.PlatformId is UserIdentifierSteam)
                    {
                        FamilyShare.Exec(_cInfo);
                    }
                    if (!string.IsNullOrEmpty(_cInfo.ip))
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Player connected with ID '{0}' '{1}' and IP '{2}' named '{3}'", _cInfo.PlatformId.CombinedString, _cInfo.CrossplatformId.CombinedString, _cInfo.ip, _cInfo.playerName));
                    }
                    else
                    {
                        Log.Out(string.Format("[SERVERTOOLS] Player connected with ID '{0}' '{1}' named '{2}'", _cInfo.PlatformId.CombinedString, _cInfo.CrossplatformId.CombinedString, _cInfo.playerName));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in API.PlayerLogin: {0}", e.Message));
            }
            return true;
        }

        private static void PlayerSpawning(ClientInfo _cInfo, int _chunkViewDim, PlayerProfile _playerProfile)//Setting player view and profile
        {
            
        }

        private static void PlayerSpawnedInWorld(ClientInfo _cInfo, RespawnType _respawnReason, Vector3i _pos)//Spawning player
        {
            try
            {
                if (_cInfo != null)
                {
                    string id = _cInfo.CrossplatformId.CombinedString;
                    EntityPlayer player = PersistentOperations.GetEntityPlayer(_cInfo.entityId);
                    if (player != null)
                    {
                        if (_respawnReason == RespawnType.NewGame)
                        {
                            if (player.distanceWalked < 1 && player.totalTimePlayed <= 1 && !PersistentOperations.NewPlayerQue.Contains(_cInfo))
                            {
                                PersistentOperations.NewPlayerQue.Add(_cInfo);
                            }
                        }
                        else if (_respawnReason == RespawnType.LoadedGame)
                        {

                        }
                        else if (_respawnReason == RespawnType.EnterMultiplayer)
                        {
                            PersistentOperations.SessionTime(_cInfo);
                            PersistentContainer.Instance.Players[id].PlayerName = _cInfo.playerName;
                            PersistentContainer.Instance.Players[id].LastJoined = DateTime.Now;
                            PersistentContainer.DataChange = true;
                            if (player.AttachedToEntity != null)
                            {
                                player.Detach();
                            }
                            if (player.distanceWalked < 1 && player.totalTimePlayed <= 1 && !PersistentOperations.NewPlayerQue.Contains(_cInfo))
                            {
                                PersistentOperations.NewPlayerQue.Add(_cInfo);
                            }
                            else
                            {
                                OldPlayerJoined(_cInfo, player);
                            }
                        }
                        else if (_respawnReason == RespawnType.JoinMultiplayer)
                        {
                            PersistentOperations.SessionTime(_cInfo);
                            PersistentContainer.Instance.Players[id].PlayerName = _cInfo.playerName;
                            PersistentContainer.Instance.Players[id].LastJoined = DateTime.Now;
                            PersistentContainer.DataChange = true;
                            if (player.AttachedToEntity != null)
                            {
                                player.Detach();
                            }
                            if (player.distanceWalked < 1 && player.totalTimePlayed <= 1 && !PersistentOperations.NewPlayerQue.Contains(_cInfo))
                            {
                                PersistentOperations.NewPlayerQue.Add(_cInfo);
                            }
                            else
                            {
                                OldPlayerJoined(_cInfo, player);
                            }
                            if (AutoPartyInvite.IsEnabled)
                            {
                                AutoPartyInvite.Exec(_cInfo, player);
                            }
                        }
                        else if (_respawnReason == RespawnType.Died)
                        {
                            if (player.AttachedToEntity != null)
                            {
                                player.Detach();
                            }
                        }
                        else if (_respawnReason == RespawnType.Teleport)
                        {
                            if (Teleportation.Teleporting.Contains(_cInfo.entityId))
                            {
                                Teleportation.Teleporting.Remove(_cInfo.entityId);
                            }
                        }
                        if (PlayerChecks.TwoSecondMovement.ContainsKey(_cInfo.entityId))
                        {
                            PlayerChecks.TwoSecondMovement.Remove(_cInfo.entityId);
                        }
                        if (FlyingDetector.Flags.ContainsKey(_cInfo.entityId))
                        {
                            FlyingDetector.Flags.Remove(_cInfo.entityId);
                        }
                        if (SpeedDetector.Flags.ContainsKey(_cInfo.entityId))
                        {
                            SpeedDetector.Flags.Remove(_cInfo.entityId);
                        }
                    }
                    if (ExitCommand.IsEnabled && !ExitCommand.Players.ContainsKey(_cInfo.entityId) && (GameManager.Instance.adminTools.GetUserPermissionLevel(_cInfo.PlatformId) > ExitCommand.Admin_Level &&
                        GameManager.Instance.adminTools.GetUserPermissionLevel(_cInfo.CrossplatformId) > ExitCommand.Admin_Level))
                    {
                        ExitCommand.Players.Add(_cInfo.entityId, player.position);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in API.PlayerSpawnedInWorld: {0}", e.Message));
            }
        }

        private static bool ChatMessage(ClientInfo _cInfo, EChatType _type, int _senderId, string _msg, string _mainName, bool _localizeMain, List<int> _recipientEntityIds)
        {
            return ChatHook.Hook(_cInfo, _type, _senderId, _msg, _mainName, _recipientEntityIds);
        }

        private static bool GameMessage(ClientInfo _cInfo, EnumGameMessages _type, string _msg, string _mainName, bool _localizeMain, string _secondaryName, bool _localizeSecondary)
        {
            try
            {
                if (_cInfo != null && _type == EnumGameMessages.EntityWasKilled)
                {
                    EntityPlayer player = PersistentOperations.GetEntityPlayer(_cInfo.entityId);
                    if (player != null)
                    {
                        if (KillNotice.IsEnabled && (KillNotice.Zombie_Kills || KillNotice .Animal_Kills) && string.IsNullOrEmpty(_secondaryName))
                        {
                            if (KillNotice.Damage.ContainsKey(player.entityId))
                            {
                                KillNotice.Damage.TryGetValue(player.entityId, out int[] damage);
                                EntityZombie zombie = PersistentOperations.GetZombie(damage[0]);
                                if (zombie != null)
                                {
                                    KillNotice.ZombieKilledPlayer(zombie, player, _cInfo, damage[1]);
                                }
                                else
                                {
                                    EntityAnimal animal = PersistentOperations.GetAnimal(damage[0]);
                                    if (animal != null)
                                    {
                                        KillNotice.AnimalKilledPlayer(animal, player, _cInfo, damage[1]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in API.GameMessage: {0}", e.Message));
            }
            return true;
        }

        private static void SavePlayerData(ClientInfo _cInfo, PlayerDataFile _playerDataFile)
        {
            try
            {
                if (_cInfo != null && _playerDataFile != null)
                {
                    if (HighPingKicker.IsEnabled)
                    {
                        HighPingKicker.Exec(_cInfo);
                    }
                    if (InvalidItems.IsEnabled || InvalidItems.Invalid_Stack)
                    {
                        InvalidItems.CheckInv(_cInfo, _playerDataFile);
                    }
                    if (DupeLog.IsEnabled)
                    {
                        DupeLog.Exec(_cInfo, _playerDataFile);
                    }
                    if (LevelUp.IsEnabled)
                    {
                        LevelUp.CheckLevel(_cInfo);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in API.SavePlayerData: {0}", e.Message));
            }
        }

        public static void PlayerDisconnected(ClientInfo _cInfo, bool _bShutdown)
        {
            try
            {
                if (_cInfo != null && _cInfo.CrossplatformId != null)
                {
                    string id = _cInfo.CrossplatformId.CombinedString;
                    Log.Out(string.Format("[SERVERTOOLS] Player with id '{0}' '{1}' disconnected", _cInfo.PlatformId.CombinedString, id));
                    if (ExitCommand.IsEnabled && ExitCommand.Players.ContainsKey(_cInfo.entityId) && !_bShutdown)
                    {
                        Timers.ExitWithoutCommand(_cInfo, _cInfo.ip);
                    }
                    if (FriendTeleport.Dict.ContainsKey(_cInfo.entityId))
                    {
                        FriendTeleport.Dict.Remove(_cInfo.entityId);
                        FriendTeleport.Dict1.Remove(_cInfo.entityId);
                    }
                    if (FriendTeleport.Dict.ContainsKey(_cInfo.entityId))
                    {
                        FriendTeleport.Dict.Remove(_cInfo.entityId);
                    }
                    if (FriendTeleport.Dict1.ContainsKey(_cInfo.entityId))
                    {
                        FriendTeleport.Dict1.Remove(_cInfo.entityId);
                    }
                    if (Wallet.IsEnabled && Wallet.Session_Bonus > 0)
                    {
                        if (PersistentOperations.Session.TryGetValue(id, out DateTime time))
                        {
                            TimeSpan varTime = DateTime.Now - time;
                            double fractionalMinutes = varTime.TotalMinutes;
                            int timepassed = (int)fractionalMinutes;
                            if (timepassed > 60)
                            {
                                int sessionBonus = timepassed / 60 * Wallet.Session_Bonus;
                                if (sessionBonus > 0)
                                {
                                    Wallet.AddCurrency(id, sessionBonus);
                                }
                            }
                            int timePlayed = PersistentContainer.Instance.Players[id].TotalTimePlayed;
                            PersistentContainer.Instance.Players[id].TotalTimePlayed = timePlayed + timepassed;
                            PersistentContainer.DataChange = true;
                        }
                    }
                    if (Bank.TransferId.ContainsKey(id))
                    {
                        Bank.TransferId.Remove(id);
                    }
                    if (Zones.ZonePlayer.ContainsKey(_cInfo.entityId))
                    {
                        Zones.ZonePlayer.Remove(_cInfo.entityId);
                    }
                    if (Zones.Reminder.ContainsKey(_cInfo.entityId))
                    {
                        Zones.Reminder.Remove(_cInfo.entityId);
                    }
                    if (BloodmoonWarrior.WarriorList.Contains(_cInfo.entityId))
                    {
                        BloodmoonWarrior.WarriorList.Remove(_cInfo.entityId);
                        BloodmoonWarrior.KilledZombies.Remove(_cInfo.entityId);
                    }
                    if (Teleportation.Teleporting.Contains(_cInfo.entityId))
                    {
                        Teleportation.Teleporting.Remove(_cInfo.entityId);
                    }
                    if (LevelUp.PlayerLevels.ContainsKey(_cInfo.entityId))
                    {
                        LevelUp.PlayerLevels.Remove(_cInfo.entityId);
                    }
                    if (KillNotice.Damage.ContainsKey(_cInfo.entityId))
                    {
                        KillNotice.Damage.Remove(_cInfo.entityId);
                    }
                    if (InfiniteAmmo.Dict.ContainsKey(_cInfo.entityId))
                    {
                        InfiniteAmmo.Dict.Remove(_cInfo.entityId);
                    }
                    if (PersistentOperations.NewPlayerQue.Contains(_cInfo))
                    {
                        PersistentOperations.NewPlayerQue.Remove(_cInfo);
                    }
                    if (PersistentOperations.BlockChatCommands.Contains(_cInfo))
                    {
                        PersistentOperations.BlockChatCommands.Remove(_cInfo);
                    }
                    if (PersistentOperations.Session.ContainsKey(id))
                    {
                        PersistentOperations.Session.Remove(id);
                    }
                }
                else
                {
                    Log.Out("[SERVERTOOLS] Player disconnected");
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in API.PlayerDisconnected: {0}", e.Message));
            }
        }

        public static void NewPlayerExec(ClientInfo _cInfo)
        {
            try
            {
                EntityPlayer player = PersistentOperations.GetEntityPlayer(_cInfo.entityId);
                if (player != null)
                {
                    if (player.IsSpawned() && player.IsAlive())
                    {
                        if (NewSpawnTele.IsEnabled && NewSpawnTele.New_Spawn_Tele_Position != "0,0,0")
                        {
                            NewSpawnTele.TeleNewSpawn(_cInfo, player);
                            if (StartingItems.IsEnabled && StartingItems.Dict.Count > 0)
                            {
                                Timers.StartingItemsTimer(_cInfo);
                            }
                        }
                        else if (StartingItems.IsEnabled && StartingItems.Dict.Count > 0)
                        {
                            StartingItems.Exec(_cInfo, null);
                        }
                        ProcessPlayer(_cInfo, player);
                    }
                    else
                    {
                        PersistentOperations.NewPlayerQue.Add(_cInfo);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in API.NewPlayerExec: {0}", e.Message));
            }
        }

        public static void ProcessPlayer(ClientInfo _cInfo, EntityPlayer _player)
        {
            try
            {
                string id = _cInfo.CrossplatformId.CombinedString;
                if (NewPlayer.IsEnabled)
                {
                    NewPlayer.Exec(_cInfo);
                }
                if (Motd.IsEnabled)
                {
                    Motd.Send(_cInfo);
                }
                if (Bloodmoon.IsEnabled)
                {
                    Bloodmoon.Exec(_cInfo);
                }
                if (ExitCommand.IsEnabled)
                {
                    ExitCommand.AlertPlayer(_cInfo);
                }
                if (Poll.IsEnabled && PersistentContainer.Instance.PollOpen && !PersistentContainer.Instance.PollVote.ContainsKey(id))
                {
                    Poll.Message(_cInfo);
                }
                if (Hardcore.IsEnabled)
                {
                    if (Hardcore.Optional)
                    {
                        if (PersistentContainer.Instance.Players[id].HardcoreEnabled)
                        {
                            Hardcore.Check(_cInfo, _player);
                        }
                    }
                    else if (!PersistentContainer.Instance.Players[id].HardcoreEnabled)
                    {
                        string[] hardcoreStats = { _cInfo.playerName, XUiM_Player.GetDeaths(_player).ToString(), "0" };
                        PersistentContainer.Instance.Players[id].HardcoreStats = hardcoreStats;
                        PersistentContainer.Instance.Players[id].HardcoreEnabled = true;
                        PersistentContainer.DataChange = true;
                        Hardcore.Check(_cInfo, _player);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in API.ProcessPlayer: {0}", e.Message));
            }
        }

        public static void OldPlayerJoined(ClientInfo _cInfo, EntityPlayer _player)
        {
            try
            {
                string id = _cInfo.CrossplatformId.CombinedString;
                if (Hardcore.IsEnabled && PersistentContainer.Instance.Players[id] != null)
                {
                    if (Hardcore.Optional)
                    {
                        if (PersistentContainer.Instance.Players[id].HardcoreEnabled)
                        {
                            Hardcore.Check(_cInfo, _player);
                        }
                    }
                    else if (!PersistentContainer.Instance.Players[id].HardcoreEnabled)
                    {
                        string[] hardcoreStats = { _cInfo.playerName, XUiM_Player.GetDeaths(_player).ToString(), "0" };
                        PersistentContainer.Instance.Players[id].HardcoreStats = hardcoreStats;
                        PersistentContainer.Instance.Players[id].HardcoreEnabled = true;
                        PersistentContainer.DataChange = true;
                        Hardcore.Check(_cInfo, _player);
                    }
                }
                if (LoginNotice.IsEnabled && LoginNotice.Dict.ContainsKey(_cInfo.PlatformId.CombinedString) || LoginNotice.Dict.ContainsKey(id))
                {
                    LoginNotice.PlayerNotice(_cInfo);
                }
                if (Motd.IsEnabled)
                {
                    Motd.Send(_cInfo);
                }
                if (Bloodmoon.IsEnabled)
                {
                    Bloodmoon.Exec(_cInfo);
                }
                if (Shutdown.IsEnabled && Shutdown.Alert_On_Login)
                {
                    Shutdown.NextShutdown(_cInfo);
                }
                if (ExitCommand.IsEnabled)
                {
                    ExitCommand.AlertPlayer(_cInfo);
                }
                if (Poll.IsEnabled && PersistentContainer.Instance.PollOpen && !PersistentContainer.Instance.PollVote.ContainsKey(id))
                {
                    Poll.Message(_cInfo);
                }
                if (ClanManager.IsEnabled && PersistentContainer.Instance.Players[id] != null)
                {
                    Dictionary<string, string> clanRequests = PersistentContainer.Instance.Players[id].ClanRequestToJoin;
                    if (clanRequests != null && clanRequests.Count > 0)
                    {
                        ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + "New clan requests from:[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                        foreach (var request in clanRequests)
                        {
                            ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + request.Value + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                        }
                    }
                }
                if (Event.Open && Event.Teams.ContainsKey(id) && PersistentContainer.Instance.Players[id] != null && PersistentContainer.Instance.Players[id].EventSpawn)
                {
                    Event.Spawn(_cInfo);
                }
                else if (PersistentContainer.Instance.Players[id] != null && PersistentContainer.Instance.Players[id].EventSpawn)
                {
                    PersistentContainer.Instance.Players[id].EventSpawn = false;
                    PersistentContainer.DataChange = true;
                }
                if (PersistentContainer.Instance.Players[id] != null && PersistentContainer.Instance.Players[id].PlayerWallet > 0)
                {
                    Wallet.AddCurrency(id, PersistentContainer.Instance.Players[id].PlayerWallet);
                    PersistentContainer.Instance.Players[id].PlayerWallet = 0;
                    PersistentContainer.DataChange = true;
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in API.OldPlayerJoined: {0}", e.Message));
            }
        }

        public static void PlayerDied(EntityAlive __instance)
        {
            ClientInfo cInfo = PersistentOperations.GetClientInfoFromEntityId(__instance.entityId);
            if (cInfo != null)
            {
                string id = cInfo.CrossplatformId.CombinedString;
                if (__instance.AttachedToEntity != null)
                {
                    __instance.Detach();
                }
                if (Died.IsEnabled)
                {
                    Died.PlayerKilled(__instance);
                }
                if (ProcessDamage.lastEntityKilled == cInfo.entityId)
                {
                    ProcessDamage.lastEntityKilled = 0;
                }
                if (Bloodmoon.IsEnabled && Bloodmoon.Show_On_Respawn)
                {
                    Bloodmoon.Exec(cInfo);
                }
                if (Event.Open && Event.Teams.ContainsKey(id))
                {
                    Event.Respawn(cInfo);
                }
                if (PersistentContainer.Instance.Players[id].EventOver)
                {
                    Event.EventOver(cInfo);
                }
                if (Hardcore.IsEnabled && PersistentContainer.Instance.Players[id].HardcoreEnabled)
                {
                    Hardcore.Check(cInfo, (EntityPlayer)__instance);
                }
                if (Zones.ZonePlayer.ContainsKey(cInfo.entityId))
                {
                    Zones.ZonePlayer.Remove(cInfo.entityId);
                }
                if (PlayerChecks.TwoSecondMovement.ContainsKey(cInfo.entityId))
                {
                    PlayerChecks.TwoSecondMovement.Remove(cInfo.entityId);
                }
                if (FlyingDetector.Flags.ContainsKey(cInfo.entityId))
                {
                    FlyingDetector.Flags.Remove(cInfo.entityId);
                }
                if (SpeedDetector.Flags.ContainsKey(cInfo.entityId))
                {
                    SpeedDetector.Flags.Remove(cInfo.entityId);
                }
            }
        }
    }
}