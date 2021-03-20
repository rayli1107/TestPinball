
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    class ImageFadeAnimation : MonoBehaviour
    {
        public float duration = 1f;
        public Vector3 move = Vector3.zero;
        public Vector3 scale = Vector3.zero;
        public float alpha = -1;
        public Action callback = null;

        private Image _image;
        private float timeDelta;
        private Transform _transform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _image = GetComponent<Image>();
        }

        private void OnEnable()
        {
            timeDelta = 0;
        }

        private void Update()
        {
            timeDelta += Time.deltaTime;
            if (timeDelta >= duration)
            {
                callback?.Invoke();
                Destroy(gameObject);
                return;
            }

            float f = Time.deltaTime / duration;

            _transform.localPosition += move * f;
            _transform.localScale += scale * f;
            _image.color = new Color(
                _image.color.r,
                _image.color.g,
                _image.color.b,
                Mathf.Clamp(_image.color.a + f * alpha, 0, 1));
        }
    }
}
