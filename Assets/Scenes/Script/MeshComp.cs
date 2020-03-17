using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class MeshComp : MonoBehaviour
{

    public string fileName = "";

    private void Awake() {
        fileName = fileName == "" ? this.name : fileName;
    }

    void export() {
        var comp = this.GetComponentInParent<Paladin>();
        var dir = comp.outputDir + "/" + comp.outputName;
        var filePath = dir + "/" + fileName + ".json";

        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
        Debug.Log(filePath);

        if (File.Exists(filePath)) {
            return;
        }

        var sr = File.CreateText(filePath);

        var output = new JsonData();
        
        MeshFilter[] primitives = GetComponentsInChildren<MeshFilter>() as MeshFilter[];
        for (int i = 0; i < primitives.Length; ++i) {
            var prim = primitives[i];
            output.Add(MeshExporter.getPrimParam(prim, transform, comp));
            //break;
        }
        Debug.Log(filePath);
        sr.Write(output.ToJson(true));

        sr.Close();
    }

    // Start is called before the first frame update
    void Start()
    {
        export();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
