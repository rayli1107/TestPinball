using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Sprite spriteLightOn;
    public Sprite spriteLightOff;

    private bool _state;

    public void SetState(bool state)
    {
        _state = state;
        GetComponent<SpriteRenderer>().sprite = (
            _state ? spriteLightOn : spriteLightOff);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        GameController.Instance.RegisterHit(_state);
    }

}