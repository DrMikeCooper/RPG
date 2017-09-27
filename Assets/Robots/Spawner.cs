using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG;

public class Spawner : MonoBehaviour {

    public GameObject prefab;
    public float frequency;
    public int team;

    float timer;

	// Use this for initialization
	void Start () {
        timer = 3;
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = frequency;
            GameObject go = Instantiate(prefab);
            go.transform.position = transform.position + 0.1f * Vector3.up;
            Character ch = go.GetComponent<Character>();
            if (ch)
                ch.team = team;
            PowerArea.ClearCharacterList();
        }
	}
}
