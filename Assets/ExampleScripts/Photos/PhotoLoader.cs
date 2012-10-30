using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections;

public class PhotoLoader : MonoBehaviour {

	public GameObject PhotoPrefab;
	public int MaxPictures = 1;
	
    void Start () 
	{
        StartCoroutine("loadImages");
    }

    IEnumerator loadImages()
    {
        string[] filePaths = Directory.GetFiles(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures), "*.jpg");
		string[] subset = filePaths.Take(MaxPictures).ToArray();
		
		foreach (string file in subset)
		{
        	WWW www = new WWW("file://" + file);
        	yield return www;
			Texture2D tex = www.texture;
			GameObject go = (GameObject)Instantiate(PhotoPrefab, transform.position, transform.rotation);
			go.renderer.material.mainTexture = tex;
		}
    }
}
