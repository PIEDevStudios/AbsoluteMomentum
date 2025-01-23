using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a class to hold misc. functions that help with many different things
/// </summary>
public class Utilites : MonoBehaviour
{
    /// <summary>
    /// Maps a value that is in between min1 and max1 to be inbetween min2 and max2. Enabling clamping will ensure that
    /// the new value is between min2 and max2.
    /// EX: mapping the value 1.5 with min1=1 and max1=2 to min2=2 and max2=3 will return 2.5.
    /// </summary>
    /// <param name="_value"></param>
    /// <param name="_min1"></param>
    /// <param name="_max1"></param>
    /// <param name="_min2"></param>
    /// <param name="_max2"></param>
    /// <param name="_clamp"></param>
    /// <returns></returns>
    public static float Map(float _value, float _min1, float _max1, float _min2, float _max2, bool _clamp = false)
    {
        float _val = _min2 + (_max2 - _min2) * ((_value - _min1) / (_max1 - _min1));
        return _clamp ? Mathf.Clamp(_val, Mathf.Min(_min2, _max2), Mathf.Max(_min2, _max2)) : _val;
    }
}
