﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System.Threading;

struct Primitive {
    public Vector3[] normals;
    public Vector3[] vertices;
    public Vector2[] UVs;
    public int[][] indices;
    public JsonData materialData;
    public Emission emission;
    public Matrix4x4 localToWorldMatrix;
}

public class MeshComp : MonoBehaviour
{

    public string fileName = "";

    private Primitive[] _primitives;

    private Matrix4x4 _parentWorldToLocalMatrix;

    private JsonData _output;

    bool _complete = false;

    string _filePath;

    string _dir;

    Paladin _paladin;

    private void Awake() {
        fileName = fileName == "" ? this.name : fileName;
    }

    void initPrimitives() {
        MeshFilter[] primitives = GetComponentsInChildren<MeshFilter>() as MeshFilter[];

        _parentWorldToLocalMatrix = transform.worldToLocalMatrix;

        _primitives = new Primitive[primitives.Length];

        _output = new JsonData();

        _paladin = this.GetComponentInParent<Paladin>();

        var comp = _paladin;
        _dir = comp.outputDir + "/" + comp.outputName;
        _filePath = _dir + "/" + fileName + ".json";

        for (int i = 0; i < primitives.Length; ++i) {
            var prim = primitives[i];
            var primitive = new Primitive();
            var mesh = prim.sharedMesh;
            primitive.normals = mesh.normals;
            primitive.vertices = mesh.vertices;
            primitive.UVs = mesh.uv;
            primitive.indices = new int[mesh.subMeshCount][];
            for(int j = 0; j < mesh.subMeshCount; ++j) {
                primitive.indices[j] = mesh.GetIndices(j);
            }
            var material = prim.GetComponent<Renderer>().material;
            primitive.materialData = MatExporter.getMaterialData(material);
            primitive.emission = prim.gameObject.GetComponent<Emission>();
            primitive.localToWorldMatrix = prim.transform.localToWorldMatrix;
            _primitives[i] = primitive;
        }
    }

    void startExport() {
        var ts = new ThreadStart(asyncExport);
        Thread childThread = new Thread(ts);
        childThread.Start();

    }

    void asyncExport() {
        for(int i = 0; i < _primitives.Length; ++i) {
            var prim = _primitives[i];
            var data = getPrimData(prim);
            _output.Add(data);
        }

        saveToFile();
    }

    JsonData getPrimData(Primitive prim) {
        var param = new JsonData();

        var normals = new JsonData();
        var verts = new JsonData();
        var UVs = new JsonData();
        var indexes = new JsonData();

        for (int i = 0; i < prim.normals.Length; ++i) {
            var normal = prim.normals[i];
            normals.Add((double)normal.x);
            normals.Add((double)normal.y);
            normals.Add((double)normal.z);
        }

        for (int i = 0; i < prim.vertices.Length; ++i) {
            var vert = prim.vertices[i];
            verts.Add((double)vert.x);
            verts.Add((double)vert.y);
            verts.Add((double)vert.z);
        }

        for (int i = 0; i < prim.UVs.Length; ++i) {
            var uv = prim.UVs[i];
            UVs.Add((double)uv.x);
            UVs.Add((double)uv.y);
        }

        for (int i = 0; i < prim.indices.Length; ++i) {
            var indices = prim.indices[i];
            for (int j = 0; j < indices.Length; ++j) {
                indexes.Add(indices[j]);
                _paladin.updateProgress();
            }
        }
        param["verts"] = verts;
        if (prim.normals.Length > 0) {
            param["normals"] = normals;
        }
        if (prim.UVs.Length > 0) {
            param["UVs"] = UVs;
        }

        var transformData = new JsonData();
        Matrix4x4 matrix = prim.localToWorldMatrix;

        Matrix4x4 parentMatrix = _parentWorldToLocalMatrix;

        // 抽离出子节点相对父节点的变换矩阵
        matrix = parentMatrix * matrix;

        transformData["type"] = "matrix";
        transformData["param"] = Util.fromMatrix(matrix);
        param["transform"] = transformData;

        param["indexes"] = indexes;
        param["material"] = prim.materialData;
        param["emission"] = MeshExporter.getEmissionData(prim.emission);

        return param;
    }

    void saveToFile() {
        if (!Directory.Exists(_dir)) {
            Directory.CreateDirectory(_dir);
        }

        if (File.Exists(_filePath)) {
            return;
        }

        var sr = File.CreateText(_filePath);


        sr.Write(_output.ToJson(true));

        sr.Close();
        Debug.Log("baocun");
    }

    void export() {
        var comp = this.GetComponentInParent<Paladin>();
        var dir = comp.outputDir + "/" + comp.outputName;
        var filePath = dir + "/" + fileName + ".json";

        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }

        if (File.Exists(filePath)) {
            return;
        }

        var sr = File.CreateText(filePath);

        var output = new JsonData();
        
        MeshFilter[] primitives = GetComponentsInChildren<MeshFilter>() as MeshFilter[];
        for (int i = 0; i < primitives.Length; ++i) {
            var prim = primitives[i];
            output.Add(MeshExporter.getPrimParam(prim, transform, comp));
        }
        sr.Write(output.ToJson(true));

        sr.Close();
    }

    // Start is called before the first frame update
    void Start()
    {
        initPrimitives();
        startExport();
        Debug.Log("haha");
        //export();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
