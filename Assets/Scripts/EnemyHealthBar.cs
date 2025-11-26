using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{

    public Slider slider;

    public void SetEnemyMaxHealth(float enemyhealth)
    {
        slider.maxValue = enemyhealth;
        slider.value = enemyhealth;
    }

    public void SetEnemyHealth(float enemyhealth)
    {
        slider.value = enemyhealth;

    }
}
