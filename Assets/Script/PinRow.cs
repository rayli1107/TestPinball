using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinRow : MonoBehaviour
{
    public float minX = -2;
    public float maxX = 2;
    public float speed = 1;
    public bool directionRight = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int m = directionRight ? 1 : -1;
        float x = transform.position.x + m * speed * Time.deltaTime;
        if (directionRight && x >= maxX)
        {
            x = maxX;
            directionRight = false;
        } else if (!directionRight && x <= minX)
        {
            x = minX;
            directionRight = true;
        }
        transform.position = new Vector3(
            x, transform.position.y, transform.position.z);        
    }
}
