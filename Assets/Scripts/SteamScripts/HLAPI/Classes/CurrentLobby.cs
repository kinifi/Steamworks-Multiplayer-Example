using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Steamworks;


public class CurrentLobby
{
	//the lobby ID
	public CSteamID lobby;
	//list of players their SteamPersona names
	public List<Player> m_players = new List<Player>();
	///a list of the chat messages
	public List<LobbyChatMessageValue> m_ChatMessages = new List<LobbyChatMessageValue>();
	//lobby name
	public string lobbyName;
	//lobby summary
	public string lobbySummary;
}
