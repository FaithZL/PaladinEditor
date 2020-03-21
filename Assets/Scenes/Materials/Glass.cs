using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : Material {

    public Color Kr = new Color(1, 1, 1);

    public Color Kt = new Color(1, 1, 1);

    [Range(0.01f, 1)]
    public float uRoughness = 0.01f;

    [Range(0.01f, 1)]
    public float vRoughness = 0.01f;

    [Range(0, 3)]
    public float eta = 1;

    public bool thin = false;

    public bool remapRoughness = false;

    public override string getType() {
        return "Glass";
    }

    void InitName() {
        var mat = gameObject.GetComponent<Renderer>().material;
        mat.name = "glass";
    }

    void UpdateMaterial() {
        if (!this.isActiveAndEnabled) {
            return;
        }
        var mat = gameObject.GetComponent<Renderer>().material;
        mat.SetColor("_Kr", Kr);
        mat.SetColor("_Kt", Kt);
        mat.SetFloat("_uRoughness", uRoughness);
        mat.SetFloat("_uRoughness", vRoughness);
        mat.SetFloat("_eta", eta);
        int remap = remapRoughness ? 1 : 0;
        mat.SetInt("_remapRoughness", remap);
    }

    private void Awake() {
        if(!this.isActiveAndEnabled) {
            return;
        }
        InitName();
        UpdateMaterial();
    }

    void Start() {
        
    }

    void Update() {
        UpdateMaterial();
    }
}
