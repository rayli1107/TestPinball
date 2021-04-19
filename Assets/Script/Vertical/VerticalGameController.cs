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
        private Rigidbody2D _ballObject;
        [SerializeField]
        private Rigidbody2D _barObject;
#pragma warning restore 0649

        private Vector3 _initialBallPosition;
        private Vector3 _initialBarPosition;
        private System.Random _random;
        private HoleController[] _holes;

        private void Awake()
        {
            _random = new System.Random(System.Guid.NewGuid().GetHashCode());
            _initialBallPosition = _ballObject.transform.position;
            _initialBarPosition = _barObject.transform.position;
            _holes = GetComponentsInChildren<HoleController>(true);
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

            int index = _random.Next(_holes.Length);
            for (int i = 0; i < _holes.Length; ++i)
            {
                _holes[i].SetState(i == index);
                _holes[i].callback = Reset;
            }
        }
    }
}
