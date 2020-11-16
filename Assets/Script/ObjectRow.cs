using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRow : MonoBehaviour
{
    public GameObject prefab;
    public Vector2 startLocation;
    public Vector2 delta;
    public int count;

    private void Awake()
    {
        for (int i = 0; i < count; ++i)
        {
            GameObject obj = Instantiate(prefab, transform);
            float x = startLocation.x + i * delta.x;
            float y = startLocation.y + i * delta.y;
            obj.transform.localPosition = new Vector3(x, y, 0);
        }
    }
}