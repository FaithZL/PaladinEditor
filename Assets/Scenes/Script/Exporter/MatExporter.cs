using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class MatExporter {

    static public JsonData getMaterialData(UnityEngine.Material mat) {

        mat.name = getName(mat.name);
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
                return getUnityMatData(mat);
        }
    }

    static string getName(string name) {
        var idx = name.LastIndexOf("(");
        if (idx >= 0) {
            name = name.Substring(0, idx - 1);
            name = name.ToLower();
        }
        return name;
    }

    static JsonData getUnityMatData(UnityEngine.Material mat) {
        var ret = new JsonData();
        ret["type"] = mat.name;

        var param = new JsonData();
        param["albedo"] = Util.fromColor(mat.GetColor("_Color"));
        param["roughness"] = 1 - mat.GetFloat("_Smoothness");
        param["metallic"] = mat.GetFloat("_Metallic");
        ret["param"] = param;

        return ret;
    }


    static JsonData getMatteData(UnityEngine.Material mat) {
        var ret = new JsonData();

        ret["type"] = mat.name;

        var param = new JsonData();
        param["Kd"] = Util.fromColor(mat.GetColor("_Kd"));
        param["sigma"] = mat.GetFloat("_sigma");
        ret["param"] = param;

        return ret;
    }

    static JsonData getMetalData(UnityEngine.Material mat) {
        var ret = new JsonData();
        ret["type"] = mat.name;

        var param = new JsonData();
        param["eta"] = Util.fromColor(mat.GetColor("_eta"));
        param["k"] = Util.fromColor(mat.GetColor("k"));
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
        param["Kr"] = Util.fromColor(mat.GetColor("_Kr"));
        ret["param"] = param;

        return ret;
    }

    static JsonData getGlassData(UnityEngine.Material mat) {
        var ret = new JsonData();

        ret["type"] = mat.name;

        var param = new JsonData();
        param["Kr"] = Util.fromColor(mat.GetColor("_Kr"));
        param["Kt"] = Util.fromColor(mat.GetColor("_Kt"));
        param["uRough"] = mat.GetFloat("_uRoughness");
        param["vRough"] = mat.GetFloat("_vRoughness");
        param["eta"] = mat.GetFloat("_eta");
        bool remap = mat.GetInt("_remapRoughness") != 0;
        param["remapRough"] = remap;
        ret["param"] = param;

        return ret;
    }

}
