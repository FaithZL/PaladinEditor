using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal : Material {

    public Color eta = new Color(0.1f, 0.9f, 0.5f);

    public Color k = new Color(0.1f, 0.9f, 0.5f);

    [Range(0.01f, 1)]
    public float uRoughness = 0.01f;

    [Range(0.01f, 1)]
    public float vRoughness = 0.01f;

    public bool remapRoughness = false;

    public override string getType() {
        return "Metal";
    }

    private void Awake() {
        updateMaterial();
    }

    void updateMaterial() {
        var mat = gameObject.GetComponent<Renderer>().material;
        mat.SetColor("_eta", eta);
        mat.SetColor("_k", k);
        mat.SetFloat("_uRoughness", uRoughness);
        mat.SetFloat("_uRoughness", vRoughness);
        int remap = remapRoughness ? 1 : 0;
        mat.SetInt("", remap);
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        updateMaterial();
    }
}
