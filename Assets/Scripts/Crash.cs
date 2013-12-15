using UnityEngine;
using System.Collections;

public class Crash : MonoBehaviour {
	
	public AudioClip crashClip;
	public Color flashTintColor;
	
	// Use this for initialization
	IEnumerator Start () {
		GameObject crashFX = gameObject;
		crashFX.transform.rotation = transform.rotation;
		crashFX.transform.position = transform.position;
		AudioSource sfxSource = crashFX.AddComponent<AudioSource>();
		sfxSource.clip = crashClip;
		sfxSource.Play();
		//LineRenderer crashLines = crashFX.AddComponent<LineRenderer>();
		MeshFilter crashMeshFilter = crashFX.AddComponent<MeshFilter>();
		Renderer crashRenderer = crashFX.AddComponent<MeshRenderer>();
		Mesh crashMesh = new Mesh();
		crashMeshFilter.mesh = crashMesh;
		int lines = 20*3;
		Vector3[] vertices = new Vector3[lines];
		int[] triangles = new int[lines];
		Vector2[] uv = new Vector2[lines];
		
		/*crashLines.material = new Material (Shader.Find("Particles/Additive"));
		crashLines.useWorldSpace = false;
		crashLines.SetColors(Color.white,Color.white);
		crashLines.SetWidth(0.1f,0.1f);
		crashLines.SetVertexCount(64);
		*/
		
		Destroy( crashFX, crashClip.length );
		
		
		float indexToDeg = 180f/lines;
		
		crashRenderer.material = new Material (Shader.Find("Particles/Additive"));
		crashRenderer.material.SetColor("_TintColor", flashTintColor );
		float startTime = Time.time;
		float endTime = Time.time+0.2f;
		
		float speed = 30f;
		
		GameObject[] bikeBits = new GameObject[lines];
		
		for( int i=0;i<lines;i++){
			bikeBits[i] = CreateBikeBit();
			bikeBits[i].renderer.sharedMaterial = crashRenderer.sharedMaterial;
		}
		
		while( Time.time < endTime ){
			Random.seed = 0;
			crashMesh.Clear();

			for(int i=2;i<lines-4;i+=3){
				float scale = Mathf.Lerp( 0f, 10f, Mathf.InverseLerp( startTime, endTime, Time.time ) );
				//Vector2 outer = Random.insideUnitCircle*0.25f;
				
				
				vertices[i] = 0.03f * ( Quaternion.Euler(0,0,indexToDeg*(i+1)) * Vector3.up );
				vertices[i+1] += speed * Time.deltaTime * Random.Range(0.5f,1f) * ( Quaternion.Euler(0,0,indexToDeg*(i+1+Random.Range(-1f,1f))) * Vector3.right );
				bikeBits[i].transform.localPosition = vertices[i+1];
				
				//+ Vector3.right * outer.x + Vector3.up * outer.y;
				vertices[i+2] = 0.03f * ( Quaternion.Euler(0,0,indexToDeg*(i+1)) * -Vector3.up );
				
				triangles[i] = i;
				triangles[i+1] = i+1;
				triangles[i+2] = i+2;
				
				uv[i] = new Vector2(0,0);
				uv[i+1] = new Vector2(0.5f,0);
				uv[i+2] = new Vector2(1,0);
				
				/*
				crashLines.SetPosition( i, transform.right * inner.x + transform.up * inner.y );
				crashLines.SetPosition( i+1, transform.right * outer.x + transform.up * outer.y );
				*/
				
			}
			
			speed *= 0.93f;
				
			
			crashMesh.vertices = vertices;
			crashMesh.triangles = triangles;
			crashMesh.RecalculateNormals();
			crashMesh.RecalculateBounds();
			yield return null;
		}
		yield return null;
		Destroy( crashRenderer );
		while( true ){
			Random.seed = 0;
			for(int i=2;i<lines-4;i+=3){
				bikeBits[i].transform.localPosition += speed * Time.deltaTime * Random.Range(0.5f,1f) * ( Quaternion.Euler(0,0,indexToDeg*(i+1+Random.Range(-1f,1f))) * Vector3.right );				
			}
			
			speed *= 0.93f;
			yield return null;
				
		}		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	GameObject CreateBikeBit(){
		GameObject gib = new GameObject();
		gib.transform.parent = transform;
		gib.transform.localPosition = Vector3.zero;
		gib.transform.localRotation = Quaternion.identity;
		
		MeshFilter meshFilter = gib.AddComponent<MeshFilter>();
		Mesh mesh = meshFilter.sharedMesh = new Mesh();
		
		float w = Random.Range(0.01f,0.1f);
		float h = Random.Range(0.01f,0.1f);
		Vector3 p0 = new Vector3(-w,-h,0);
		Vector3 p1 = new Vector3(w,-h,0);
		Vector3 p2 = new Vector3(-w,h,0);
		Vector3 p3 = new Vector3(w,h,0);

		mesh.vertices = new Vector3[]{
			p0,p1,p2,p3
		};
		
		mesh.triangles = new int[]{
			0,1,3,
			0,2,3
		};
		
		mesh.uv = new Vector2[]{
			new Vector2(0,0),
			new Vector2(1,0),
			new Vector2(0,1),
			new Vector2(1,1)
		};
		
		mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
		
		gib.AddComponent<MeshRenderer>();
		
		return  gib;
	}
}
