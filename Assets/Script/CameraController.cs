using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float minY = 0;

    private EventSystem _eventSystem;
    private Camera _camera;

    void Start()
    {
        _eventSystem = EventSystem.current;
        _camera = GetComponent<Camera>();

        float x = Screen.safeArea.x / Screen.width;
        float width = Screen.safeArea.width / Screen.width;
        float y = Screen.safeArea.y / Screen.height;
        float height = Screen.safeArea.height / Screen.height;
        _camera.rect = new Rect(x, y, width, height);
    }

    void LateUpdate()
    {
        transform.position = new Vector3(
            transform.position.x,
            Mathf.Max(target.position.y, minY),
            transform.position.z);
    }
}
