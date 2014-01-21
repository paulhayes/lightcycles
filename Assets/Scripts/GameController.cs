using UnityEngine;
using System.Collections;
using System;

public class GameController : MonoBehaviour {
	
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
	
	public KeyCode restartKey;
	
	public GUIStyle startStyle;
	public string restartText;
	
	public float gameRestartTextDelay;
	public float gameRestartTextFadeDuration;
	
	public bool isGameEnded;
	public float gameEndedStartTime;
	public Color backgroundColor;
	
	protected Color textColor;
	protected Texture2D background;
	
	[HideInInspector]
	public bool paused = true;
	
	[System.Serializable]
	public class Keys {
		public KeyCode left;
		public KeyCode right;
	};
	
	public Keys[] playerKeys;
	
	protected int remainingPlayers = 4;
	
	void Start(){
		textColor = startStyle.normal.textColor;
		background = new Texture2D(1,1,TextureFormat.ARGB32,false);
		background.wrapMode = TextureWrapMode.Repeat;
		startStyle.normal.background = background;
	}
	
	public void StartGame (int numberOfPlayers) {
		isGameEnded = false;
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
				
				GameObject player = (GameObject)Instantiate( playerPrefab, spawnPositions[playerIndex].position, spawnPositions[playerIndex].rotation );
				GameObject playerCamera = (GameObject)Instantiate( playerCameraPrefab );
			
				PlayerSettings playerSettings = new PlayerSettings();
				playerSettings.keys = playerKeys[playerIndex];
				playerSettings.playerColor = playerColors[playerIndex];
				playerSettings.player = playerIndex;
				
				player.SendMessage("SetGameController", this);
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
		isGameEnded = false;
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
		//StopAllCoroutines();
		paused = true;
	}
	
	public IEnumerator EndOfGameTest(){
		
		//Wait to see if anyone else crashes
		yield return new WaitForSeconds(0.1f);
		
		if( remainingPlayers == 1 ) {
			OnWin();
	
			StartCoroutine( GameEnded() );
			

		}
		else if( remainingPlayers == 0 ){
			OnDraw();
			
			StartCoroutine( GameEnded() );
			
		}
	}
	
	IEnumerator GameEnded(){
		paused = true;
		gameEndedStartTime = Time.time;
		isGameEnded = true;
			
		yield return new WaitForSeconds(0.5f);
		while( !Input.GetKeyDown( restartKey ) ){
			yield return null;
		}
		
		GameReset();
	}
	
	
	
	void OnGUI(){
		if( isGameEnded ){
			
			float t = Mathf.InverseLerp( gameRestartTextDelay, gameRestartTextDelay+gameRestartTextFadeDuration, Time.time - gameEndedStartTime );
			
			startStyle.normal.textColor = Color.Lerp( Color.clear, textColor, t );
			Color currentBackgroundColor = Color.Lerp( Color.clear, backgroundColor, t );
			
			background.SetPixel( 0, 0, currentBackgroundColor );
			background.Apply();
			
			Rect rect = new Rect( 0, 0, Screen.width, Screen.height );
		
			GUI.Label( rect, restartText, startStyle );
			
		}
		
	}
	
}
