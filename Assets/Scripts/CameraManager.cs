using UnityEngine;

public static class CameraManager{
    public static Camera MainCamera{get;} = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    public static float standardCameraSize{get;} = MainCamera.orthographicSize;
    public static bool cameraIsStandardSized{get;set;} = true;

    public static void CameraFollow(){
        MainCamera.transform.position = new Vector3(GameController.Instance.Player.Rigidbody.position.x, GameController.Instance.Player.Rigidbody.position.y + 2.0f, GameController.Instance.Player.Rigidbody.position.z - 5.0f);
    }

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
