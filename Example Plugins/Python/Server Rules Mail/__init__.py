#This plugin sends a "Server Rules" mail to every player that connects to the server.
#It also adds a /rules command to resend the mail if a player needs it.

import clr
clr.AddReferenceToFileAndPath(GameDir + "IR.exe")
import Game
from System import DateTime

#This function sends the "Server Rules" mail to a player
def SendRulesMail ( player ):
	Sender = Game.ClientServer.Classes.ClSvMailParticipantNPC("Server") #Create an NPC Mail Participant and set it's name to "Server"
	Recipient = Game.ClientServer.Classes.ClSvMailParticipantPlayer() #Create an Player Mail Participant
	Recipient.ID = player.ID #Set the Recipient's ID to the player's ID
	
	Mail = Game.ClientServer.Classes.ClSvMail() #Create a new Mail
	Mail.Subject = "Server Rules" #Set the subject to "Server Rules"
	Mail.Body = "1. Be nice to other players\n2. Any glitching or cheating will result in a permanent ban\n3. Have fun!" #Set the Body of the mail to a string with some example rules
	Mail.Sender = Sender #Set the sender of the mail to the NPC participant defined above
	Mail.Recipient = Recipient #Set the recipient to the Player participant defined above
	Date = DateTime.Now #Set the sent date to right now
	
	player.SendMail(Mail, player.ID) #Send the mail to the player

#This function is called whenever a player joins the server
def OnAddPlayer ( player ):
	SendRulesMail(player)

#This function is called whenever a player types "/rules" in the chat.
#Parameter player: The player that typed the command
#Parameter text: Any text after the command
def OnRulesCommand ( player, text ):
	for Pair in player.PersonalState.Mail: #Foreach mail the player has...
		if (Pair.Value.Subject == "Server Rules" and Pair.Value.Sender is Game.ClientServer.Classes.ClSvMailParticipantNPC): #If the mail has a subject of "Server Rules" and the mail is sent by an NPC...
			player.PersonalState.Mail.Remove(Pair.Key) #Remove the mail from the player
			serverRemMail = Game.ClientServer.Packets.ServerLostMail() #Create an RPC packet that will tell the server to remove the mail from the player's mailbox
			serverRemMail.MailID = Pair.Key #Set the Mail ID to the value of Pair.Key
			
			player.SendRPC(serverRemMail) #Send the RPC Packet to the player
			break
			
	SendRulesMail(player) #Send the "Server Rules" mail to the player.

#This function is called whenever the server has been initialized.
def OnGameServerInitialized ( gameServer ):
	gameServer.Controllers.Chat.Commands.Add("rules", OnRulesCommand)
	
	gameServer.Controllers.Players.OnAddPlayer += OnAddPlayer

#A Python plugin doesn't have an Init method, unlike the C# Plugins.
#Just declare the initialization process directly in the root of the script.
#Like this:
PluginManager.OnGameServerInitialized += OnGameServerInitialized