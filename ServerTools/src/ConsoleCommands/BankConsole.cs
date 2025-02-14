﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    class BankConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools] - Enable or disable bank.";
        }

        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. st-bk off\n" +
                   "  2. st-bk on\n" +
                   "  3. st-bk <EOS/EntityId/PlayerName>\n" +
                   "1. Turn off the bank\n" +
                   "2. Turn on the bank\n" +
                   "3. Shows the current bank value of the specified player\n";
        }

        public override string[] GetCommands()
        {
            return new string[] { "st-Bank", "bk", "st-bk" };
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count != 1)
                {
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("[SERVERTOOLS] Wrong number of arguments, expected 1, found {0}", _params.Count));
                    return;
                }
                if (_params[0].ToLower().Equals("off"))
                {
                    if (Bank.IsEnabled)
                    {
                        Bank.IsEnabled = false;
                        Config.WriteXml();
                        Config.LoadXml();
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("[SERVERTOOLS] Bank has been set to off"));
                        return;
                    }
                    else
                    {
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("[SERVERTOOLS] Bank is already off"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    if (!Bank.IsEnabled)
                    {
                        Bank.IsEnabled = true;
                        Config.WriteXml();
                        Config.LoadXml();
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("[SERVERTOOLS] Bank has been set to on"));
                        return;
                    }
                    else
                    {
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("[SERVERTOOLS] Bank is already on"));
                        return;
                    }
                }
                else
                {
                    ClientInfo cInfo = GeneralOperations.GetClientInfoFromNameOrId(_params[0]);
                    if (cInfo != null)
                    {
                        int currentBank = Bank.GetCurrency(cInfo.CrossplatformId.CombinedString);
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("[SERVERTOOLS] Id '{0}' named '{1}' has '{2}' '{3}' in their bank", _params[0], cInfo.playerName, currentBank, Wallet.Currency_Name));
                    }
                    else if (_params[0].Contains("_") && PersistentContainer.Instance.Players[_params[0]] != null)
                    {
                        int currentBank = PersistentContainer.Instance.Players[_params[0]].Bank;
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("[SERVERTOOLS] Id '{0}' named '{1}' has '{2}' '{3}' in their bank", _params[0], cInfo.playerName, currentBank, Wallet.Currency_Name));
                    }
                    else
                    {
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("[SERVERTOOLS] Invalid argument {0}", _params[0]));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in BankConsole.Execute: {0}", e.Message));
            }
        }
    }
}