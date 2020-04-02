using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json;

public class Util {

    static public JsonData fromVec3(Vector3 v) {
        var ret = new JsonData();
        ret.Add((double)v.x);
        ret.Add((double)v.y);
        ret.Add((double)v.z);
        return ret;
    }

    static public JsonData fromVec4(Vector4 v) {
        var ret = new JsonData();
        ret.Add((double)v.x);
        ret.Add((double)v.y);
        ret.Add((double)v.z);
        ret.Add((double)v.w);
        return ret;
    }

    static public JsonData fromColor(Color color) {
        var ret = new JsonData();
        ret.Add((double)color.r);
        ret.Add((double)color.g);
        ret.Add((double)color.b);
        return ret;
    }

    static public Vector3 decodeHDR(Vector4 data, Vector4 decodeInstructions) {
        // Take into account texture alpha if decodeInstructions.w is true(the alpha value affects the RGB channels)
        var alpha = (decodeInstructions.w * (data.w - 1.0)) + 1.0;
        Vector3 outputData = new Vector3(data.x, data.y, data.z);
        //If Linear mode is not supported we can skip exponent part
//#if defined(UNITY_COLORSPACE_GAMMA)
        //return (float)(decodeInstructions.x * alpha) * outputData;
//#else
//#if defined(UNITY_USE_NATIVE_HDR)
            //return (float)decodeInstructions.x * outputData; // Multiplier for future HDRI relative to absolute conversion.
//#else
        return (float)(decodeInstructions.x * Math.Pow(alpha, decodeInstructions.y)) * outputData;
//#endif
//#endif
    }

    static public Vector3 colorToVec3(Color color) {
        return new Vector3(color.r, color.g, color.b);
    }

    static public Vector4 colorToVec4(Color color) {
        return new Vector4(color.r, color.g, color.b, color.a);
    }

    static public Color vec4ToColor(Vector4 vec) {
        return new Color(vec.x, vec.y, vec.z, vec.w);
    }

    static public Color vec3ToColor(Vector3 vec) {
        return new Color(vec.x, vec.y, vec.z, 1);
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

    static public byte[] jsonToBytes(JObject str) {
        using (var ms = new MemoryStream()) {
            using (var bw = new BsonWriter(ms)) {
                var serializer = new JsonSerializer();

                serializer.Serialize(bw, str);

                bw.Flush();
            }

            return ms.ToArray();
        }
    }
}
