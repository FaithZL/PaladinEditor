﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class MeshExporter {
    static public JsonData getPrimParam(MeshFilter prim, Transform Parent, Paladin paladin) {

        var param = new JsonData();

        var mesh = prim.sharedMesh;
        var mats = prim.GetComponent<Renderer>().materials;

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
            var subIndexes = new JsonData();
            for (int j = 0; j < indices.Length; ++j) {
                subIndexes.Add(indices[j]);
                paladin.updateProgress();
            }
            indexes.Add(subIndexes);
        }
        param["verts"] = verts;
        if (mesh.normals.Length > 0) {
            param["normals"] = normals;
        }
        if (mesh.uv.Length > 0) {
            param["UVs"] = UVs;
        }

        var transformData = new JsonData();
        Matrix4x4 matrix = prim.transform.localToWorldMatrix;
        
        Matrix4x4 parentMatrix = Parent.worldToLocalMatrix;

        // 抽离出子节点相对父节点的变换矩阵
        matrix = parentMatrix * matrix;

        transformData["type"] = "matrix";
        transformData["param"] = Util.fromMatrix(matrix);
        param["transform"] = transformData;

        param["indexes"] = indexes;
        param["materials"] = new JsonData();
        for(int i = 0; i < mats.Length; ++i) {
            var mat = mats[i];
            param["materials"].Add(MatExporter.getMaterialData(mat, paladin));
        }
        param["emission"] = getEmissionData(prim);

        return param;
    }

    static public JsonData getPrimData() {
        var ret = new JsonData();


        return ret;
    }

    static public long getMeshVertexCount(MeshFilter prim) {
        var mesh = prim.sharedMesh;
        long ret = 0;

        for (int i = 0; i < mesh.subMeshCount; ++i) {
            var indices = mesh.GetIndices(i);
            ret += indices.Length;
        }

        return ret;
    }

    static public JsonData getPrimData(MeshFilter prim, Transform Parent, Paladin paladin) {
        var ret = new JsonData();

        ret["type"] = "triMesh";
        ret["subType"] = "mesh";

        ret["param"] = getPrimParam(prim, Parent, paladin);
        ret["name"] = prim.name;
        
        return ret;
    }

    static JsonData fromColor(Color color) {
        var ret = new JsonData();
        ret.Add((double)color.r);
        ret.Add((double)color.g);
        ret.Add((double)color.b);
        return ret;
    }

    public static JsonData getEmissionData(Emission emission) {

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

    public static JsonData getEmissionData(MeshFilter prim) {

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
