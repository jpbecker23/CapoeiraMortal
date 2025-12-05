using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] uninhoPrefabs;
    private float spawnPosX = 4;
    private float spawnPosZ = -40;
    private float startDelay = 5;
    private float spawnInterval = 4f;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnUninho", startDelay, spawnInterval);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnUninho()
    {
        int uninhoIndex = 0;
        Vector3 spawnPos = new Vector3(spawnPosX, 0, spawnPosZ);
        Instantiate(uninhoPrefabs[uninhoIndex], spawnPos, uninhoPrefabs[uninhoIndex].transform.rotation);
    }
}
