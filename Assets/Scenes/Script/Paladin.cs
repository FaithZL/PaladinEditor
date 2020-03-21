
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;
using UnityEngine.Assertions;
using System.IO;
using UnityEditor;
using System.Threading;

public class Paladin : MonoBehaviour {

    public string outputDir = "paladin_output";

    public int threadNum = 0;

    //[Header("--------medium--------")]

    [Header("--------filter param--------")]
    public Filter filterName;
    public Vector2 filterRadius = new Vector2(1, 1);
    [Header("Gaussian")]
    public float alpha = 2;
    [Header("Mitchell")]
    public float B = 1.0f / 3.0f;
    public float C = 1.0f / 3.0f;
    [Header("Sinc")]
    public float tau = 3.0f;


    [Header("--------film param--------")]
    public Vector2Int resolution = new Vector2Int(500,500);
    public Rect cropWindow = new Rect(0,0,1,1);
    public string outputName = "paladin";
    public string imageName = "paladin";
    public FileFormat fileFormat;
    [Range(0, 1)]
    public float scale = 1.0f;
    [Range(0, 1)]
    public float diagonal = 1.0f;

    [Header("Halton,Random")]
    [Header("--------sampler param--------")]

    public Sampler samplerName;
    public int spp = 2;
    [Header("Stratified")]
    public int xSpp = 1;
    public int ySpp = 1;
    public int dimensions = 6;
    public bool jitter = true;

    [Header("--------integrator param--------")]
    public Integrator integratorName;
    public int maxBounce = 6;
    public float rrThreshold = 1;
    public LightSampleStrategy lightSampleStrategy;

    [Header("--------camera param--------")]
    public float shutterOpen = 0;
    public float shutterClose = 1;
    public float lensRadius = 0;
    public float focalDistance = 100;

    [Header("--------accelerator--------")]
    public Accelerator acceleratorName;
    public int maxPrimsInNode = 1;
    public AclrtSplitMethod splitMethod;


    private List<MeshFilter> _primitives;

    private List<Light> _lights;

    private Camera _camera;

    private JsonData _output = new JsonData();

    private long _vertexCount = 0;

    private long _progress = 0;

    private ThreadStart progressReporter;

    public void updateProgress() {
        _progress += 1;
    }

    void startReporter() {
        //progressReporter = new ThreadStart(onProgress);
        //Thread childThread = new Thread(progressReporter);
        //childThread.Start();
    }

    private void Awake() {
        clearDir();
    }

    void onProgress() {
        while (true) {
            float percent = (float)_progress / _vertexCount;
            string s = "total is " + _vertexCount + ",progress is " + _progress + ",percent is " + percent;
            Debug.Log(s);
            Thread.Sleep(1000);
            if (percent == 1) {
                return;
            }
        }
    }

    void Start() {
        Debug.Log("导出");
        
        exec();
        export();
        Debug.Log("导出完毕");
    }

    void clearDir() {
        var dir = outputDir + "/" + outputName;
        if (Directory.Exists(dir))
            Directory.Delete(dir, true);
        Directory.CreateDirectory(dir);
    }

    void statusVertexNum() {
        MeshFilter[] primitives = GameObject.FindObjectsOfType<MeshFilter>() as MeshFilter[];
        for(int i = 0; i < primitives.Length; ++i) {
            _vertexCount += MeshExporter.getMeshVertexCount(primitives[i]);
        }
    }

    void exec() {
        statusVertexNum();
        startReporter();
        handleCamera();
        handleFilm();
        handleFilter();
        handleSampler();
        handlePrimitives();
        handleIntegrator();
        handleLights();
        handleEnvMap();
        handleAccelerator();
        handleThreadNum();
    }

    void handleEnvMap() {
        var matSky = RenderSettings.skybox;
        if (matSky == null) {
            return;
        }
        var texture = matSky.GetTexture("_MainTex");
        var rotation = - matSky.GetFloat("_Rotation") - 180;
        var color = matSky.GetColor("_Tint");
        var exposure = matSky.GetFloat("_Exposure");
        exposure = 2 * (float)Math.Pow(exposure, 2) + 0.5f * exposure - 0.5f;

        var envmapData = new JsonData();
        envmapData["type"] = "envmap";
        var param = new JsonData();
        envmapData["param"] = param;

        var transformLst = new JsonData();
        var t1 = new JsonData();
        t1["type"] = "rotateX";
        t1["param"] = new JsonData();
        t1["param"].Add(-90);

        var t2 = new JsonData();
        t2["type"] = "rotateY";
        t2["param"] = new JsonData();
        t2["param"].Add((double)rotation);
        transformLst.Add(t1);
        transformLst.Add(t2);

        param["scale"] = exposure;
        param["L"] = Util.fromColor(color);
        param["nSamples"] = 1;
        param["transform"] = transformLst;
        param["fromBasePath"] = true;

        
        var srcFn = AssetDatabase.GetAssetPath(texture);
        var idx = srcFn.LastIndexOf("/");
        var dstFn = outputDir + "/" +outputName + srcFn.Substring(idx);
        param["fn"] = srcFn.Substring(idx + 1);

        if (!File.Exists(dstFn)) {
            FileUtil.CopyFileOrDirectory(srcFn, dstFn);
        }

        if (_output["lights"] == null) {
            _output["lights"] = new JsonData();
        }
        _output["lights"].Add(envmapData);


    }

    void handleCamera() {

        _camera = GameObject.FindObjectOfType<Camera>() as Camera;

        var param = new JsonData();

        var pos = _camera.transform.localPosition;
        var target = _camera.transform.forward + pos;
        var up = _camera.transform.up;

        var lookAt = new JsonData();
        lookAt.Add(Util.fromVec3(pos));
        lookAt.Add(Util.fromVec3(target));
        lookAt.Add(Util.fromVec3(up));

        param["lookAt"] = lookAt;

        param["shutterOpen"] = shutterOpen;
        param["shutterClose"] = shutterClose;
        param["fov"] = _camera.fieldOfView;
        param["lensRadius"] = lensRadius;
        param["focalDistance"] = focalDistance;

        var cameraData = new JsonData();
        cameraData["type"] = "perspective";
        cameraData["param"] = param;

        _output["camera"] = cameraData;

    }

    void handleThreadNum() {
        _output["threadNum"] = threadNum;
    }

    void handleSampler() {
        var samplerData = new JsonData();

        var param = new JsonData();

        samplerData["param"] = param;

        switch (samplerName) {
            case Sampler.halton:
                samplerData["type"] = "halton";
                param["spp"] = spp;
                break;
            case Sampler.random:
                samplerData["type"] = "random";
                param["spp"] = spp;
                break;
            case Sampler.stratified:
                samplerData["type"] = "stratified";
                param["xsamples"] = xSpp;
                param["ysamples"] = ySpp;
                param["dimensions"] = dimensions;
                param["jitter"] = jitter;
                break;
        }
        _output["sampler"] = samplerData;
    }

    void handleFilter() {
        var filterData = new JsonData();
        var param = new JsonData();
        filterData["param"] = param;

        var radius = new JsonData();
        radius.Add((double)filterRadius.x);
        radius.Add((double)filterRadius.y);
        param["radius"] = radius;

        filterData["type"] = PaladinEnum.getName(filterName);

        switch (filterName) {
            case Filter.gaussian:
                param["alpha"] = alpha;
                break;
            case Filter.mitchell:
                param["B"] = B;
                param["C"] = C;
                break;
            case Filter.sinc:
                param["tau"] = tau;
                break;
        }
        _output["filter"] = filterData;

    }

    void handleAccelerator() {
        var aclrtData = new JsonData();
        var param = new JsonData();
        aclrtData["param"] = param;
        switch (acceleratorName) {
            case Accelerator.bvh:
                aclrtData["type"] = "bvh";
                param["maxPrimsInNode"] = maxPrimsInNode;
                param["splitMethod"] = PaladinEnum.getName(splitMethod);
                break;
            case Accelerator.kdTree:
                //暂时不支持kdtree
                Assert.IsFalse(false);
                break;
        }

        _output["accelerator"] = aclrtData;
    }

    void handleFilm() {
        var filmData = new JsonData();
        var param = new JsonData();
        filmData["param"] = param;

        var res = new JsonData();
        res.Add((double)resolution.x);
        res.Add((double)resolution.y);
        param["resolution"] = res;

        var cw = new JsonData();
        cw.Add((double)cropWindow.x);
        cw.Add((double)cropWindow.y);
        cw.Add((double)cropWindow.width);
        cw.Add((double)cropWindow.height);
        param["cropWindow"] = cw;

        var fn = imageName + "." + PaladinEnum.getName(fileFormat);
        param["fileName"] = fn;

        param["scale"] = scale;
        param["diagonal"] = diagonal;

        _output["film"] = filmData;
    }

    void handleIntegrator() {
        var integratorData = new JsonData();
        var param = new JsonData();
        integratorData["param"] = param;

        switch (integratorName) {
            case Integrator.path:
                integratorData["type"] = "pt";
                param["maxBounce"] = maxBounce;
                param["rrThreshold"] = rrThreshold;
                param["lightSampleStrategy"] = PaladinEnum.getName(lightSampleStrategy);
                break;
            case Integrator.bdpt:
                integratorData["type"] = "bdpt";
                param["maxBounce"] = maxBounce;
                param["rrThreshold"] = rrThreshold;
                param["lightSampleStrategy"] = PaladinEnum.getName(lightSampleStrategy);
                break;
            case Integrator.volpt:
                integratorData["type"] = "volpt";
                param["maxBounce"] = maxBounce;
                param["rrThreshold"] = rrThreshold;
                param["lightSampleStrategy"] = PaladinEnum.getName(lightSampleStrategy);
                break;
            case Integrator.Geometry:
                integratorData["type"] = "Geometry";
                param["type"] = "normal";
                break;
        }
        _output["integrator"] = integratorData;
    }

    void handleLights() {
        var lightData = new JsonData();
        Light[] lights = GameObject.FindObjectsOfType<Light>() as Light[];

        for(int i = 0; i < lights.Length; ++i) {
            Light light = lights[i];
            if(light.type == LightType.Directional
                || light.type == LightType.Point
                || light.type == LightType.Spot) {
                lightData.Add(LightExporter.getLight(light));
            }
        }
        if (lights.Length > 0) {
            _output["lights"] = lightData;
        } else {
            _output["lights"] = null;
        }

    }

    void handleChild(Transform node) {

        var mc = node.GetComponent<MeshComp>();
        var prim = node.GetComponent<MeshFilter>();
        var isActive = node.gameObject.activeInHierarchy;

        if (mc && mc.isActiveAndEnabled && isActive) {
            // 单独导出网格文件
            var shapeData = new JsonData();
            shapeData["type"] = "triMesh";
            shapeData["subType"] = "mesh";
            var transformData = new JsonData();
            transformData["type"] = "matrix";
            transformData["param"] = Util.fromMatrix(node.localToWorldMatrix);
            shapeData["transform"] = transformData;
            shapeData["param"] = mc.fileName + ".json";
            if(_output["shapes"] == null) {
                _output["shapes"] = new JsonData();
            }
            _output["shapes"].Add(shapeData);
            // 如果该节点包含MeshComp组件，则表示该节点单独导出文件
            return;

        } else if (prim && isActive) {
            if (_output["shapes"] == null) {
                _output["shapes"] = new JsonData();
            }
            // 如果有prim对象并且处于激活状态
            _output["shapes"].Add(MeshExporter.getPrimData(prim, transform, this));
        }

        foreach (Transform child in node.transform) {
            handleChild(child);
        }

    }



    void handlePrimitives() {
        _output["shapes"] = null;
        handleChild(transform);
    }


    void export() {
        string json = _output.ToJson(true);
        var dir = outputDir + "/" + outputName;
        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
        string SceneFileName = outputName;
        var sr = File.CreateText("./" + dir + "/" + SceneFileName + ".json");

        sr.WriteLine(json);
        sr.Close();
    }
}
