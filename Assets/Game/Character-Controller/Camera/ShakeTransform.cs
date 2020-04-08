using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeTransform : MonoBehaviour
{
    public class ShakeEvent
    {
        public ShakeTransformEventData data;

        public ShakeTransformEventData.Target target
        {
            get
            {
                return data.target;
            }
        }

        private float duration;
        private float timeRemaining;

        Vector3 noiseOffset;
        public Vector3 noise;

        public ShakeEvent(ShakeTransformEventData _data)
        {
            data = _data;
            duration = data.duration;
            timeRemaining = duration;

            noiseOffset.x = Random.Range(0f, 32f);
            noiseOffset.y = Random.Range(0f, 32f);
            noiseOffset.z = Random.Range(0f, 32f);
        }

        public void Update()
        {
            timeRemaining -= Time.deltaTime;

            noiseOffset.x += Time.deltaTime * data.frequency;
            noiseOffset.y += Time.deltaTime * data.frequency;
            noiseOffset.z += Time.deltaTime * data.frequency;

            noise.x = Mathf.PerlinNoise(noiseOffset.x, 0);
            noise.y = Mathf.PerlinNoise(noiseOffset.y, 1);
            noise.z = Mathf.PerlinNoise(noiseOffset.z, 2);

            noise -= Vector3.one * 0.5f;

            noise *= data.amplitude;

            float _agePercent = 1 - (timeRemaining / duration);

            noise *= data.blendOverLifetime.Evaluate(_agePercent);
        }

        public bool IsAlive()
        {
            return timeRemaining > 0;
        }
    }

    public List<ShakeEvent> shakeEvents = new List<ShakeEvent>();

    public void AddShakeEvent(ShakeTransformEventData _data)
    {
        shakeEvents.Add(new ShakeEvent(_data));
    }

    public void AddShakeEventData(ShakeTransformEventData.Target _target, float _frequency, float _amplitude, float _duration)
    {
        AnimationCurve _blendOverLifetime = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 720f),
            new Keyframe(0.2f, 1f),
            new Keyframe(1f, 0f)
        );

        ShakeTransformEventData _data = ScriptableObject.CreateInstance<ShakeTransformEventData>();
        _data.Init(_target, _frequency, _amplitude, _duration, _blendOverLifetime);

        AddShakeEvent(_data);
    }

    void LateUpdate()
    {
        Vector3 _positionOffset = Vector3.zero;
        Vector3 _rotationOffset = Vector3.zero;

        for(int i = shakeEvents.Count - 1; i != -1; i--)
        {
            ShakeEvent _shakeEvent = shakeEvents[i];
            _shakeEvent.Update();

            if(_shakeEvent.target == ShakeTransformEventData.Target.Position)
            {
                _positionOffset += _shakeEvent.noise;
            }
            else
            {
                _rotationOffset += _shakeEvent.noise;
            }

            if(!_shakeEvent.IsAlive())
            {
                shakeEvents.RemoveAt(i);
            }
        }

        transform.localPosition = _positionOffset;
        transform.localEulerAngles = _rotationOffset;
    }
}