using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFoward : MonoBehaviour
{
    public float speed = 40.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //nesse caso down vai para  a direita do mapa 3, pois o mapa esta rotacionando no plano do Unity e os inputs trocados
        transform.Translate(Vector3.down * Time.deltaTime * speed);
    }
}
