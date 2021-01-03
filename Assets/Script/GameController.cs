using UnityEngine;
using UnityEngine.SceneManagement;
using UI;
using System.Collections.Generic;
using System.Collections;

public class GameController : MonoBehaviour
{
    public enum GameState
    {
        kCreditCheck,
        kSelectMultiplier,
        kGameStart,
        kScoring
    }

#pragma warning disable 0649
    [SerializeField]
    private Vector2Int[] _creditMultiplierList;
    [SerializeField]
    private GameObject _walls;
#pragma warning restore 0649
    public Vector2Int[] creditMultiplierList => _creditMultiplierList;

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

    private int _multiplier;
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

        CameraController.Instance.SetBackgroundColor(theme.backgroundColor);
        foreach (SpriteRenderer sprite in _walls.GetComponentsInChildren<SpriteRenderer>(true))
        {
            sprite.color = theme.wallColor;
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

    private void ResetLights(int count)
    {
        LightController[] lightControllers = lights.GetComponentsInChildren<LightController>();
        HashSet<int> indices = new HashSet<int>();
        for (int i = 0; i < count; ++i)
        {
            while (true)
            {
                int index = _random.Next(lightControllers.Length);
                if (indices.Add(index))
                {
                    break;
                }
            }
        }

        for (int i = 0; i < lightControllers.Length; ++i)
        {
            lightControllers[i].SetState(indices.Contains(i));
        }

        /*
        foreach (LightController lightController in lightControllers)
        {
            lightController.SetState(_random.NextDouble() < lightEnableChance);
        }
        */
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
            _credit += _multiplier;
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
        if (_state == GameState.kScoring)
        {
            _state = GameState.kCreditCheck;
            CameraController.Instance.ZoomOut();
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
            _state = GameState.kSelectMultiplier;
            StartCoroutine(AnimateLights());
//            GameUIManager.Instance.ShowCreditMultiplierPanel();
        }
    }

    public void SelectCreditMultiplier(Vector2Int setting)
    {
        GameUIManager.Instance.EnableButtons(true);
        GameUIManager.Instance.gameObject.SetActive(false);
        _multiplier = setting.y;
        _state = GameState.kGameStart;
        ball.SetBouncy(true);
        ResetLights(setting.x);
        springController.AllowPlay();
        if (GlobalConfig.autoZoom)
        {
            CameraController.Instance.ZoomIn();
        }
    }

    private IEnumerator AnimateLights()
    {
        GameUIManager.Instance.EnableButtons(false);
        LightController[] lightControllers = lights.GetComponentsInChildren<LightController>();
        for (int i = 0; i < 3 * lightControllers.Length; ++i)
        {
            int prev = i % lightControllers.Length;
            int cur = (i + 1) % lightControllers.Length;
            lightControllers[prev].SetState(false);
            lightControllers[cur].SetState(true);
            yield return new WaitForSeconds(0.5f / lightControllers.Length);
        }

        int index = _random.Next(_creditMultiplierList.Length);
        Vector2Int setting = _creditMultiplierList[index];
        GameUIManager.Instance.SetMultiplierText(setting.y);
        yield return new WaitForSeconds(1);
        GameUIManager.Instance.SetMultiplierText(0);
        SelectCreditMultiplier(setting);
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