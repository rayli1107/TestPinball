using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    /*
    public Rigidbody2D _sliderLeft;
    public Rigidbody2D _sliderRight;
/*
    private void AdjustSlider(Rigidbody2D slider, KeyCode up, KeyCode down)
    {
        int dy = 0;
        dy += Input.GetKey(up) ? 1 : 0;
        dy -= Input.GetKey(down) ? 1 : 0;
        slider.velocity = new Vector2(0, dy);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        AdjustSlider(_sliderLeft, KeyCode.W, KeyCode.S);
        AdjustSlider(_sliderRight, KeyCode.I, KeyCode.K);
    }
    */

    public Transform _leftTip;
    public Transform _rightTip;
    private Rigidbody2D _body;
    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    private int getDeltaY(KeyCode up, KeyCode down)
    {
        int dy = 0;
        dy += Input.GetKey(up) ? 1 : 0;
        dy -= Input.GetKey(down) ? 1 : 0;
        return dy;
    }

    private void Update()
    {
        int ldy = getDeltaY(KeyCode.W, KeyCode.S);
        int rdy = getDeltaY(KeyCode.I, KeyCode.K);
        int dy = ldy + rdy;
        int dr = rdy - ldy;
        _body.velocity = new Vector2(0, dy);
        if ((dr > 0 && transform.rotation.z < 0.1f) ||
            (dr < 0 && transform.rotation.z > -0.1f))
        {
            _body.angularVelocity = 15 * dr;
        }
        else
        {
            _body.angularVelocity = 0;
        }
    }
}
