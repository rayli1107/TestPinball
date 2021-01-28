using UnityEngine;

[CreateAssetMenu(fileName = "Theme", menuName = "ScriptableObjects/Themes")]
public class ThemeProfile : ScriptableObject
{
    public string themeName;
    public Sprite background;
    public Sprite goal;
    public Sprite ball;
    public Color backgroundColor;
    public Color wallColor;
    public int keysToUnlock;
}
