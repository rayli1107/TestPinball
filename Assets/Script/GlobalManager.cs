using System;
using System.Collections.Generic;
using UnityEngine;

public class ThemeContext
{
    public ThemeProfile theme { get; private set; }
    public bool locked;

    public ThemeContext(ThemeProfile theme, bool locked)
    {
        this.theme = theme;
        this.locked = locked;
    }
}

public static class GlobalGameContext
{
    public static List<ThemeContext> themes {get; private set;}
    public static bool initialized { get; private set; } = false;

    private static int _keys;
    public static int keys {
        get => _keys;
        set
        {
            _keys = value;
            statUpdateAction.Invoke();
        }
    }

    private static int _credits;
    public static int maxCredits { get; private set; }
    private static TimeSpan _creditReloadTimeDuration;
    private static DateTime _lastCreditReloadTime;
    public static DateTime creditReloadTime { get; private set; }

    public static int credits
    {
        get => _credits;
        set
        {
            _credits = value;
            if (_credits < maxCredits)
            {
                _lastCreditReloadTime = DateTime.Now;
                creditReloadTime = _lastCreditReloadTime + _creditReloadTimeDuration;
            }
            statUpdateAction.Invoke();
        }
    }

    private static int _creditReloadCount;
    private static int _themeIndex;
    public static int themeIndex
    {
        get => _themeIndex;
        set
        {
            if (themes == null || themes.Count == 0)
            {
                _themeIndex = 0;
            }
            else
            {
                _themeIndex = (value + themes.Count) % themes.Count;
            }
        }
    }

    public static ThemeProfile currentTheme => themes[themeIndex].theme;
    public static Action statUpdateAction;

    public static void Initialize(GlobalManager globalManager)
    {
        if (initialized)
        {
            return;
        }
        initialized = true;

        themes = new List<ThemeContext>();
        foreach (ThemeProfile theme in globalManager.themes)
        {
            themes.Add(new ThemeContext(theme, theme.keysToUnlock > 0));
        }

        _creditReloadTimeDuration = TimeSpan.FromSeconds(
            globalManager.creditReloadTimeSeconds);
        _creditReloadCount = globalManager.adCreditCount;
        maxCredits = globalManager.maxCreditCount;

        keys = globalManager.initialKeyCount;
        credits = globalManager.initialCreditCount;
        themeIndex = 0;
    }

    public static void ReloadCredits()
    {
        credits = _creditReloadCount;
    }

    public static void UpdateCredits()
    {
        DateTime timeNow = DateTime.Now;
        if (credits < maxCredits && timeNow >= creditReloadTime)
        {
//            Debug.Log("Credits Added");
            int diffSecs = (timeNow - _lastCreditReloadTime).Seconds;
            int creditAdded = diffSecs / _creditReloadTimeDuration.Seconds;
            if (creditAdded > 0)
            {
                credits = Mathf.Min(credits + creditAdded, maxCredits);
            }
        }
    }
}

public class GlobalManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private ThemeProfile[] _themes;
    [SerializeField]
    private int _initialCreditCount = 2;
    [SerializeField]
    private int _adCreditCount = 10;
    [SerializeField]
    private int _initialKeyCount = 10;
    [SerializeField]
    private int _maxCreditCount = 10;
    [SerializeField]
    private int _creditReloadTimeSeconds = 10*60;
    [SerializeField]
    private bool _initialScene = true;
#pragma warning restore 0649

    public ThemeProfile[] themes => _themes;
    public int initialCreditCount => _initialCreditCount;
    public int maxCreditCount => _maxCreditCount;
    public int adCreditCount => _adCreditCount;
    public int creditReloadTimeSeconds => _creditReloadTimeSeconds;
    public int initialKeyCount => _initialKeyCount;


    private void Awake()
    {
        if (_initialScene)
        {
            GlobalGameContext.Initialize(this);
        }
    }

    private void Update()
    {
        if (GlobalGameContext.initialized)
        {
            GlobalGameContext.UpdateCredits();
        }
    }
}