using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public enum ThemeSelectionState
    {
        kInit,
        kReady,
        kDragging,
        kSnapping
    }

    public class ThemeSelectionManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
#pragma warning disable 0649
        [SerializeField]
        private MenuUIManager _menuUIManager;
        [SerializeField]
        private ThemePanel _prefabThemePanel;
        [SerializeField]
        private Button _buttonPlay;
        [SerializeField]
        private float _dragFactorPC = 1;
        [SerializeField]
        private float _dragFactorMobile = 1;
        [SerializeField]
        private float _snapSpeed = 4;
#pragma warning restore 0649

        private float _themePanelWidth;
        private float _deltaX;
        private Vector2 _prevPosition;

        private ThemeSelectionState _state;
        public ThemeSelectionState state
        {
            get => _state;
            private set
            {
                _state = value;
                _menuUIManager.EnableAction(_state == ThemeSelectionState.kReady);
            }
        }

        private IEnumerator DelayedEnable()
        {
            while (!GlobalGameContext.initialized)
            {
                yield return null;
            }
            for (int i = 0; i < GlobalGameContext.themes.Count; ++i)
            {
                ThemePanel themePanel = Instantiate(_prefabThemePanel, transform);
                themePanel.themeIndex = i;
                themePanel.gameObject.SetActive(true);
            }
            state = ThemeSelectionState.kReady;
        }

        private void Awake()
        {
            _themePanelWidth = _prefabThemePanel.GetComponent<RectTransform>().rect.width;
            _deltaX += _themePanelWidth + GetComponent<HorizontalLayoutGroup>().spacing;
        }

        private float getThemePanelX(int i)
        {
            return -1 * (i * _deltaX + _themePanelWidth / 2);
        }

        private void OnEnable()
        {
            state = ThemeSelectionState.kInit;
            StartCoroutine(DelayedEnable());
        }

        private void processDrag(Vector2 position)
        {
            float moveX = position.x - _prevPosition.x;
            moveX *= GlobalGameContext.isMobile ? _dragFactorMobile : _dragFactorPC;
            transform.localPosition = new Vector3(
                transform.localPosition.x + moveX,
                transform.localPosition.y,
                transform.localPosition.z);
            _prevPosition = position;
        }

        private int getClosestIndex()
        {
            float x = -1 * transform.localPosition.x;
            x -= _themePanelWidth / 2;
            x /= _deltaX;
            return Mathf.Clamp(
                Mathf.RoundToInt(x), 0, GlobalGameContext.themes.Count - 1);
        }

        private void Update()
        {
            if (state == ThemeSelectionState.kDragging)
            {
                processDrag(Input.mousePosition);
            }
            else if (state == ThemeSelectionState.kSnapping)
            {
                int targetIndex = getClosestIndex();
                float targetX = getThemePanelX(targetIndex);
                float diffX = targetX - transform.localPosition.x;
                if (Mathf.Abs(diffX) / _deltaX < 0.01f)
                {
                    transform.localPosition = new Vector3(
                        targetX,
                        transform.localPosition.y,
                        transform.localPosition.z);
                    GlobalGameContext.themeIndex = targetIndex;
                    state = ThemeSelectionState.kReady;
                }
                else
                {
                    float moveX = _snapSpeed * Time.deltaTime;
                    float newX = transform.localPosition.x + moveX * (diffX > 0 ? 1 : -1);
                    float minX = Mathf.Min(transform.localPosition.x, targetX);
                    float maxX = Mathf.Max(transform.localPosition.x, targetX);
                    transform.localPosition = new Vector3(
                        Mathf.Clamp(newX, minX, maxX),
                        transform.localPosition.y,
                        transform.localPosition.z);
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (state == ThemeSelectionState.kReady)
            {
                _prevPosition = eventData.position;
                state = ThemeSelectionState.kDragging;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (state == ThemeSelectionState.kDragging)
            {
                processDrag(eventData.position);
                state = ThemeSelectionState.kSnapping;
            }
        }

        public void Refresh()
        {
            foreach (ThemePanel panel in GetComponentsInChildren<ThemePanel>())
            {
                panel.Refresh();
            }
        }
    }
}
