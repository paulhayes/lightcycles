using UnityEngine;
using System.Collections;

public class Crash : MonoBehaviour {
	
	public AudioClip crashClip;
	public Color flashTintColor;
	
	protected const float duration = 0.2f;
	protected const int numOfBikeGibs = 50;
	protected const float startSpeed = 20f;
	protected const float drag = 0.999f;
	protected const int randomSeed = 0;
	
	void Start () {
		GameObject crashFX = gameObject;
		crashFX.transform.rotation = transform.rotation;
		crashFX.transform.position = transform.position;
		AudioSource sfxSource = crashFX.AddComponent<AudioSource>();
		sfxSource.clip = crashClip;
		sfxSource.Play();
		
		/* this effect */
		Destroy( crashFX, crashClip.length + 5f );
		
		float indexToDeg = 180f/numOfBikeGibs;
		
		float startTime = Time.time;
		//float endTime = startTime + duration;
		
		GameObject[] bikeGibs = new GameObject[numOfBikeGibs];
		
		Material crashRendererMaterial = new Material (Shader.Find("Particles/Additive"));
		crashRendererMaterial.SetColor("_TintColor", flashTintColor );

		
		/* Create rectangular bits of bike */
		for(int i=0;i<numOfBikeGibs;i++){
			
			float angleRandomOffset = indexToDeg*Random.Range(-0.5f,0.5f);
			float speed = startSpeed * Random.Range( 0.2f, 1f );
			Vector3 gibDirection = Quaternion.Euler(0,0,indexToDeg*i+angleRandomOffset) * Vector3.right;
			
			bikeGibs[i] = CreateBikeGib();
			
			bikeGibs[i].renderer.sharedMaterial = crashRendererMaterial;
			bikeGibs[i].AddComponent<CrashGib>().Setup( gibDirection.normalized * speed, drag );
			
			Destroy( bikeGibs[i], sfxSource.clip.length + 2f );
		}
	}
	
	/* Create a random rectangle mesh */
	GameObject CreateBikeGib(){
		GameObject gib = new GameObject();
		gib.transform.parent = transform;
		gib.transform.localPosition = Vector3.zero;
		gib.transform.localRotation = Quaternion.identity;
		
		MeshFilter meshFilter = gib.AddComponent<MeshFilter>();
		Mesh mesh = meshFilter.sharedMesh = new Mesh();
		
		float w = Random.Range(0.01f,0.12f);
		float h = Random.Range(0.01f,0.12f);
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
		
		return gib;
	}
}
