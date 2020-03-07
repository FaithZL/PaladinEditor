using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMat : MonoBehaviour {
    [Range(0, 1)]
    public double metallic = 0.5;

    [Range(0, 1)]
    public double smoothness = 0.5;

    public Color albedo;

    // Start is called before the first frame update
    void Start() {
        Debug.Log(metallic);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
