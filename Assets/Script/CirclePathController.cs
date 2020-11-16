using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclePathController : MonoBehaviour
{
    public Rigidbody2D ball;
    public GameObject prefabWall;
    public int sides = 16;
    public float startAngleDegree = 90;
    public float endAngleDegree = 180;
    private BoxCollider2D _boxColldier;
    private Vector2 _ballVelocity;
    private float _ballAngularVelocity;
    private float _currentRadian;
    private float _radius;
    private System.Random _random;

    void Start()
    {
        float startAngle = startAngleDegree * Mathf.Deg2Rad;
        float endAngle = endAngleDegree * Mathf.Deg2Rad;
        float deltaAngle = (endAngle - startAngle) / sides;
        Vector3 localScale = new Vector3(0.02f, deltaAngle, 1);

        for (int i = 0; i < sides; ++i)
        {
            float radius = 1f;
            float angle = startAngle + (i + 0.5f) * deltaAngle;
            GameObject wall = Instantiate(prefabWall, transform);
            wall.transform.localPosition = new Vector3(
                Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            wall.transform.localScale = localScale;
            wall.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);
            wall.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
