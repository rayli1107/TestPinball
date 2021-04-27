using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Vertical
{
    public class VerticalGameController : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private HoleController _prefabHoleController;
        [SerializeField]
        private Rigidbody2D _ballObject;
        [SerializeField]
        private Rigidbody2D _barObject;
        [SerializeField]
        private Rigidbody2D _leftWall;
        [SerializeField]
        private Rigidbody2D _rightWall;
        [SerializeField]
        private Rigidbody2D _topWall;
        [SerializeField]
        private int _numHoles = 20;
        [SerializeField]
        private int _maxHoleAttempts = 10000;
        [SerializeField]
        private float _minHoleDistance = 0.5f;
#pragma warning restore 0649

        private Vector3 _initialBallPosition;
        private Vector3 _initialBarPosition;
        private System.Random _random;
        private List<HoleController> _holes;

        private List<HoleController> SetupHoles()
        {
            float ballRadiusX = _ballObject.transform.localScale.x / 2;
            float ballRadiusY = _ballObject.transform.localScale.x / 2;

            float minX = _leftWall.transform.position.x;
            minX += _leftWall.transform.localScale.x / 2;
            minX += ballRadiusX;

            float maxX = _rightWall.transform.position.x;
            maxX -= _rightWall.transform.localScale.x / 2;
            maxX -= ballRadiusX;

            float minY = _initialBarPosition.y;
            minY += _barObject.transform.localScale.y / 2 + 0.5f;
            minY += ballRadiusY;

            float maxY = _topWall.transform.position.y;
            maxY -= _topWall.transform.localScale.y / 2;
            maxY -= ballRadiusY;

            int attempt = 0;
            _holes.Clear();
            foreach (HoleController hole in GetComponentsInChildren<HoleController>())
            {
                hole.transform.SetParent(null);
                Destroy(hole.gameObject);
            }

            for (; attempt < _maxHoleAttempts && _holes.Count < _numHoles; ++attempt)
            {
                float x = (float)_random.NextDouble() * (maxX - minX) + minX;
                float y = (float)_random.NextDouble() * (maxY - minY) + minY;
                Vector3 position = new Vector3(x, y, 0);

                bool conflict = false;
                foreach (HoleController hole in _holes)
                {
                    if (Vector3.Distance(position, hole.transform.localPosition) < _minHoleDistance)
                    {
                        conflict = true;
                        break;
                    }
                }
                if (!conflict)
                {
                    HoleController hole = Instantiate(_prefabHoleController, transform);
                    hole.transform.localPosition = position;
                    _holes.Add(hole);
                }
            }
            Debug.LogFormat("Attempts: {0}", attempt);
            return _holes;
        }

        private void Awake()
        {
            _random = new System.Random(System.Guid.NewGuid().GetHashCode());
            _initialBallPosition = _ballObject.transform.position;
            _initialBarPosition = _barObject.transform.position;
            _holes = new List<HoleController>();
        }

        private void OnEnable()
        {
            Reset();
        }

        private void Reset()
        {
            _ballObject.transform.position = _initialBallPosition;
            _ballObject.transform.rotation = Quaternion.identity;
            _ballObject.velocity = Vector2.zero;
            _ballObject.angularVelocity = 0;

            _barObject.transform.position = _initialBarPosition;
            _barObject.transform.rotation = Quaternion.identity;
            _barObject.velocity = Vector2.zero;
            _barObject.angularVelocity = 0;

            SetupHoles();

            int index = _random.Next(_holes.Count);
            for (int i = 0; i < _holes.Count; ++i)
            {
                _holes[i].SetState(i == index);
                _holes[i].callback = Reset;
            }
        }
    }
}
