using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SceneModel
{
    public string uuid;
    public string type;
    public string name;
    public float[] matrix;
    public List<BoxModel> children;

    public SceneModel(string uuid, string type, string name, float[] matrix, List<BoxModel> children)
    {
        this.uuid = uuid;
        this.type = type;
        this.name = name;
        this.matrix = matrix;
        this.children = children;
    }
}
