using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoxModel
{
    public string uuid;
    public string type;
    public string name;
    public float[] matrix;
    public string geometry;
    public string material;
    public List<BoxModel> children;

    public BoxModel(string uuid, string type, string name, float[] matrix, string geometry, string material, List<BoxModel> children)
    {
        this.uuid = uuid;
        this.type = type;
        this.name = name;
        this.matrix = matrix;
        this.geometry = geometry;
        this.material = material;
        this.children = children;
    }
}