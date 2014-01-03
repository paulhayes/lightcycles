using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	
	public GUILayoutOption layout;
	public GUIStyle title;
	public GUIStyle customButton;
	public Rect singlePlayerButtonPos;
	public Rect multiPlayerButtonPos;
	
	public bool menuEnabled = true;
	public bool initMenu = true;
	public StartScript startScript;
	public Rect menuSize;
	
	public Color backgroundColor;
	
	protected Texture2D background;
	protected GUIStyle backgroundStyle;
	
	// Use this for initialization
	void Start () {
		
		background = new Texture2D(1,1,TextureFormat.ARGB32,false);
		background.SetPixel(0,0, backgroundColor );
		background.Apply();
		backgroundStyle = new GUIStyle();
		backgroundStyle.normal.background = background;
		
		startScript.OnGameEnd += OnGameEnd;
	}
	
	void Update(){
		if( !menuEnabled && Input.GetKeyDown(KeyCode.Escape) ){
			MenuOn();
		}
	}
	
	void MenuOn(){
		startScript.Pause();
		menuEnabled = true;
	}
	
	void MenuOff(){
		menuEnabled = false;
		initMenu = false;
	}
	
	void OnGUI(){
		if( !menuEnabled ) return;
				
		Rect area = new Rect(0.5f*(Screen.width-menuSize.width),0.5f*(Screen.height-menuSize.height),menuSize.width,menuSize.height);
		
		if( !initMenu ) GUI.Box( new Rect(0,0,Screen.width,Screen.height), "", backgroundStyle );
		
		GUILayout.BeginArea( area );
		GUILayout.BeginVertical();
		
		GUILayout.Label("Light Cycles", title );
		
		if( !initMenu && GUILayout.Button("Resume", customButton) ){
			startScript.Unpause();
			MenuOff();
		}
		
		if( GUILayout.Button("2 Player", customButton) ){ 
			startScript.StartGame(2);
			MenuOff();
		}
		if( GUILayout.Button("3 Player", customButton) ){ 		
			startScript.StartGame(3);
			MenuOff();
		}
		if( GUILayout.Button("4 Player", customButton) ){ 			
			startScript.StartGame(4);
			MenuOff();
		}
		
		if( GUILayout.Button("Quit", customButton) ){ 			
			Application.Quit();
		}
		
		GUILayout.EndVertical();
		GUILayout.EndArea();

	}
	
	void OnGameEnd(){
		MenuOn();
	}
}
