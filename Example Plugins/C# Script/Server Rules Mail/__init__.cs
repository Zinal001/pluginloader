using Game.GameStates;
using PluginLoader;
using System;
using System.Collections.Generic;

namespace Plugins
{
    /// <summary>
    /// This plugin sends a "Server Rules" mail to every player that connects to the server.
    /// It also adds a /rules command to resend the mail if a player needs it.
    /// </summary>
    public class RulesPlugin : PluginClass //<--- NOTE: Every plugin has to be a subclass of PluginClass (Defined in PluginLoader)
    {
        private String _RulesText = null;

        /// <summary>
        /// This function is called when the plugin is loaded by the Plugin Loader.
        /// It's the only function that is required by the PluginClass.
        /// This function is always required
        /// </summary>
        public override void Init()
        {
            Global.PluginManager.OnGameServerInitialized += PluginManager_OnGameServerInitialized;
        }

        /// <summary>
        /// This function is called whenever the server has been initialized.
        /// </summary>
        /// <param name="arg">The GameServer instance</param>
        private void PluginManager_OnGameServerInitialized(object arg = null)
        {
            GameServer gameServer = (GameServer)arg;

            gameServer.Controllers.Chat.Commands.Add("rules", new Action<Game.Server.Player, string>(OnRulesCommand));

            gameServer.Controllers.Players.OnAddPlayer += Players_OnAddPlayer;
        }

        /// <summary>
        /// This function is called whenever a player types "/rules" in the chat.
        /// </summary>
        /// <param name="player">The player that typed the command</param>
        /// <param name="text">Any text after the command</param>
        void OnRulesCommand(Game.Server.Player player, String text)
        {
            foreach(KeyValuePair<Guid, Game.ClientServer.Classes.ClSvMail> Pair in player.PersonalState.Mail) //Foreach mail the player has...
            {
                if(Pair.Value.Subject == "Server Rules" && Pair.Value.Sender is Game.ClientServer.Classes.ClSvMailParticipantNPC) //If the mail has a subject of "Server Rules" and the mail is sent by an NPC...
                {
                    player.PersonalState.Mail.Remove(Pair.Key); //Remove the mail from the player
                    Game.ClientServer.Packets.ServerLostMail serverRemMail = new Game.ClientServer.Packets.ServerLostMail() {
                        MailID = Pair.Key
                    }; //Create an RPC packet that will tell the server to remove the mail from the player's mailbox

                    player.SendRPC(serverRemMail); //Send the RPC Packet to the player
                    break;
                }
            }
            
            SendRulesMail(player); //Send the "Server Rules" mail to the player.
        }

        /// <summary>
        /// This function is called whenever a player joins the server
        /// </summary>
        /// <param name="player"></param>
        private void Players_OnAddPlayer(Game.Server.Player player)
        {
            SendRulesMail(player);
        }

        /// <summary>
        /// This function sends the "Server Rules" mail to a player
        /// </summary>
        /// <param name="player"></param>
        private void SendRulesMail(Game.Server.Player player)
        {
            if(String.IsNullOrEmpty(_RulesText)) //If the _RulesText variable (defined above) is empty...
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(this.PluginDir, "rules.txt"))) //If a text file named "rules.txt" exists in the root of the plugin direction...
                    _RulesText = String.Join("\n", System.IO.File.ReadAllLines(System.IO.Path.Combine(this.PluginDir, "rules.txt"))); //Read the file and set _RulesText to it
                else
                    _RulesText = "1. Be nice to other players\n2. Any glitching or cheating will result in a permanent ban\n3. Have fun!"; //The file didn't exist, so create a couple of "example" rules
            }

            //Send a mail to the player with a subject of "Server Rules" and set the sender to an NPC named "Server".
            player.SendMail(new Game.ClientServer.Classes.ClSvMail()
            {
                Subject = "Server Rules",
                Body = _RulesText,
                Sender = new Game.ClientServer.Classes.ClSvMailParticipantNPC("Server"),
                Recipient = new Game.ClientServer.Classes.ClSvMailParticipantPlayer() { ID = player.ID },
                Date = DateTime.Now
            }, player.ID);
        }
    }
}
