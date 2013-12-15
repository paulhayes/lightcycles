using UnityEngine;
using System.Collections;
using System;

public class StartScript : MonoBehaviour {
	
	public event Action OnGameStart;
	public event Action OnGameEnd;
	public event Action OnGameReset;
	public event Action OnWin;
	public event Action OnDraw;

	public int numberOfPlayers;
	public Color[] playerColors;
	
	public GameObject playerPrefab;
	public GameObject playerCameraPrefab;
	public Transform[] spawnPositions;
	
	[System.NonSerialized]
	public bool paused = true;
	
	[System.Serializable]
	public class Keys {
		public KeyCode left;
		public KeyCode right;
	};
	
	public Keys[] playerKeys;
	
	protected int remainingPlayers = 4;
	
	void Start(){
		
	}
	
	public void StartGame (int numberOfPlayers) {
		
		this.numberOfPlayers = numberOfPlayers;
		
		int rows = (int)Mathf.Ceil( Mathf.Sqrt( numberOfPlayers ) );
		int cols = (int)Mathf.Ceil( 1f * numberOfPlayers / rows );
		
		float cameraViewPortWidth = 1f / cols;
		float cameraViewPortHeight = 1f / rows;
		
		int playerIndex = 0;
		remainingPlayers = numberOfPlayers;
		
		OnGameStart();
		
		for( int i=0; i<cols; i++ ){
			for( int j=0; j < rows && playerIndex<numberOfPlayers; j++ ){
				Debug.Log (i+", "+j);
				GameObject player = (GameObject)Instantiate( playerPrefab, spawnPositions[playerIndex].position, spawnPositions[playerIndex].rotation );
				GameObject playerCamera = (GameObject)Instantiate( playerCameraPrefab );
			
				PlayerSettings playerSettings = new PlayerSettings();
				playerSettings.keys = playerKeys[playerIndex];
				playerSettings.playerColor = playerColors[playerIndex];
				playerSettings.player = playerIndex;
				
				player.SendMessage("SetStartScript", this);
				player.SendMessage("SetPlayerCamera", playerCamera.camera);
				player.SendMessage("Settings", playerSettings);
				playerCamera.SendMessage("SetTarget", player );
				
				Rect cameraRect = new Rect( i*cameraViewPortWidth, (1f - j)*cameraViewPortHeight, cameraViewPortWidth, cameraViewPortHeight );
				
				playerCamera.camera.rect = cameraRect;
				
				playerIndex++;
				
			}
		}
		
		Invoke("Unpause",0.5f);
		
	}
	
	public void GameReset(){
		remainingPlayers = numberOfPlayers;
		if( OnGameReset != null ) OnGameReset();
		
		Invoke( "Unpause", 0.5f );

	}
	
	
	public void PlayerCrashed(){
		remainingPlayers--;
		StopCoroutine("EndOfGameTest");
		StartCoroutine("EndOfGameTest");
	}
	
	public void Unpause(){
		paused = false;
	}
	
	public void Pause(){
		StopAllCoroutines();
		paused = true;
	}
	
	public IEnumerator EndOfGameTest(){
		
		//Wait to see if anyone else crashes
		yield return new WaitForSeconds(0.1f);
		
		Debug.Log ( String.Format("Remaining Players {0}",remainingPlayers));
		
		if( remainingPlayers == 1 ) {
			paused = true;
			OnWin();
			
			yield return new WaitForSeconds(0.5f);
			bool lastKeyState = Input.anyKey;
			while( true ){
				if( lastKeyState && !Input.anyKey ) break;
				lastKeyState = Input.anyKey;
				yield return null;
			}
			
			GameReset();
			

		}
		else if( remainingPlayers == 0 ){
			paused = true;
			OnDraw();
			
			while( !Input.anyKeyDown ){
				yield return null;
			}
			
			GameReset();
			
		}
	}
	
}
