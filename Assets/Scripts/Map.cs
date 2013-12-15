using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {
	
	public static Color EMPTY = new Color(0,0,0,1);

	
	public int size;
	public float worldSize;
	
	[HideInInspector]
	public Matrix4x4 mapToWorld = new Matrix4x4();
	[HideInInspector]
	public Texture2D map;

	void Start () {
		
		float scale = worldSize / size;
		
		mapToWorld.SetTRS( new Vector3(-worldSize/2,0,-worldSize/2), Quaternion.identity, Vector3.one * scale );
		
		map = new Texture2D(size,size,TextureFormat.RGB24,false);
		
		StartScript startScript = (StartScript)Object.FindObjectOfType(typeof( StartScript ) );
		startScript.OnGameStart += OnGameStart;
		startScript.OnGameReset += OnGameStart;
	}
	
	void OnGameStart(){
		Color[] c = new Color[size*size];
		for( int i=0;i<size*size;i++) c[i] = EMPTY;
		map.SetPixels(0,0,size,size,c);
		map.Apply();
		
	}
	
	
}
