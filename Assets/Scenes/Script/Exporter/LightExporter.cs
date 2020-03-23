using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class LightExporter {

    static public JsonData getLight(Light light) {
        switch (light.type) {
            case LightType.Point:
                return getPointLight(light);
            case LightType.Spot:
                return getSpotLight(light);
            case LightType.Directional:
                return getDistantLight(light);
            default:
                return null;
        }
    }

    static JsonData getPointLight(Light light) {
        var ret = new JsonData();
        ret["type"] = "pointLight";

        var param = new JsonData();
        var pos = light.transform.localPosition;
        var tf = new JsonData();
        tf["type"] = "translate";
        tf["param"] = fromVec3(pos);
        param["transform"] = tf;

        var I = new JsonData();
        I["colorType"] = 1;
        var c = light.color;
        I["color"] = fromVec3(new Vector3(c.r, c.g, c.b));
        param["I"] = I;

        param["scale"] = light.intensity;

        ret["param"] = param;

        return ret;
    }

    static JsonData fromVec3(Vector3 v) {
        var ret = new JsonData();
        ret.Add((double)v.x);
        ret.Add((double)v.y);
        ret.Add((double)v.z);
        return ret;
    }

    static JsonData getDistantLight(Light light) {
        var ret = new JsonData();

        ret["type"] = "distant";
        var param = new JsonData();
        var fwd = light.transform.forward;
        param["wLight"] = fromVec3(-fwd);

        var L = new JsonData();
        L["colorType"] = 1;
        var c = light.color;
        L["color"] = fromVec3(new Vector3(c.r, c.g, c.b));
        param["L"] = L;

        param["scale"] = light.intensity * 2.5;

        ret["param"] = param;

        return ret;
    }

    static JsonData getSpotLight(Light light) {
        var ret = new JsonData();

        ret["type"] = "spot";
        var param = new JsonData();

        var matrix = light.transform.localToWorldMatrix;
        var transformData = new JsonData();
        transformData["type"] = "matrix";
        transformData["param"] = Util.fromMatrix(matrix);
        param["transform"] = transformData;

        var I = new JsonData();
        I["colorType"] = 1;
        var c = light.color;
        I["color"] = fromVec3(new Vector3(c.r, c.g, c.b));
        param["I"] = I;

        param["scale"] = light.intensity;
        param["totalAngle"] = light.spotAngle / 2;
        param["falloffStart"] = 0;
        ret["param"] = param;
        return ret;
    }

}
