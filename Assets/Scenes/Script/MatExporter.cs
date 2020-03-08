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
        ret["type"] = mat.name;

        var param = new JsonData();
        param["albedo"] = fromColor(mat.GetColor("_Color"));
        param["roughness"] = 1 - mat.GetFloat("_Smoothness");
        param["metallic"] = mat.GetFloat("_Metallic");
        ret["param"] = param;

        return ret;
    }

    static JsonData fromColor(Color color) {
        var ret = new JsonData();
        ret.Add((double)color.r);
        ret.Add((double)color.g);
        ret.Add((double)color.b);
        return ret;
    }

    static JsonData getMatteData(UnityEngine.Material mat) {
        var ret = new JsonData();

        ret["type"] = mat.name;

        var param = new JsonData();
        param["Kd"] = fromColor(mat.GetColor("_Kd"));
        param["sigma"] = mat.GetFloat("_sigma");
        ret["param"] = param;

        return ret;
    }

    static JsonData getMetalData(UnityEngine.Material mat) {
        var ret = new JsonData();
        ret["type"] = mat.name;

        var param = new JsonData();
        param["eta"] = fromColor(mat.GetColor("_eta"));
        param["k"] = fromColor(mat.GetColor("k"));
        param["uRough"] = mat.GetFloat("_uRoughness");
        param["vRough"] = mat.GetFloat("_vRoughness");
        param["rough"] = null;
        bool remap = mat.GetInt("_remapRoughness") != 0;
        param["remapRough"] = remap;
        ret["param"] = param;

        return ret;
    }

    static JsonData getMirrorData(UnityEngine.Material mat) {
        var ret = new JsonData();

        ret["type"] = mat.name;

        var param = new JsonData();
        param["Kr"] = fromColor(mat.GetColor("_Kr"));
        ret["param"] = param;

        return ret;
    }

    static JsonData getGlassData(UnityEngine.Material mat) {
        var ret = new JsonData();

        ret["type"] = mat.name;

        var param = new JsonData();
        param["Kr"] = fromColor(mat.GetColor("_Kr"));
        param["Kt"] = fromColor(mat.GetColor("_Kt"));
        param["uRough"] = mat.GetFloat("_uRoughness");
        param["vRough"] = mat.GetFloat("_vRoughness");
        param["eta"] = mat.GetFloat("_eta");
        bool remap = mat.GetInt("_remapRoughness") != 0;
        param["remapRough"] = remap;
        ret["param"] = param;

        return ret;
    }

}
