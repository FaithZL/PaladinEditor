﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : Material {


    public Color Kr = new Color(1, 1, 1);

    public override string getType() {
        return "Mirror";
    }

    void InitName() {
        var mat = gameObject.GetComponent<Renderer>().material;
        mat.name = "mirror";
    }

    void UpdateMaterial() {
        if (!this.isActiveAndEnabled) {
            return;
        }
        var mat = gameObject.GetComponent<Renderer>().material;
        mat.SetColor("_Kr", Kr);
    }

    private void Awake() {
        if (!this.isActiveAndEnabled) {
            return;
        }
        InitName();
        UpdateMaterial();
    }

    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        UpdateMaterial();
    }
}
