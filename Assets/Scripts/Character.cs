using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Vector3 fixedPosition;

    // Start is called before the first frame update
    void Start()
    {
        fixedPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        transform.position = fixedPosition;
    }
}