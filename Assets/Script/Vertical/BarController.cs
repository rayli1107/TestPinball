using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertical
{
    public class DragContext
    {
        public bool active;
        public int id;
        public Vector2 initialPosition;
        public Vector2 lastPosition;
    }

    public class BarController : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private float _dragFactor = 100f;
#pragma warning restore 0649

        private Rigidbody2D _body;
        private DragContext _leftDrag;
        private DragContext _rightDrag;
        private float _midX;

        private void Awake()
        {
            _leftDrag = new DragContext();
            _rightDrag = new DragContext();

            _midX = Screen.safeArea.center.x;
            Debug.LogFormat("mid x {0}", _midX);
        }

        public void Reset()
        {
            _leftDrag.active = false;
            _rightDrag.active = false;
        }

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

        private void ProcessDrag()
        {
            for (int i = 0; i < Input.touchCount; ++i)
            {
                Touch touch = Input.GetTouch(i);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        bool isLeft = touch.position.x < _midX;
                        DragContext dragContext = isLeft ? _leftDrag : _rightDrag;
                        if (dragContext.active == false)
                        {
                            dragContext.active = true;
                            dragContext.id = touch.fingerId;
                            dragContext.initialPosition = touch.position;
                            dragContext.lastPosition = touch.position;
                        }
                        if (isLeft)
                        {
                            Debug.LogFormat("Left Drag Began");
                        }
                        else
                        {
                            Debug.LogFormat("Right Drag Began");
                        }
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (_leftDrag.active && _leftDrag.id == touch.fingerId)
                        {
                            _leftDrag.lastPosition = touch.position;
                        }
                        else if (_rightDrag.active && _rightDrag.id == touch.fingerId)
                        {
                            _rightDrag.lastPosition = touch.position;
                        }
                        break;
                    default:
                        if (_leftDrag.active && _leftDrag.id == touch.fingerId)
                        {
                            Debug.LogFormat("Left Drag Ended");
                            _leftDrag.active = false;
                        }
                        else if (_rightDrag.active && _rightDrag.id == touch.fingerId)
                        {
                            Debug.LogFormat("Right Drag Ended");
                            _rightDrag.active = false;
                        }
                        break;
                }
            }
        }

        private float CalculateDrag(DragContext drag)
        {
            float dy = 0;
            if (drag.active)
            {
                dy = drag.lastPosition.y - drag.initialPosition.y;
                dy = Mathf.Clamp(dy / _dragFactor, -1, 1);
            }
            return dy;
        }

        private void Update()
        {
            ProcessDrag();

            float ldy = getDeltaY(KeyCode.W, KeyCode.S);
            float rdy = getDeltaY(KeyCode.I, KeyCode.K);
            ldy += CalculateDrag(_leftDrag);
            rdy += CalculateDrag(_rightDrag);
            float dy = ldy + rdy;
            float dr = rdy - ldy;
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
}

