using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	
	public GUILayoutOption layout;
	public GUIStyle title;
	public GUIStyle customButton;
	public Rect singlePlayerButtonPos;
	public Rect multiPlayerButtonPos;
	
	public bool menuEnabled = true;
	public StartScript startScript;
	
	public Rect menuSize;
	
	// Use this for initialization
	void Start () {
		startScript.OnGameEnd += OnGameEnd;
	}
	
	void Update(){
		if( Input.GetKeyDown(KeyCode.Escape) ){
			MenuOn();
		}
	}
	
	void MenuOn(){
		startScript.Pause();
		menuEnabled = true;
	}
	
	void MenuOff(){
		menuEnabled = false;
	}
	
	void OnGUI(){
		if( !menuEnabled ) return;
		
		Rect area = new Rect(0.5f*(Screen.width-menuSize.width),0.5f*(Screen.height-menuSize.height),menuSize.width,menuSize.height);
		Rect buttonRect = new Rect(50,50, menuSize.width, 100);
		
		
		GUILayout.BeginArea( area );
		GUILayout.BeginVertical();
		
		GUILayout.Label("Light Cycles", title );
		
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
