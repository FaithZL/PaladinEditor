using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class Util {

    static public JsonData fromVec3(Vector3 v) {
        var ret = new JsonData();
        ret.Add((double)v.x);
        ret.Add((double)v.y);
        ret.Add((double)v.z);
        return ret;
    }

    static public JsonData fromColor(Color color) {
        var ret = new JsonData();
        ret.Add((double)color.r);
        ret.Add((double)color.g);
        ret.Add((double)color.b);
        return ret;
    }

    static public JsonData fromMatrix(Matrix4x4 matrix) {
        var matrixParam = new JsonData();
        for (int i = 0; i < 4; ++i) {
            var row = matrix.GetRow(i);
            matrixParam.Add((double)row.x);
            matrixParam.Add((double)row.y);
            matrixParam.Add((double)row.z);
            matrixParam.Add((double)row.w);
        }
        return matrixParam;
    }
}
