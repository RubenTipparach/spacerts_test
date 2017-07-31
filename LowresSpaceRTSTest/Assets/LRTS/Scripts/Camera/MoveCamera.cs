using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour {

    private Vector3 newLocation;

    [SerializeField]
    float speed = 20;

	// Use this for initialization
	void Start () {
        newLocation = transform.position;
        //Debug.Log(newLocation);
    }
	
	// Update is called once per frame
	void Update () {
        newLocation += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.position = Vector3.Lerp( transform.position, newLocation, Time.deltaTime * 50);
	}
}
