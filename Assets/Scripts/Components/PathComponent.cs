using UnityEngine;
using System.Collections;

public class PathComponent : MonoBehaviour
{
    LineRenderer mLr;

    public void Awake()
    {
        mLr = gameObject.AddComponent<LineRenderer>();
        mLr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        mLr.startColor = Color.red;
        mLr.endColor = Color.red;
        mLr.startWidth = 0.05f;
        mLr.endWidth = 0.05f;
        mLr.positionCount = 0;
    }

    private Vector3[] _points;
    public Vector3[] RenderPoints
    {
        get { return _points; }
        set
        {
            _points = value;
            Draw.DrawLineRend(mLr, _points);
        }
    }

    public void Zero()
    {
        mLr.positionCount = 0;
    }
}
