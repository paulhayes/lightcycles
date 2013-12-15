using UnityEngine;
using System.Collections;

public class WallTexture : MonoBehaviour {
	
	public Color foreground;
	public Color background;
	public int minLength;
	public int maxLength;
	public int totalLineLength;
	public int seed;

	public int size;
	
	// Use this for initialization
	void Start () {
		
		if( renderer.material.mainTexture == null ){
			GenerateTexture();
		}
		
		Object.Destroy( this );
	}
	
	void Update(){
		GenerateTexture();
	}
	
	void GenerateTexture(){
		
		Random.seed = seed;
		Texture2D tex = new Texture2D(size,size,TextureFormat.RGB24, false, false );
		tex.filterMode = FilterMode.Point;
		int vx = 1;
		int vy = 0;
		int x = 0;
		int y = 0;
		int nextChange = 0;
		int vxTmp = vx;
		
		Color[] fill = new Color[size*size];
		for(int i=size*size;i>0;i--) fill[i-1] = background;
		tex.SetPixels(fill);
		
		for( int i = 0; i < totalLineLength ; i++ ){
			if( i == nextChange ){
				nextChange = i + Random.Range(minLength,maxLength);
				if( Random.value < 0.5f ){
					vxTmp = vx;
					vx = vy;
					vy = -vxTmp;
				} 
				else {
					vxTmp = vx;
					vx = -vy;
					vy = vxTmp;
				}
			}
			
			x+=vx;
			y+=vy;
			
			if( x > size ) x = 0;
			if( y > size ) y = 0;
			if( x < 0 ) x = size;
			if( y < 0 ) y = size;
			
			tex.SetPixel(x,y,foreground);
		}
		


		vx = -(int)Mathf.Sign(x);
		vy = -(int)Mathf.Sign(y);
		for( int i = Mathf.Max( x, y ); i>=0; i-- ) {
			if( x!=0 ) x+=vx;
			if( y!=0 ) y+=vy;
			tex.SetPixel(x,y,foreground);
		}
		
		tex.Apply();
		
		renderer.material.mainTexture = tex;
	}
	
	
}
