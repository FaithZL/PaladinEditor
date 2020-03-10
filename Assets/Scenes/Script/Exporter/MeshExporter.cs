using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class MeshExporter {
    static public JsonData getPrimParam(MeshFilter prim) {
        var param = new JsonData();

        var mesh = prim.sharedMesh;

        var normals = new JsonData();
        var verts = new JsonData();
        var UVs = new JsonData();
        var indexes = new JsonData();

        for (int i = 0; i < mesh.normals.Length; ++i) {
            var normal = mesh.normals[i];
            normals.Add((double)normal.x);
            normals.Add((double)normal.y);
            normals.Add((double)normal.z);
        }

        for (int i = 0; i < mesh.vertices.Length; ++i) {
            var vert = mesh.vertices[i];
            verts.Add((double)vert.x);
            verts.Add((double)vert.y);
            verts.Add((double)vert.z);
        }

        for (int i = 0; i < mesh.uv.Length; ++i) {
            var uv = mesh.uv[i];
            UVs.Add((double)uv.x);
            UVs.Add((double)uv.y);
        }

        for (int i = 0; i < mesh.subMeshCount; ++i) {
            var indices = mesh.GetIndices(i);
            for (int j = 0; j < indices.Length; ++j) {
                indexes.Add(indices[j]);
            }
        }
        param["verts"] = verts;
        if (mesh.normals.Length > 0) {
            param["normals"] = normals;
        }        
        if (mesh.uv.Length > 0) {
            param["UVs"] = UVs;
        }
        
        param["indexes"] = indexes;

        return param;
    }

    static public JsonData getPrimData(MeshFilter prim) {
        var ret = new JsonData();

        ret["type"] = "triMesh";
        ret["subType"] = "mesh";

        var transformData = new JsonData();
        
        Matrix4x4 matrix = prim.transform.localToWorldMatrix;
        transformData["type"] = "matrix";
        var matParam = new JsonData();
        for (int i = 0; i < 4; ++i) {
            var row = matrix.GetRow(i);
            matParam.Add((double)row.x);
            matParam.Add((double)row.y);
            matParam.Add((double)row.z);
            matParam.Add((double)row.w);
        }
        transformData["param"] = matParam;

        ret["emission"] = getEmissionData(prim);

        var mat = prim.GetComponent<Renderer>().material;
        ret["param"] = getPrimParam(prim);
        ret["name"] = prim.name;
        ret["transform"] = transformData;
        ret["param"]["material"] = MatExporter.getMaterialData(mat);
        return ret;
    }

    static JsonData fromColor(Color color) {
        var ret = new JsonData();
        ret.Add((double)color.r);
        ret.Add((double)color.g);
        ret.Add((double)color.b);
        return ret;
    }

    static JsonData getEmissionData(MeshFilter prim) {

        prim.GetComponentsInParent<Emission>();

        Emission emission = prim.gameObject.GetComponent<Emission>();

        if (emission == null) {
            return null;
        }

        var ret = new JsonData();

        ret["scale"] = emission.scale;
        ret["nSamples"] = emission.sampleNum;
        ret["twoSided"] = emission.twoSided;
        var Le = new JsonData();
        Le["colorType"] = 1;
        Le["color"] = fromColor(emission.color);
        ret["Le"] = Le;

        return ret;
    }
}
