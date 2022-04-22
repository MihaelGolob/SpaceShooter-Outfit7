using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBezierCurve : MonoBehaviour{
    // internal variables
    private List<List<Transform>> _curves;
    
    public void Initialize(List<Transform> routes) {
        _curves = new List<List<Transform>>();
        // copy curves list
        foreach (var c in routes) {
            var temp = new List<Transform>();
            temp.Add(c.GetChild(0));
            temp.Add(c.GetChild(1));
            temp.Add(c.GetChild(2));
            temp.Add(c.GetChild(3));
            _curves.Add(temp);
        }
    }
    public void Initialize(Transform curve) {
        _curves = new List<List<Transform>>();
        // copy curves list
        var temp = new List<Transform>();
        temp.Add(curve.GetChild(0));
        temp.Add(curve.GetChild(1));
        temp.Add(curve.GetChild(2));
        temp.Add(curve.GetChild(3));
        _curves.Add(temp);
    }

    public Vector3 GetPoint(int curveIndex, float t) {
        // calculate point
        Vector2 pos = Mathf.Pow(1 - t, 3) * _curves[curveIndex][0].position +
                      3 * Mathf.Pow(1 - t, 2) * t * _curves[curveIndex][1].position +
                      3 * (1 - t) * Mathf.Pow(t, 2) * _curves[curveIndex][2].position +
                      Mathf.Pow(t, 3) * _curves[curveIndex][3].position;

        return pos;
    }
}