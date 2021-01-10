using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LightType
{
    NONE,
    GOAL,
    KEY
}

public class LightController : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private Sprite _spriteLightNone;
    [SerializeField]
    private Sprite _spriteLightKey;
#pragma warning restore 0649

    private LightType _lightType;

    public void SetState(LightType lightType)
    {
        _lightType = lightType;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        switch (lightType)
        {
            case LightType.NONE:
                spriteRenderer.sprite = _spriteLightNone;
                break;
            case LightType.GOAL:
                spriteRenderer.sprite = CurrentTheme.theme.goal;
                break;
            case LightType.KEY:
                spriteRenderer.sprite = _spriteLightKey;
                break;
        }
    }

    public void OnTriggerExit2D(Collider2D _)
    {
        GameController.Instance.RegisterHit(_lightType);
    }

}