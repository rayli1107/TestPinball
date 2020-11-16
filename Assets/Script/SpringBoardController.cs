using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringBoardController : MonoBehaviour
{
    private SpringController _springController;

    private void Awake()
    {
        _springController = GetComponentInParent<SpringController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _springController.OnBallCollision();
    }
}