using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public float speed = 3f;
	public float maxBoostSpeed = 2f;
	public float boostMaxDistance = 5f;
	public GameObject trailPrefab;
	
	public Camera playerCamera;
	
	protected GameObject currentTrailLine;
	protected int currentLineIndex;
	protected Vector3 currentLineStart;
	protected Vector3 currentLineEnd;
	protected Vector3 lastCurrentLineEnd;
	protected Color playerColor;
	protected KeyCode leftKey;
	protected KeyCode rightKey;
	public float boostSpeed;
	
	protected struct Line{
		public Vector3 start;
		public Vector3 end;
		
		public Line(Vector3 start, Vector3 end){
			this.start = start;
			this.end = end;
		}
	}
	
	
	static protected List<Line> lines;
	
	// Use this for initialization
	void Start () {
		if( lines == null ) lines = new List<Line>();
		CreateTrailLine();
	}
	
	// Update is called once per frame
	void Update () {
		CheckKeys();	
		//LineBoost();
		Move();
		CheckForCollisions();
		UpdateCurrentTailLine();
	}
	
	void SetPlayerKeys( StartScript.Keys playerKeys ){
		leftKey = playerKeys.left;
		rightKey = playerKeys.right;
	}
	
	void SetPlayerCamera( Camera camera ){
		playerCamera = camera;
	}
	
	void SetPlayerColor(Color c){
		playerColor = c;
		
		Renderer renderer = transform.GetComponentInChildren<Renderer>();
		renderer.material.color = playerColor;
				
	}
	
	void CheckKeys(){
		
		bool newTrail = false;
		
		if( Input.GetKeyDown( rightKey ) ){
			transform.forward = transform.right;
			newTrail = true;
		}
		
		if( Input.GetKeyDown( leftKey ) ){
			transform.forward = -transform.right;
			newTrail = true;
		}
		
		if( newTrail ){
			CreateTrailLine();
		}
	}
	
	void CreateTrailLine(){
		currentTrailLine = (GameObject) Instantiate( trailPrefab );
		Color lineColor = playerColor;
		lineColor.a = 0.176f;
		currentTrailLine.transform.GetComponentInChildren<Renderer>().material.SetColor("_TintColor", lineColor );
		currentLineEnd = currentLineStart = transform.position;
		currentTrailLine.transform.forward = transform.forward;
		currentLineIndex = lines.Count;
		lines.Add( new Line( currentLineStart, currentLineEnd ) );
	}
	
	void CheckForCollisions(){
		
		Line current = new Line( lastCurrentLineEnd, currentLineEnd );
		
		for( int i=0; i<lines.Count; i++ ){
			Line line = lines[i];
			if( currentLineIndex == i ) continue;
			
			Vector3 lineRay = line.end - line.start;
			Vector3 startRay = current.start - line.start;
			Vector3 endRay = current.end - line.start;
			
			Vector3 lineNormal = Vector3.Cross( lineRay, Vector3.up );
			Vector3 currentNormal = Vector3.Cross( current.end - current.start, Vector3.up );
			
			
			float currentNormalDotLine = Vector3.Dot( currentNormal, lineRay );
			
			float lineNormalDotStart = Vector3.Dot( lineNormal, startRay );
			float lineNormalDotEnd = Vector3.Dot( lineNormal, endRay );
			
			bool currentCrossesLine = ( Vector3.Dot( lineNormal, startRay ) * Vector3.Dot ( lineNormal, endRay ) ) < 0f;
				
			// Are lines parallel
			if( Mathf.Approximately( currentNormalDotLine, 0f ) ){
				float t0 = Vector3.Dot( lineRay.normalized, endRay ) / lineRay.magnitude;
				Debug.Log( t0 +" "+Vector3.Dot( lineRay.normalized, endRay )+" "+lineRay.magnitude+" "+endRay );
				if( Within( t0, 0f, 1f ) ){
					transform.position = current.end = t0 < 0.5f ? line.start : line.end;
					Debug.Log("parrallel crash");
					Debug.Log( t0 );
					
					
					//Debug.Break();

					Destroy( gameObject );
					return;
				}
			}
			//Have lines crossed
			else if( currentCrossesLine ){
				
				//Is the intersection within the bounds of the line
				float t0 = -Vector3.Dot( currentNormal, -startRay ) / currentNormalDotLine;
				float t1 = Vector3.Dot( lineNormal, -startRay ) / -currentNormalDotLine;
					
				if( Within(t0, 0f, 1f) ){
					
					
					Debug.Log("perpendicular crash");
 					Debug.Log( t0 );
					Debug.Log ( playerColor );
					Debug.DrawRay( current.start, current.end - current.start, Color.grey, 1f ); //v  
					Debug.DrawRay( current.start, currentNormal, Color.yellow, 1f ); //vT  
					Debug.DrawRay( current.start, -startRay, Color.cyan, 1f ); // w
					Debug.DrawRay( line.start, lineRay, Color.green, 1f ); //u
					Debug.DrawRay ( current.start, currentNormal, Color.white );
					//Debug.Break();
					
					transform.position = currentLineEnd = current.end = line.start + t0 * lineRay;
					lines[currentLineIndex] = current;
					
					
					Destroy( gameObject );
					return;
				}
				
			}
			else{
				Debug.Log( lineNormalDotEnd );
			}
		}
	}
	
	void LineBoost(){
		if( currentLineIndex > lines.Count ) return;
		
		Line current = lines[currentLineIndex];
		
		boostSpeed *= 0.995f;
		
		foreach( Line line in lines ){
			if( line.end == current.end ) continue;
			Vector3 a = current.end - line.start;
			Vector3 b = line.end - line.start;
			Vector3 c = b.normalized * Vector3.Dot( a, b.normalized );
			
			float t = c.magnitude / b.magnitude;
			float distance = Vector3.Distance( a, c );
						
			if( t > 0f && t < 1f && 0f < distance && distance < boostMaxDistance ){
				Debug.DrawLine( line.start+a, line.start+c, Color.red, 0.25f );
				Debug.DrawRay( line.start+a, (a-c).normalized, Color.blue, 0.25f );
				boostSpeed += Time.deltaTime * maxBoostSpeed * ( boostMaxDistance - distance ) / boostMaxDistance ;
				
			}
			
			if( boostSpeed > maxBoostSpeed ) boostSpeed = maxBoostSpeed;
		}
		
	}
	

	
	void Move(){
		transform.position += transform.forward * ( speed + boostSpeed ) * Time.deltaTime;
		Line current = lines[currentLineIndex];
		lastCurrentLineEnd = currentLineEnd;
		currentLineEnd = current.end = transform.position;
		lines[currentLineIndex] = current;
	}
	
	void UpdateCurrentTailLine(){
		if( currentTrailLine != null ){ 
			currentTrailLine.transform.localScale = new Vector3( 1f, 1f, Vector3.Distance( currentLineStart, currentLineEnd ) );
			currentTrailLine.transform.position = ( currentLineStart + currentLineEnd ) * 0.5f;
		}
	}
	
	void OnGUI(){
		
		GUI.Label( playerCamera.rect, boostSpeed.ToString() );
	}
	
	protected bool Within( float t, float min, float max ){
		return min < t && t < max;
	}

	
}
