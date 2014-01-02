using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum Result{
	WIN,
	LOSE,
	DRAW
}

public class Player : MonoBehaviour {
	
	public AudioClip turnClip;
	public AudioClip startClip;
	
	public GameObject trailPrefab;
	public GameObject crashPrefab;
	public GUIStyle scoreStyle;
	public Transform rearWheel;
	
	[HideInInspector]
	public StartScript startScript;
	
	public event Action OnCrashed;
	
	protected Camera playerCamera;
	protected Map map;
	protected PlayerSettings settings;
	
	protected GameObject currentTrailLine;
	protected Vector3 currentLineStart;
	protected Vector3 currentLineEnd;

	protected int northPosition;
	protected int eastPosition;
	protected int northMovement;
	protected int eastMovement;
	
	protected int score = 0;
	protected bool dead = false;
	protected bool gameOver = false;
	protected Result result;
	
	protected Vector3 startPosition;
	protected Quaternion startRotation;
	
	
	protected static Dictionary<Result, string> resultMessages = new Dictionary<Result, string> {
	    { Result.WIN, "Win" },
		{ Result.LOSE, "Lose" },
		{ Result.DRAW, "Draw" }
	};
	
	void Start () {
				
		startPosition = transform.position;
		startRotation = transform.rotation;
		
		map = ((Map)UnityEngine.Object.FindObjectOfType(typeof( Map ) ));
		
		gameObject.AddComponent<AudioSource>();
		
		Init();
		

	}
	
	void Init(){
		northMovement = Mathf.RoundToInt( transform.forward.z );
		eastMovement = Mathf.RoundToInt( transform.forward.x );
		
		Vector3 pos = map.mapToWorld.inverse.MultiplyPoint( transform.position );
				
		northPosition = Mathf.RoundToInt( pos.z );
		eastPosition = Mathf.RoundToInt( pos.x );
		
		CreateTrailLine();
		
		audio.clip = startClip;
		audio.PlayDelayed(0.5f);
			
	}
	
	void Update () {
		if( startScript.paused || dead ) return;
		
		CheckKeys();
		Move();
		UpdateBike();
		UpdateCurrentTailLine();
		CheckForCollisions();
	}
	
	void CheckKeys(){
		
		bool newTrail = false;
		
		if( Input.GetKeyDown( settings.keys.left ) ){
			int n = northMovement;
			northMovement = eastMovement;
			eastMovement = -n;
			
			transform.forward = new Vector3(eastMovement,0,northMovement);
			
			newTrail = true;
		}
		else if( Input.GetKeyDown( settings.keys.right ) ){
			int n = northMovement;
			northMovement = -eastMovement;
			eastMovement = n;
			transform.forward = new Vector3(eastMovement,0,northMovement);
			
			newTrail = true;
		}
		
		if( newTrail ){
			
			CreateTrailLine();
		}
	}
	
	void CreateTrailLine(){
		
		audio.clip = turnClip;
		audio.Play();
		
		if( currentTrailLine ){
			currentLineEnd = transform.position;
			UpdateCurrentTailLine();
		}
		
		currentTrailLine = (GameObject) Instantiate( trailPrefab );
		currentTrailLine.AddComponent<DestroyOnGameStart>();
		Color lineColor = settings.playerColor;
		lineColor.a = 0.176f;
		currentTrailLine.transform.GetComponentInChildren<Renderer>().material.SetColor("_TintColor", lineColor );
		currentLineStart = currentLineEnd = transform.position;
		
		currentTrailLine.transform.forward = transform.forward;
	}
	
	void Move(){
		northPosition += northMovement;
		eastPosition += eastMovement;
		
	}
	
	void CheckForCollisions(){
		if( northPosition <= 0 || northPosition > map.size || eastPosition <= 0 || eastPosition > map.size ) {
			Crashed();
			return;
		}
		
		Color currentPosition = map.map.GetPixel( eastPosition, northPosition );
		if( currentPosition == Map.EMPTY ){
			map.map.SetPixel( eastPosition, northPosition, settings.playerColor );
		}
		else {
			Crashed();
		}
	}
	
	void UpdateBike(){
		Vector3 pos = transform.position;
		Vector3 newPos = map.mapToWorld.MultiplyPoint( new Vector3( eastPosition, 0, northPosition ) );
		
		pos.z = newPos.z;
		pos.x = newPos.x;
		
		transform.position = pos;
		currentLineEnd = rearWheel.position;

	}
	
	void UpdateCurrentTailLine(){
		if( currentTrailLine != null ){ 
			currentTrailLine.transform.localScale = new Vector3( 1f, 1f, Vector3.Distance( currentLineStart, currentLineEnd ) );
			currentTrailLine.transform.position = ( currentLineStart + currentLineEnd ) * 0.5f;
		}
	}
			
	void Settings( PlayerSettings settings ){
		this.settings = settings;
		
		SetPlayerColor( settings.playerColor );
		
	}

	void SetPlayerCamera( Camera camera ){
		playerCamera = camera;
	}

	void SetPlayerColor(Color c){
		
		Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
		foreach( Renderer renderer in renderers ) renderer.material.color = c;
				
	}
	
	void SetStartScript(StartScript s){
		startScript = s;
		startScript.OnWin += OnWin;
		startScript.OnDraw += OnDraw;
		startScript.OnGameReset += OnGameReset;
		startScript.OnGameStart += OnGameStart;
	}
	
	void Crashed(){
		Instantiate( crashPrefab, rearWheel.position, transform.rotation );
		
		currentLineEnd = transform.position;
		UpdateCurrentTailLine();
		
		startScript.PlayerCrashed();
		
		dead = true;
	
		SetVisibility( false );
		
		if( OnCrashed != null ) OnCrashed();
	}
	
	void SetVisibility(bool value){
		foreach( Renderer renderer in GetComponentsInChildren<Renderer>() ){
			renderer.enabled = value;
		}
	}
	
	void OnWin(){
		gameOver = true;
		result = dead ? Result.LOSE : Result.WIN;
		if( result == Result.WIN ) score++;
	}
	
	void OnDraw(){
		gameOver = true;
		result = Result.DRAW;
	}
	
	void OnGameReset(){
		SetVisibility( true );
		gameOver = false;
		dead = false;
		transform.position = startPosition;
		transform.rotation = startRotation;
		
		Init();
	}
	
	void OnGameStart(){
		
		startScript.OnWin -= OnWin;
		startScript.OnDraw -= OnDraw;
		startScript.OnGameReset -= OnGameReset;
		startScript.OnGameStart -= OnGameStart;

		
		Destroy( gameObject );
	}
	
	void OnGUI(){
		GUI.Label( new Rect(
			10+Screen.width*playerCamera.rect.x,
			10+Screen.height*( playerCamera.rect.height-playerCamera.rect.y ),
			100,
			100), score.ToString(), scoreStyle );
		
		if( gameOver ){
			Rect rect = new Rect(
				Screen.width*playerCamera.rect.x,
				10+Screen.height*( playerCamera.rect.height-playerCamera.rect.y ),
				Screen.width*playerCamera.rect.width,
				Screen.height*playerCamera.rect.height
			);
			
			
			
			GUI.Label( rect, resultMessages[result], scoreStyle );		
		}
	}
	
}
