using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertical
{
    public class HoleController : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Color _colorRight = Color.green;
        [SerializeField]
        private Color _colorWrong = Color.red;
#pragma warning restore 0649

        public Action callback;
        private SpriteRenderer _sprite;

        public void SetState(bool state)
        {
            _sprite.color = state ? Color.green : Color.red;
        }

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<CircleCollider2D>() != null)
            {
                callback?.Invoke();
            }
        }
    }
}

