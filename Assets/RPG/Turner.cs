using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turner : MonoBehaviour {

    public float speed = 180;
    Vector3 angles;
    void Start()
    {
        angles = transform.eulerAngles;
    }
	// Update is called once per frame
	void Update () {
        transform.eulerAngles = angles + speed * Vector3.up * Time.time;
	}
}
