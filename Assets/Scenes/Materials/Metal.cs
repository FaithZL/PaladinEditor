using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal : MonoBehaviour {

    public Color eta = new Color(0.1f, 0.9f, 0.5f);

    public Color k = new Color(0.1f, 0.9f, 0.5f);

    [Range(0.01f, 1)]
    public double uRoughness = 0.01f;

    [Range(0.01f, 1)]
    public double vRoughness = 0.01f;

    public bool remapRoughness = false;


    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }
}
