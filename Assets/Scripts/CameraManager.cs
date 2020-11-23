using UnityEngine;

public static class CameraManager{
    public static Camera MainCamera;
    public static float standardCameraSize;
    public static bool cameraIsStandardSized;

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
