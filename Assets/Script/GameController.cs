using UnityEngine;
using UnityEngine.SceneManagement;
using UI;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public enum GameState
{
    kInit,
    kWaitForPlay,
    kSelectMultiplier,
    kWaitForGameStart,
    kGameInit,
    kGameStart,
    kScoring
}

public class GameController : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private Vector2Int[] _creditMultiplierList;
    [SerializeField]
    private GameObject _walls;
    [SerializeField]
    private bool _autoStartGame = true;
    [SerializeField]
    private TimeSpan _updateLightInterval = TimeSpan.FromMilliseconds(100);
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

    private Vector3 _initialBallPosition;

    public static GameController Instance { get; private set; }

    public float lightEnableChance = 0.3f;

    private int _multiplier;

    public Action stateUpdateActions;
    private GameState _state;
    public GameState state
    {
        get => _state;
        private set
        {
            if (_state != GameState.kWaitForPlay &&
                value == GameState.kWaitForPlay)
            {
                foreach (LightController light in lights.GetComponentsInChildren<LightController>())
                {
                    light.SetState(LightType.NONE);
                }
            }
            else if (_state != GameState.kSelectMultiplier &&
                     value == GameState.kSelectMultiplier)
            {
                UpdateLightSetting(DateTime.Now);
            }

            _state = value;
            stateUpdateActions?.Invoke();
        }
    }

    private System.Random _random;

    private int _selectedCreditMultiplierIndex;
    private DateTime _nextLightUpdateTime;
    private EventSystem _eventSystem;

    public void OnEnable()
    {
        if (!GlobalGameContext.initialized)
        {
            Quit();
            return;
        }

        state = GameState.kInit;

        ThemeProfile theme = GlobalGameContext.currentTheme;
        background.sprite = theme.background;
        ball.GetComponent<SpriteRenderer>().sprite = theme.ball;

        CameraController.Instance.SetBackgroundColor(theme.backgroundColor);
        foreach (SpriteRenderer sprite in _walls.GetComponentsInChildren<SpriteRenderer>(true))
        {
            sprite.color = theme.wallColor;
        }
    }

    private void Awake()
    {
        _random = new System.Random(System.Guid.NewGuid().GetHashCode());
        _eventSystem = EventSystem.current;
        if (Instance == null)
        {
            Instance = this;
            Screen.orientation = ScreenOrientation.Portrait;
        }

        _initialBallPosition = ball.transform.position;
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

        int keyIndex;
        do
        {
            keyIndex = _random.Next(lightControllers.Length);
        } while (indices.Contains(keyIndex));

        for (int i = 0; i < lightControllers.Length; ++i)
        {
            if (indices.Contains(i))
            {
                lightControllers[i].SetState(LightType.GOAL);
            }
            else if (i == keyIndex)
            {
                lightControllers[i].SetState(LightType.KEY);
            }
            else
            {
                lightControllers[i].SetState(LightType.NONE);
            }
        }
    }
/*
    public void AddCredit(int credit)
    {
        GlobalGameContext.credits += credit;
        GameUIManager.Instance.RefreshStats();
    }
    */

    private void OnHitAnimationFinish()
    {
        ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ball.transform.position = _initialBallPosition + 2 * Vector3.up;
        ball.gameObject.SetActive(true);
    }

    public void RegisterHit(LightType lightType)
    {
        switch (lightType)
        {
            case LightType.GOAL:
                Debug.LogFormat("Hit, Credit: {0}", GlobalGameContext.credits);

                ball.gameObject.SetActive(false);
                GameUIManager.Instance.ShowGoalAnimation(
                    () => ++GlobalGameContext.credits, OnHitAnimationFinish, _multiplier);
                break;
            case LightType.KEY:
                GlobalGameContext.keys++;
                Debug.LogFormat("Hit, Key: {0}", GlobalGameContext.keys);

                ball.gameObject.SetActive(false);
                GameUIManager.Instance.ShowKeyAnimation(OnHitAnimationFinish);
                break;
            default:
                Debug.LogFormat("Miss, Credit: {0}", GlobalGameContext.credits);
                break;
        }
        state = GameState.kScoring;
        ball.SetBouncy(false);
    }

    public void OnBallReturn()
    {
        if (state == GameState.kScoring)
        {
            state = GameState.kWaitForPlay;
            CameraController.Instance.ZoomOut();
        }
        else if (state == GameState.kGameStart)
        {
            springController.AllowPlay();
        }
    }

    public void StartVideoAd()
    {
        if (state == GameState.kWaitForPlay && GlobalGameContext.credits == 0)
        {
            AdsManager.Instance.DisplayVideoAd();
        }
    }

    public void StartPlay()
    {
        if (state == GameState.kWaitForPlay && GlobalGameContext.credits > 0)
        {
//            --GlobalGameContext.credits;
            state = GameState.kSelectMultiplier;
//            StartCoroutine(AnimateLights());
        }
    }

    public void CancelCreditMultiplierSelect()
    {
        if (state == GameState.kSelectMultiplier)
        {
            GameUIManager.Instance.SetMultiplierText(0);
            state = GameState.kWaitForPlay;
        }
    }

    public void SelectCreditMultiplier(Vector2Int setting)
    {
        _multiplier = setting.y;
        state = GameState.kGameStart;
        ball.SetBouncy(true);
        springController.AllowPlay();
        if (GlobalConfig.autoZoom)
        {
            CameraController.Instance.ZoomIn();
        }
    }

    public void SelectCreditMultiplier()
    {
        if (state == GameState.kSelectMultiplier)
        {
            --GlobalGameContext.credits;
            state = GameState.kWaitForGameStart;
            StartCoroutine(DelayedRun(1f, () => state = GameState.kGameInit));
        }
    }
    /*
        private IEnumerator AnimateLights()
        {
            _onCreditMultiplierSelect = false;
            int selectedMultiplierIndex = 0;
            while (!_onCreditMultiplierSelect)
            {
                selectedMultiplierIndex = _random.Next(_creditMultiplierList.Length);
                Vector2Int setting = _creditMultiplierList[selectedMultiplierIndex];
                ResetLights(setting.x);
                GameUIManager.Instance.SetMultiplierText(setting.y);
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(1f);
            GameUIManager.Instance.SetMultiplierText(0);
            SelectCreditMultiplier(_creditMultiplierList[selectedMultiplierIndex]);
        }
        */
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

    private IEnumerator DelayedRun(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }

    private void UpdateLightSetting(DateTime timeNow)
    {
        _selectedCreditMultiplierIndex = _random.Next(_creditMultiplierList.Length);
        Vector2Int setting = _creditMultiplierList[_selectedCreditMultiplierIndex];
        ResetLights(setting.x);
        GameUIManager.Instance.SetMultiplierText(setting.y);
        _nextLightUpdateTime = timeNow + _updateLightInterval;
    }

    void Update()
    {
        switch (state)
        {
            case GameState.kInit:
                _nextLightUpdateTime = DateTime.MinValue;
                if (_autoStartGame && GlobalGameContext.credits > 0)
                {
                    state = GameState.kSelectMultiplier;
                }
                else
                {
                    state = GameState.kWaitForPlay;
                }
                break;

            case GameState.kSelectMultiplier:
                DateTime timeNow = DateTime.Now;
                if (timeNow > _nextLightUpdateTime)
                {
                    UpdateLightSetting(timeNow);
                }
                break;

            case GameState.kGameInit:
                GameUIManager.Instance.SetMultiplierText(0);
                SelectCreditMultiplier(
                    _creditMultiplierList[_selectedCreditMultiplierIndex]);
                state = GameState.kGameStart;
                break;

            default:
                break;
        }
    }

    public void Quit()
    {
        SceneManager.LoadScene("MenuScene");
    }
}