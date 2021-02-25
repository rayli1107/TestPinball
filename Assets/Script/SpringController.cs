using UnityEngine;
using UnityEngine.EventSystems;

public class SpringController : MonoBehaviour
{
    public enum SpringState
    {
        kWaitingForBall,
        kWaitingForCredit,
        kWaitingForPress,
        kPressing,
        kReleased,
        kRising,
        kFalling
    }

    public float tension = -10;
    public float maxDistance = -2;
    public float delta = -1;

    public Rigidbody2D springBoard;
    public Rigidbody2D ball;

    public bool PCDragInvertY = false;
    public bool MobileDragInvertY = true;
    public float PCDragUnit = 10f;
    public float MobileDragUnit = 20f;


    private SpringState _state;

    private CircleCollider2D _boardCircleCollider;
    private CircleCollider2D _ballCircleCollider;
    private float _boardMass;
    private Vector3 _firstMousePosition;
    private bool _mousePressed;
    private EventSystem _eventSystem;

    //    private Rigidbody2D _rigidBody;
    private bool isMobile
    {
        get
        {
            return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
        }
    }

    private void Awake()
    {
        _boardCircleCollider = springBoard.GetComponent<CircleCollider2D>();
        _ballCircleCollider = ball.GetComponent<CircleCollider2D>();
        _eventSystem = EventSystem.current;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        _state = SpringState.kWaitingForCredit;
        ResetSpringBoard();
        AssignBall();
    }

    public void ResetSpringBoard()
    {
        springBoard.transform.localPosition = Vector3.zero;
        springBoard.velocity = Vector2.zero;
        springBoard.isKinematic = true;
    }

    private void SetBallPosition()
    {
        float boardHeight = springBoard.transform.localScale.y / 2;
        float ballRadius = ball.transform.localScale.y / 2;
        float y = springBoard.transform.localPosition.y;
        y += boardHeight + ballRadius + 0.01f;
        ball.transform.localPosition = new Vector3(0, y, 0);
    }

    public void AssignBall()
    {
        ball.transform.SetParent(transform);
        ball.velocity = Vector2.zero;
        ball.angularVelocity = 0;
        SetBallPosition();
        ball.isKinematic = true;
    }

    public void UnassignBall()
    {
        ball.transform.parent = null;
        ball.isKinematic = false;
    }

    public void AllowPlay()
    {
        if (_state == SpringState.kWaitingForCredit)
        {
            _state = SpringState.kWaitingForPress;
        }
    }

    public void OnBallCollision()
    {
        if (_state == SpringState.kWaitingForBall)
        {
            _state = SpringState.kWaitingForCredit;
            AssignBall();
            GameController.Instance.OnBallReturn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (_state)
        {
            case SpringState.kWaitingForPress:
                if (isMobile && Input.touchCount != 1)
                {
                    break;
                }

                if (Input.GetMouseButtonDown(0) &&
                    !_eventSystem.IsPointerOverGameObject())
                {
                    _state = SpringState.kPressing;
                    _firstMousePosition = Input.mousePosition;
                }
                break;
            case SpringState.kPressing:
                if (isMobile && Input.touchCount != 1)
                {
                    break;
                }

                if (Input.GetMouseButton(0) &&
                    !_eventSystem.IsPointerOverGameObject())
                {
                    Vector3 diff = Input.mousePosition - _firstMousePosition;
                    int my = (isMobile ? MobileDragInvertY : PCDragInvertY) ? -1 : 1;
                    float dragUnit = isMobile ? MobileDragUnit : PCDragUnit;
                    float y = my * diff.y / dragUnit;
                    y = Mathf.Clamp(y, maxDistance, 0);
                    springBoard.transform.localPosition = new Vector3(0, y, 0);
                    SetBallPosition();
                }
                else
                {
                    UnassignBall();
                    springBoard.isKinematic = false;
                    _state = SpringState.kReleased;
                }
                /*
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    UnassignBall();
                    springBoard.isKinematic = false;
                    _state = SpringState.kReleased;
                }
                else
                {
                    float y = springBoard.transform.localPosition.y;
                    y += delta * Time.deltaTime;
                    y = Mathf.Max(y, maxDistance);
                    springBoard.transform.localPosition = new Vector3(0, y, 0);
                    SetBallPosition();
                }*/
                break;
            case SpringState.kReleased:
                AddTensionToBoard();
                if (springBoard.velocity.y > 0)
                {
                    _state = SpringState.kRising;
                }
                break;
            case SpringState.kRising:
                AddTensionToBoard();
                if (springBoard.velocity.y < 0)
                {
                    _state = SpringState.kFalling;
                }
                break;
            case SpringState.kFalling:
                if (springBoard.transform.localPosition.y <= 0)
                {
                    _state = SpringState.kWaitingForBall;
                    ResetSpringBoard();
                }
                else
                {
                    AddTensionToBoard();
                }
                break;
            default:
                break;
        }

        /*
        if (_rigidBody.isKinematic)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                float y = transform.localPosition.y + delta * Time.deltaTime;
                y = Mathf.Max(y, maxDistance);
                transform.localPosition = new Vector3(0, y, 0);
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                _rigidBody.isKinematic = false;
            }
        }

        if (!_rigidBody.isKinematic)
        {
            Vector2 a = new Vector2(0, transform.localPosition.y * tension);
            _rigidBody.AddForce(a * _rigidBody.mass, ForceMode2D.Impulse);

            if (_rigidBody.velocity.y < 0 &&
                transform.localPosition.magnitude < 0.1)
            {
                _rigidBody.isKinematic = true;
                transform.localPosition = Vector3.zero;
            }
        }
        */
    }

    private void AddTensionToBoard()
    {
        Vector2 a = new Vector2(
            0, springBoard.transform.localPosition.y * tension);
        springBoard.AddForce(a * springBoard.mass, ForceMode2D.Impulse);
    }
}
