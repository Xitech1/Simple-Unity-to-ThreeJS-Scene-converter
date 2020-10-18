using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModel 
{
    public string uuid;
    public string type;
    public string name;
    public float[] matrix;
    public float fov;
    public int zoom;
    public float near;
    public float far;
    public int focus;
    public float aspect;
    public int filmGauge;

    public CameraModel(string uuid, string type, string name, float[] matrix, float fov, int zoom, float near, float far, int focus, float aspect, int filmGauge)
    {
        this.uuid = uuid;
        this.type = type;
        this.name = name;
        this.matrix = matrix;
        this.fov = fov;
        this.zoom = zoom;
        this.near = near;
        this.far = far;
        this.focus = focus;
        this.aspect = aspect;
        this.filmGauge = filmGauge;
    }
}
