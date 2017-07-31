using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMotion : MonoBehaviour {

    [SerializeField]
    float Speed = 5;

    GameObject target;

    float enemyDistance;

    [SerializeField]
    GameObject miniExplosion;

    [SerializeField]
    float timeToLive = 5;

    float timePassed = 0;
	// Use this for initialization
	void Start () {
		
	}

    public void SetTarget(float enemyDist, GameObject trg)
    {
        target = trg;
        enemyDistance = enemyDist;
    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.up* Speed * Time.deltaTime, Space.Self);

        timePassed += Time.deltaTime;

        if(timePassed > timeToLive || 
            (target != null && Vector3.Distance(target.transform.position, transform.position) < enemyDistance))
        {
            Destroy(gameObject);

            //spawn explosion too!
            GameObject.Instantiate(miniExplosion, transform.position, Quaternion.identity);
        }
	}
}
