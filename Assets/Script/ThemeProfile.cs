using UnityEngine;

[CreateAssetMenu(fileName = "Theme", menuName = "ScriptableObjects/Themes")]
public class ThemeProfile : ScriptableObject
{
    public string themeName;
    public Sprite background;
    public Sprite goal;
    public Sprite ball;
}
