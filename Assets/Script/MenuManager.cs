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
    public GameObject prefabThemePanel;
    public HorizontalLayoutGroup panelThemeList;
    public float speedMultiple = 2;

    private int _current;
    private float _speedX;

    private float deltaX
    {
        get
        {
            float ret = prefabThemePanel.GetComponent<RectTransform>().rect.width;
            ret += panelThemeList.spacing;
            return ret;
        }
    }

    private float getX(int i)
    {
        float ret = prefabThemePanel.GetComponent<RectTransform>().rect.width / 2;
        ret += i * deltaX;
        return -1 * ret;
    }

    private void SelectTheme()
    {
        ThemeProfile theme = themes[_current];
        CurrentTheme.theme = theme;
    }

    private void OnEnable()
    {
        _current = 0;
        _speedX = 0f;
        foreach (ThemeProfile profile in themes)
        {
            GameObject panel = Instantiate(
                prefabThemePanel, panelThemeList.transform);
            panel.GetComponentInChildren<TextMeshProUGUI>().text = profile.themeName;
            panel.GetComponentInChildren<Image>().sprite = profile.background;
            panel.SetActive(true);
        }

        SelectTheme();
    }

    public void OnNextButton()
    {
        _current = (_current + 1) % themes.Length;
        _speedX = deltaX * (_current == 0 ? 1 : -1);
        SelectTheme();
    }

    public void OnPrevButton()
    {
        _current = (_current + themes.Length - 1) % themes.Length;
        _speedX = deltaX * ((_current == themes.Length - 1) ? -1 : 1);
        SelectTheme();
    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void Update()
    {
        RectTransform rect = panelThemeList.GetComponent<RectTransform>();
        float x = rect.anchoredPosition.x;
        float targetX = getX(_current);
        if (x != targetX)
        {
            float speed = speedMultiple * _speedX * Time.deltaTime;
            if (_speedX > 0)
            {
                x = Mathf.Min(x + speed, targetX);
            }
            else
            {
                x = Mathf.Max(x + speed, targetX);
            }

            rect.anchoredPosition = new Vector2(x, 0);
        }
    }
}
