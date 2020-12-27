using UnityEngine;
using UnityEngine.SceneManagement;
using UI;

public class GameController : MonoBehaviour
{
    public enum GameState
    {
        kCreditCheck,
        kGameStart,
        kScoring
    }

    public GameObject lightPrefab;
    public BallController ball;
    public SpringController springController;
    public ObjectRow lights;

    public bool PCDragInvertY = false;
    public bool MobileDragInvertY = true;
    public float PCDragUnit = 10f;
    public float MobileDragUnit = 20f;

    public SpriteRenderer background;

    public static GameController Instance { get; private set; }

    public float lightEnableChance = 0.3f;

    public int startingCredit = 10;
    private int _credit;
    private GameState _state;
    private System.Random _random;
    private bool _first;

    public void OnEnable()
    {
        _credit = startingCredit;
        _state = GameState.kCreditCheck;
        _first = true;

        ThemeProfile theme = CurrentTheme.theme;
        background.sprite = theme.background;
        ball.GetComponent<SpriteRenderer>().sprite = theme.ball;

        LightController[] lightControllers = lights.GetComponentsInChildren<LightController>();
        foreach (LightController lightController in lightControllers)
        {
            lightController.spriteLightOn = theme.goal;
        }
    }

    private void Awake()
    {
        _random = new System.Random(System.Guid.NewGuid().GetHashCode());
        if (Instance == null)
        {
            Instance = this;
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }

    private void ResetLights()
    {
        LightController[] lightControllers = lights.GetComponentsInChildren<LightController>();
        foreach (LightController lightController in lightControllers)
        {
            lightController.SetState(_random.NextDouble() < lightEnableChance);
        }
    }

    public void AddCredit(int credit)
    {
        _credit += credit;
        GameUIManager.Instance.SetCredit(_credit);
    }

    public void RegisterHit(bool hit)
    {
        if (hit)
        {
            _credit += 2;
            Debug.LogFormat("Hit, Credit: {0}", _credit);
        }
        else
        {
            Debug.LogFormat("Miss, Credit: {0}", _credit);
        }
        _state = GameState.kScoring;
        ball.SetBouncy(false);
    }

    public void OnBallReturn()
    {
        CameraController.Instance.ZoomOut();
        if (_state == GameState.kScoring)
        {
            _state = GameState.kCreditCheck;
            GameUIManager.Instance.SetCredit(_credit);
            GameUIManager.Instance.gameObject.SetActive(true);
        }
        else if (_state == GameState.kGameStart)
        {
            springController.AllowPlay();
        }
    }

    public void StartVideoAd()
    {
        if (_state == GameState.kCreditCheck && _credit == 0)
        {
            AdsManager.Instance.DisplayVideoAd();
        }
    }

    public void StartPlay()
    {
        if (_state == GameState.kCreditCheck && _credit > 0)
        {
            --_credit;
            GameUIManager.Instance.gameObject.SetActive(false);
            _state = GameState.kGameStart;
            ball.SetBouncy(true);
            ResetLights();
            springController.AllowPlay();
            if (GlobalConfig.autoZoom)
            {
                CameraController.Instance.ZoomIn();
            }
        }
    }
/*
    private void ProcessDrag()
    {
        if (isMobile && Input.touchCount != 1)
        {
            return false;
        }

        Vector2 direction = Vector2.zero;
        if (Input.GetMouseButtonDown(0) && !_eventSystem.IsPointerOverGameObject())
        {
            _mousePressed = true;
            _firstMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _mousePressed = false;
        }
        else if (Input.GetMouseButton(0) && !_eventSystem.IsPointerOverGameObject())
        {
            Vector3 diff = Input.mousePosition - _firstMousePosition;

            int my = (isMobile ? MobileDragInvertY : PCDragInvertY) ? -1 : 1;
            float dragUnit = isMobile ? MobileDragUnit : PCDragUnit;
            direction.y = my * diff.y / dragUnit;
        }
    }
    */

    void Update()
    {
        if (_first)
        {
            _first = false;
            GameUIManager.Instance.SetCredit(_credit);
            GameUIManager.Instance.gameObject.SetActive(true);
        }
    }

    public void Quit()
    {
        SceneManager.LoadScene("MenuScene");
    }
}