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

    private Vector3 _initialBallPosition;

    public static GameController Instance { get; private set; }

    public float lightEnableChance = 0.3f;

    private int _multiplier;
    private GameState _state;
    private System.Random _random;
    private bool _first;
    private bool _onCreditMultiplierSelect;

    public void OnEnable()
    {
        if (!GlobalGameContext.initialized)
        {
            Quit();
            return;
        }

        _state = GameState.kCreditCheck;
        _first = true;

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
        _state = GameState.kScoring;
        ball.SetBouncy(false);
    }

    public void OnBallReturn()
    {
        if (_state == GameState.kScoring)
        {
            _state = GameState.kCreditCheck;
            CameraController.Instance.ZoomOut();
            GameUIManager.Instance.EnableGameUIPanel(true);
        }
        else if (_state == GameState.kGameStart)
        {
            springController.AllowPlay();
        }
    }

    public void StartVideoAd()
    {
        if (_state == GameState.kCreditCheck && GlobalGameContext.credits == 0)
        {
            AdsManager.Instance.DisplayVideoAd();
        }
    }

    public void StartPlay()
    {
        if (_state == GameState.kCreditCheck && GlobalGameContext.credits > 0)
        {
            --GlobalGameContext.credits;
            _state = GameState.kSelectMultiplier;
            StartCoroutine(AnimateLights());
//            GameUIManager.Instance.ShowCreditMultiplierPanel();
        }
    }

    public void SelectCreditMultiplier(Vector2Int setting)
    {
        GameUIManager.Instance.EnableGameUIPanel(false);
        _multiplier = setting.y;
        _state = GameState.kGameStart;
        ball.SetBouncy(true);
//        ResetLights(setting.x);
        springController.AllowPlay();
        if (GlobalConfig.autoZoom)
        {
            CameraController.Instance.ZoomIn();
        }
    }

    public void SelectCreditMultiplier()
    {
        _onCreditMultiplierSelect = true;
    }

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
        /*

        LightController[] lightControllers = lights.GetComponentsInChildren<LightController>();
        for (int i = 0; i < lightControllers.Length; ++i)
        {
            lightControllers[i].SetState(LightType.NONE);
        }

        for (int i = 0; i < 3 * lightControllers.Length; ++i)
        {
            int prev = i % lightControllers.Length;
            int cur = (i + 1) % lightControllers.Length;
            lightControllers[prev].SetState(LightType.NONE);
            lightControllers[cur].SetState(LightType.GOAL);
            yield return new WaitForSeconds(0.3f / lightControllers.Length);
        }

        int index = _random.Next(_creditMultiplierList.Length);
        Vector2Int setting = _creditMultiplierList[index];
        GameUIManager.Instance.SetMultiplierText(setting.y);
        yield return new WaitForSeconds(1);
        GameUIManager.Instance.SetMultiplierText(0);
        SelectCreditMultiplier(setting);
*/
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
            GameUIManager.Instance.EnableGameUIPanel(true);
        }
    }

    public void Quit()
    {
        SceneManager.LoadScene("MenuScene");
    }
}