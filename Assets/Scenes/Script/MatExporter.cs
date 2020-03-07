using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class MatExporter {

    static public JsonData getMaterialData(UnityEngine.Material mat) {
        switch (mat.name) {
            case "unity":
                return getUnityMatData(mat);
            case "matte":
                return getMatteData(mat);
            case "metal":
                return getMetalData(mat);
            case "glass":
                return getGlassData(mat);
            case "mirror":
                return getMirrorData(mat);
            default:
                return null;
        }
    }

    static JsonData getUnityMatData(UnityEngine.Material mat) {
        var ret = new JsonData();

        return ret;
    }

    static JsonData getMatteData(UnityEngine.Material mat) {
        var ret = new JsonData();

        return ret;
    }

    static JsonData getMetalData(UnityEngine.Material mat) {
        var ret = new JsonData();

        return ret;
    }

    static JsonData getMirrorData(UnityEngine.Material mat) {
        var ret = new JsonData();

        return ret;
    }

    static JsonData getGlassData(UnityEngine.Material mat) {
        var ret = new JsonData();

        return ret;
    }

}
