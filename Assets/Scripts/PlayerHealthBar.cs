using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider slider;

    public void SetPlayerMaxHealth(float playerhealth)
    {
        slider.maxValue = playerhealth;
        slider.value = playerhealth;
    }

    public void SetPlayerHealth(float playerhealth)
    {
        slider.value = playerhealth;

    }
}
