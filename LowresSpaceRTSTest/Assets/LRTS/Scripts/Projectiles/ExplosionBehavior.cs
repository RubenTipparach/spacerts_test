using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBehavior : MonoBehaviour {

    [SerializeField]
    float timeToLive = 5;

    float timePassed = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        timePassed += Time.deltaTime;

        if (timePassed > timeToLive)
        {
            Destroy(gameObject);
        }
    }
}
