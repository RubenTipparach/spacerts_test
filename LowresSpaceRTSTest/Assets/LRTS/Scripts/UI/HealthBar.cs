using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : Slider {

	public BasicShip ship;

	Slider sliderVal;

	protected override void Awake()
	{
		sliderVal = GetComponent<Slider>();

        base.Awake();
	}

    // Use this for initialization
    protected override void Start () {
        base.Start();
    }
	
	// Update is called once per frame
	void Update () {
        if (ship != null)
        {
            //sliderVal.value = ship.GetCurrentHealth.AsFloat();
        }
	}


	public void SetInitVal(float val)
	{
		sliderVal.value = val;
	}
}
