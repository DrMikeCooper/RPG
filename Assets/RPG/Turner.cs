using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turner : MonoBehaviour {

    public float speed = 180;

	// Update is called once per frame
	void Update () {
        transform.eulerAngles = speed * Vector3.up * Time.time;
	}
}
