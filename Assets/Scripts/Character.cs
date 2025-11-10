using UnityEngine;

public class Character : MonoBehaviour
{
    private float fixedPosition;

    // Start is called before the first frame update
    void Start()
    {
        fixedPosition = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z != fixedPosition)
            transform.Translate(transform.position.x, transform.position.y, fixedPosition);
    }
}