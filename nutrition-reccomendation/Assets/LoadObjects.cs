using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Vuforia;
using System.Linq;

public class LoadObjects : MonoBehaviour {
    public GameObject Model;

    void Start() {
        // Add Call back to check when Vuforia is ready
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnStarted);
    }

    void Update() {

    }

    void OnStarted() {

        // Set working directory to access to dataset
        // Site : https://docs.unity3d.com/Manual/StreamingAssets.html
        string file = "Vuforia/VuforiaMars_Images.xml";
        string path = Application.streamingAssetsPath + "/" + file;

        if(LoadDataSet(path, VuforiaUnity.StorageType.STORAGE_ABSOLUTE))
            Debug.Log(file + " loaded");
        else 
            Debug.Log(file + " load failed");
    }

    private bool LoadDataSet(string filepath, VuforiaUnity.StorageType storageType) {
        // References :
        //  DataSet :
        //      https://library.vuforia.com/sites/default/files/references/unity/classVuforia_1_1DataSet.html
        ObjectTracker objTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

        // Deactivate current active datasets to avoid duplication
        objTracker.Stop();
        System.Collections.Generic.IEnumerable<DataSet> dataSetList = 
            objTracker.GetActiveDataSets();
        foreach(DataSet ds in dataSetList.ToList()) {
            objTracker.DeactivateDataSet(ds);
        }

        // Check if file exists 
        if (!DataSet.Exists(filepath, storageType)) {
            Debug.LogError("Dataset not found!");
            return false;
        }

        // Load DataSet 
        DataSet dataSet = objTracker.CreateDataSet();
        if (!dataSet.Load(filepath, storageType)) {
            Debug.LogError("Couldn't load dataset!");
            return false;
        }

        // Load and add to TrackableBehaviours
        objTracker.ActivateDataSet(dataSet);
        objTracker.Start(); 

        AddMenuToImageTarget(dataSet);

        return true;
    }

    private void AddMenuToImageTarget(DataSet dataSet) {
        // Get All current TrackableBehaviour
        System.Collections.Generic.IEnumerable<TrackableBehaviour> trackableBehaviours = 
            TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();

        foreach (TrackableBehaviour trackable in trackableBehaviours) {
            // Check if the trackable is in dataset
            if (dataSet.Contains(trackable.Trackable)) {
                // Get gameobject of the image target
                GameObject go = trackable.gameObject; 
                go.name = "Imported<" + trackable.TrackableName + ">";
                // Add eventhandler to the Trackable for default features of detecting object
                go.AddComponent<DefaultTrackableEventHandler>();
                // Add TrackingHandler.cs to the Component
                go.AddComponent<TrackingHandler>();
                // Clone Model and add Model to ImageTarget
                GameObject model = Instantiate(Model) as GameObject;
                model.name = go.name + "." + model.name;
                go.GetComponent<TrackingHandler>().Model = model; 
                // Change String from Script
            }
        }
    }

}