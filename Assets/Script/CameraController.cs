using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public SpringController springController;
    public float zoomFactor = 0.4f;
    public float zoomSpeed = 0.6f;
    public float viewportY = 0.15f;
    public float viewportHeight = 0.75f;

    public Transform walls;
    public float padding = 0.1f;
    public static CameraController Instance { get; private set; }

    private EventSystem _eventSystem;
    private Camera _camera;
    private float _maxCameraSize;
    private Rect _gameAreaRect;

    private int _zoomSpeedMultiple = 1;

    private float _cameraWidth { 
        get
        {
            return _camera.rect.width * Screen.width;
        }
    }

    private float _cameraHeight
    {
        get
        {
            return _camera.rect.height * Screen.height;
        }
    }

    private void Awake()
    {
        Instance = this;
        _camera = GetComponent<Camera>();
    }

    private void calculateGameAreaRect()
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        for (int i = 0; i < walls.childCount; ++i)
        {
            Transform child = walls.GetChild(i);
            Vector3 scale = child.lossyScale;
            Vector3 pos = child.position;
            minX = Mathf.Min(minX, pos.x - scale.x / 2);
            maxX = Mathf.Max(maxX, pos.x + scale.x / 2);
            minY = Mathf.Min(minY, pos.y - scale.y / 2);
            maxY = Mathf.Max(maxY, pos.y + scale.y / 2);
        }
        minY += springController.maxDistance;
        float width = maxX - minX + 2 * padding;
        float height = maxY - minY + 2 * padding;
        _gameAreaRect = new Rect(minX - padding, minY - padding, width, height);
    }

    void Start()
    {
        _eventSystem = EventSystem.current;

        float x = Screen.safeArea.x / Screen.width;
        float width = Screen.safeArea.width / Screen.width;
        float y = Screen.safeArea.y / Screen.height;
        float height = Screen.safeArea.height / Screen.height;
        float newY = y + height * viewportY;
        height *= viewportHeight;
        _camera.rect = new Rect(x, newY, width, height);

        calculateGameAreaRect();
        float cameraRatio = _cameraWidth / _cameraHeight;
        float gameRatio = _gameAreaRect.width / _gameAreaRect.height;

        if (gameRatio > cameraRatio)
        {
            _maxCameraSize = (_gameAreaRect.width / cameraRatio) / 2;
        }
        else
        {
            _maxCameraSize = _gameAreaRect.height / 2;
        }
        _camera.orthographicSize = _maxCameraSize;
    }

    private void Update()
    {
        float speed = zoomSpeed * _maxCameraSize;
        float size = _camera.orthographicSize + Time.deltaTime * speed * _zoomSpeedMultiple;
        _camera.orthographicSize = Mathf.Clamp(size, _maxCameraSize * zoomFactor, _maxCameraSize);
    }

    private void LateUpdate()
    {
        float cameraRatio = _cameraWidth / _cameraHeight;
        float paddingHeight = _camera.orthographicSize;
        float paddingWidth = paddingHeight * cameraRatio;
        paddingHeight = Mathf.Min(paddingHeight, _gameAreaRect.height / 2);
        paddingWidth = Mathf.Min(paddingWidth, _gameAreaRect.width / 2);
        float minX = _gameAreaRect.x + paddingWidth;
        float maxX = _gameAreaRect.x + _gameAreaRect.width - paddingWidth;
        float minY = _gameAreaRect.y + paddingHeight;
        float maxY = _gameAreaRect.y + _gameAreaRect.height - paddingHeight;
        transform.position = new Vector3(
            Mathf.Clamp(target.position.x, minX, maxX),
            Mathf.Clamp(target.position.y, minY, maxY),
            transform.position.z);
    }

    public void ZoomIn()
    {
        _zoomSpeedMultiple = -1;
    }

    public void ZoomOut()
    {
        _zoomSpeedMultiple = 1;
    }

    public void SetBackgroundColor(Color color)
    {
        _camera.backgroundColor = color;
    }
}
