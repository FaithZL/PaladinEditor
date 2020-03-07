using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Unity : Material {
    [Range(0, 1)]
    public float metallic = 0.5f;

    [Range(0, 1)]
    public float smoothness = 0.5f;

    public Color albedo = new Color(1, 1, 1);

    public override string getType() {
        return "UnityMat";
    }

    void InitName() {
        var mat = gameObject.GetComponent<Renderer>().material;
        mat.name = "unity";
    }

    private void Awake() {
        InitName();
        UpdateMaterial();
    }

    void UpdateMaterial() {
        var mat = gameObject.GetComponent<Renderer>().material;
        mat.SetColor("_Color", albedo);
        mat.SetFloat("_Metallic", metallic);
        mat.SetFloat("_Smoothness", smoothness);
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        UpdateMaterial();
    }
}
