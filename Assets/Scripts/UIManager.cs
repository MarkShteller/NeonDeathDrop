using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Slider manaSlider;
    public Slider healthSlider;

    /*
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    */

    public void SetMana(float value)
    {
        manaSlider.value = value;
    }

    public void SetHealth(float value)
    {
        healthSlider.value = value;
    }

}
