using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour {

    public Color Kr = new Color(1, 1, 1);

    public Color Kt = new Color(1, 1, 1);

    [Range(0.01f, 1)]
    public double uRoughness = 0.01f;

    [Range(0.01f, 1)]
    public double vRoughness = 0.01f;

    [Range(0, 3)]
    public double eta = 1;

    public bool remapRoughness = false;

    void Start() {
        
    }

    void Update() {
        
    }
}
