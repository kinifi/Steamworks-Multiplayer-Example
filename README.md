# Steamworks-Multiplayer-Example

This is an attempt at making Steam Multiplayer Easier in Unity via inheritance. 


Creating a new script and inheriting from SteamServerCreator will hide all the callbacks needed to get what you need to start the game. 

```csharp
using UnityEngine;
using System.Collections;

public class SteamTestScript : SteamServerCreator {

	// Use this for initialization
	void Start () {

		//setup the server
		Setup();

		//request all the lobbbies
		RequestLobbyList();
	
	}
	
}
```

Accessing the lobbies after this is just accessing a List of a class called Lobby.cs
```csharp
  ///<summary>
	/// the lobby list from RequestLobbyList
	///</summary>
	public List<Lobby> m_lobbyList = new List<Lobby>();
```

Lobby Class
```csharp
using Steamworks;

public class Lobby
{
	public string name;
	public string summary;
	public int lobbyIndex;
	public CSteamID lobby;
}

```

This makes accessing lobbies and getting the information you need easy! Note: Lobby.summary isn't filled out sometimes due to the Lobby Owner not setting that property.

