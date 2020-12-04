using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class CurrentTheme
{
    public static ThemeProfile theme;
}

public class MenuManager : MonoBehaviour
{
    public ThemeProfile[] themes;
    public Image themeImage;
    public TextMeshProUGUI themeText;

    private int _current;

    private void SelectTheme()
    {
        ThemeProfile theme = themes[_current];
        CurrentTheme.theme = theme;
        themeImage.sprite = theme.background;
        themeText.text = theme.themeName;
    }

    private void OnEnable()
    {
        _current = 0;
        SelectTheme();
    }

    public void OnNextButton()
    {
        _current = (_current + 1) % themes.Length;
        SelectTheme();
    }

    public void OnPrevButton()
    {
        _current = (_current + themes.Length - 1) % themes.Length;
        SelectTheme();
    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene("GameScene");
    }
}
