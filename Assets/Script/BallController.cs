using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public PhysicsMaterial2D materialBouncy;
    public PhysicsMaterial2D materialHeavy;

    private Rigidbody2D _rigidBody;

    private void Awake()
    {
        Debug.Log("BallController awake");
        _rigidBody = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetBouncy(bool bouncy)
    {
        _rigidBody.sharedMaterial = bouncy ? materialBouncy : materialHeavy;
    }
}
