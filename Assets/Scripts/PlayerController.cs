using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator PlayerAnim;
    public bool isAttacking = false;


    void Start()
    {
        PlayerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleActions();
    }
    private void HandleActions()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            
        }
        // Chute Alto (K)
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerAnim.SetBool("IsAttacking", true);
            PerformBencao();
        }

        /*         // Ponteira - Chute Baixo (S)
                if (Input.GetKeyDown(KeyCode.L) && !isDodging && !isAttacking && gameOver == false)
                {
                    PerformLowKick();
                } */
    }
    private void PerformBencao()
    {
        // mark state and trigger animation logic
        // isAttacking = true;
        PlayerAnim.SetTrigger("Bencao");
    }

}
