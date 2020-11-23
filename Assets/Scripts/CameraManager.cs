using UnityEngine;

public static class CameraManager{
    public static Camera MainCamera{get;} = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    public static float standardCameraSize{get;} = MainCamera.orthographicSize;
    public static bool cameraIsStandardSized{get;set;} = true;

    public static void MapToggle(){
        if (cameraIsStandardSized){
            MainCamera.orthographicSize = standardCameraSize * 10;
            cameraIsStandardSized = false;
            } else {
            MainCamera.orthographicSize = standardCameraSize;
            cameraIsStandardSized = true;
            }
    }
}
