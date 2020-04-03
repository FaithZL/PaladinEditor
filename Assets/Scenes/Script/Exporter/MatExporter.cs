using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEditor;
using System.IO;

public class MatExporter {

    static public JsonData getMaterialData(UnityEngine.Material mat, Paladin paladin = null) {
        mat.name = getName(mat.name);
        switch (mat.name) {
            case "unity":
                return getUnityMatData(mat, paladin);
            case "matte":
                return getMatteData(mat,paladin);
            case "metal":
                return getMetalData(mat,paladin);
            case "glass":
                return getGlassData(mat,paladin);
            case "mirror":
                return getMirrorData(mat,paladin);
            default:
                mat.name = "unity";
                return getUnityMatData(mat,paladin);
        }
    }

    static string getName(string name, Paladin paladin = null) {
        var idx = name.LastIndexOf("(");
        if (idx >= 0) {
            name = name.Substring(0, idx - 1);
            name = name.ToLower();
        }
        return name;
    }

    static JsonData getUnityMatData(UnityEngine.Material mat, Paladin paladin = null) {
        var ret = new JsonData();
        ret["type"] = mat.name;

        var param = new JsonData();
        var albedo = new JsonData();
        var color = Util.fromColor(mat.GetColor("_Color"));
        albedo.Add(color);
        var uvOffset = mat.GetVector("_MainTex_ST");

        var mainTex = mat.GetTexture("_MainTex");
        var texData = new JsonData();
        if (mainTex != null) {
            texData["type"] = "image";
            var srcFn = AssetDatabase.GetAssetPath(mainTex);
            var idx = srcFn.LastIndexOf("/");
            var dstFn = paladin.outputDir + "/" + paladin.outputName + srcFn.Substring(idx);
            var fn = srcFn.Substring(idx + 1);
            if (!File.Exists(dstFn)) {
                FileUtil.CopyFileOrDirectory(srcFn, dstFn);
            }
            texData["param"] = new JsonData();
            texData["subtype"] = "spectrum";
            if (uvOffset != null) {
                texData["param"]["uvOffset"] = Util.fromVec4(uvOffset);
            }
            texData["param"]["fileName"] = fn;
            texData["param"]["fromBasePath"] = true;
        } else {
            texData.Add(1);
            texData.Add(1);
            texData.Add(1);
        }
        albedo.Add(texData);
        param["albedo"] = albedo;
        param["bumpScale"] = mat.GetFloat("_BumpScale");

        var emission = mat.GetColor("_EmissionColor");
        if (emission != null) {
            var emissionData = new JsonData();
            param["emission"] = emissionData;
            emissionData["Le"] = new JsonData();
            emissionData["Le"]["colorType"] = 1;
            emissionData["Le"]["color"] = Util.fromColor(emission);
        }

        var normalMap = mat.GetTexture("_BumpMap");
        if (normalMap != null) {
            var normalMapData = new JsonData();
            var srcFn = AssetDatabase.GetAssetPath(normalMap);
            var idx = srcFn.LastIndexOf("/");
            var dstFn = paladin.outputDir + "/" + paladin.outputName + srcFn.Substring(idx);
            var fn = srcFn.Substring(idx + 1);
            if (!File.Exists(dstFn)) {
                FileUtil.CopyFileOrDirectory(srcFn, dstFn);
            }
            
            normalMapData["param"] = new JsonData();
            normalMapData["subtype"] = "spectrum";
            normalMapData["type"] = "image";
            normalMapData["param"]["fileName"] = fn;
            normalMapData["param"]["fromBasePath"] = true;
            if (uvOffset != null) {
                normalMapData["param"]["uvOffset"] = Util.fromVec4(uvOffset);
            }
            param["normalMap"] = normalMapData;
        }

        var bumpMap = mat.GetTexture("_ParallaxMap");
        if (bumpMap != null) {
            var bumpMapData = new JsonData();
            var srcFn = AssetDatabase.GetAssetPath(bumpMap);
            var idx = srcFn.LastIndexOf("/");
            var dstFn = paladin.outputDir + "/" + paladin.outputName + srcFn.Substring(idx);
            var fn = srcFn.Substring(idx + 1);
            if (!File.Exists(dstFn)) {
                FileUtil.CopyFileOrDirectory(srcFn, dstFn);
            }
            
            bumpMapData["param"] = new JsonData();
            bumpMapData["subtype"] = "spectrum";
            bumpMapData["type"] = "image";
            bumpMapData["param"]["fileName"] = fn;
            bumpMapData["param"]["fromBasePath"] = true;
            if (uvOffset != null) {
                bumpMapData["param"]["uvOffset"] = Util.fromVec4(uvOffset);
            }
            param["bumpMap"] = bumpMapData;
        }

        param["roughness"] = 1 - mat.GetFloat("_Glossiness");
        param["metallic"] = mat.GetFloat("_Metallic");
        ret["param"] = param;

        return ret;
    }


    static JsonData getMatteData(UnityEngine.Material mat, Paladin paladin = null) {
        var ret = new JsonData();

        ret["type"] = mat.name;

        var param = new JsonData();
        param["Kd"] = Util.fromColor(mat.GetColor("_Kd"));
        param["sigma"] = mat.GetFloat("_sigma");
        ret["param"] = param;

        return ret;
    }

    static JsonData getMetalData(UnityEngine.Material mat, Paladin paladin = null) {
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

    static JsonData getMirrorData(UnityEngine.Material mat, Paladin paladin = null) {
        var ret = new JsonData();

        ret["type"] = mat.name;

        var param = new JsonData();
        param["Kr"] = Util.fromColor(mat.GetColor("_Kr"));
        ret["param"] = param;

        return ret;
    }

    static JsonData getGlassData(UnityEngine.Material mat, Paladin paladin = null) {
        var ret = new JsonData();

        ret["type"] = mat.name;

        var param = new JsonData();
        param["Kr"] = Util.fromColor(mat.GetColor("_Kr"));
        param["Kt"] = Util.fromColor(mat.GetColor("_Kt"));
        param["uRough"] = mat.GetFloat("_uRoughness");
        param["vRough"] = mat.GetFloat("_vRoughness");
        param["eta"] = mat.GetFloat("_eta");
        bool remap = mat.GetInt("_remapRoughness") != 0;
        bool thin = mat.GetInt("_thin") != 0;
        param["remapRough"] = remap;
        param["thin"] = thin;
        ret["param"] = param;

        return ret;
    }

}
