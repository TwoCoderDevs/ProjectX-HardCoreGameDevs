using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="ShakeTransformEventData", menuName="Custom/ShakeTransformEventData")]
public class ShakeTransformEventData : ScriptableObject
{
    public enum Target
    {
        Position,
        Rotation
    }

    public Target target;
    public float frequency;
    public float amplitude;
    public float duration;
    public AnimationCurve blendOverLifetime;

    public void Init(Target _target, float _frequency, float _amplitude, float _duration, AnimationCurve _blendOverLifetime)
    {
        target = _target;
        frequency = _frequency;
        amplitude = _amplitude;
        duration = _duration;
    }
}
