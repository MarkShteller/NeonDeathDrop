using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager Instance;

    public Slider manaSlider;
    public Slider healthSlider;

	void Awake () {
        Instance = this;
	}

    public void SetMana(float value)
    {
        manaSlider.value = value;
    }

    public void SetHealth(float value)
    {
        healthSlider.value = value;
    }

}
