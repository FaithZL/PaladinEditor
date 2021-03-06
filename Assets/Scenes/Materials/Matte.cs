﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matte : Material {

    public Color Kd = new Color(1, 1, 1);

    [Range(0,1)]
    public float sigma = 0;

    public override string getType() {
        return "Matte";
    }

    private void Awake() {
        if (!this.isActiveAndEnabled) {
            return;
        }
        InitName();
        UpdateMaterial();
    }

    void InitName() {
        var mat = gameObject.GetComponent<Renderer>().material;
        mat.name = "matte";
    }

    void UpdateMaterial() {
        if (!this.isActiveAndEnabled) {
            return;
        }
        var mat = gameObject.GetComponent<Renderer>().material;
        mat.SetColor("_Kd", Kd);
        mat.SetFloat("_sigma", sigma);
    }

    void Start() {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMaterial();
    }
}
