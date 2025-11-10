using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator PlayerAnim;
    public bool isAttacking = false;
    public float speed = 5f;


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
        var horizontalInput = Input.GetAxis("Horizontal");

        transform.Translate(Vector3.forward * speed * horizontalInput * Time.deltaTime);



        if (Input.GetKeyDown(KeyCode.D) && transform.rotation.y != 180)
        {
            transform.Rotate(new Vector3(0, 180, 0));
        }
        else if (Input.GetKeyDown(KeyCode.A) && transform.rotation.y != 0)
        {
            transform.Rotate(new Vector3(0, -180, 0));
        }

        // else if (Input.GetKeyDown(KeyCode.A))
        // {
        //     transform.Translate(Vector3.back * speed * Time.deltaTime);
        // }
        // Chute Alto (K)

        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerAnim.SetBool("IsAttacking", true);
            PerformBencao();
        }

        /*         // Ponteira - Chute Baixo (L)
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
