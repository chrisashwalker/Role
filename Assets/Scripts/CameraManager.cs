using UnityEngine;

public static class CameraManager{
    public static Camera MainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    public static float standardCameraSize = MainCamera.orthographicSize;
    public static bool cameraIsStandardSized = true;

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
