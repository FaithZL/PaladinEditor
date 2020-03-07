using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : Material {


    public Color Kr = new Color(1, 1, 1);

    public override string getType() {
        return "Mirror";
    }

    void updateMaterial() {
        var mat = gameObject.GetComponent<Renderer>().material;
        mat.SetColor("_Kr", Kr);
    }

    private void Awake() {
        updateMaterial();
    }

    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        updateMaterial();
    }
}
