using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : MonoBehaviour {

    [System.Serializable]
    public struct FatcorySetting
    {
        GameObject prefab;
        int number;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static GameObject GetObject(GameObject prefab, Transform par = null)
    {
        return Instantiate(prefab, par);
    }

    public static void Recycle(GameObject obj)
    {
        Destroy(obj);
    } 
}
