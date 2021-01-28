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
    public static int credits
    {
        get => _credits;
        set
        {
            _credits = value;
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

        keys = globalManager.initialKeyCount;
        credits = globalManager.initialCreditCount;
        _creditReloadCount = globalManager.adCreditCount;
        themeIndex = 0;
    }

    public static void ReloadCredits()
    {
        credits = _creditReloadCount;
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
#pragma warning restore 0649

    public ThemeProfile[] themes => _themes;
    public int initialCreditCount => _initialCreditCount;
    public int adCreditCount => _adCreditCount;
    public int initialKeyCount => _initialKeyCount;

    private void Awake()
    {
        GlobalGameContext.Initialize(this);
    }
}