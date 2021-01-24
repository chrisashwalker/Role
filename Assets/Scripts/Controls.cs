using UnityEngine;

public static class Controls{
    public static KeyCode MapZoom{get;} = KeyCode.V;
    public static KeyCode Interact{get;} = KeyCode.E;
    public static KeyCode ScrollLeft{get;} = KeyCode.Comma;
    public static KeyCode ScrollRight{get;} = KeyCode.Period;
    public static KeyCode UseItem{get;} = KeyCode.Slash;
    public static KeyCode Buy{get;} = KeyCode.LeftBracket;
    public static KeyCode Sell{get;} = KeyCode.RightBracket;
    public static Vector3 targetPosition = Vector3.zero;

    public static void MoveCharacter(UnityCharacter character){
        float speed = 10.0f;
        Ray ray = CameraManager.MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask ground = LayerMask.GetMask("Ground"); 
        if (character.Grounded){
            Vector3 inputVector = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
            if (inputVector != Vector3.zero){
                targetPosition = character.Rigidbody.position + inputVector;
            } else if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, ground)){
                targetPosition = hit.point;
            }
            if (targetPosition != Vector3.zero && (targetPosition - character.Rigidbody.position).magnitude > 0.1){
                character.Rigidbody.velocity = (targetPosition - character.Rigidbody.position).normalized * speed;
                Quaternion angle = Quaternion.LookRotation(character.Rigidbody.velocity);
                character.Rigidbody.MoveRotation(angle);
                GameController.Instance.anim.SetBool("Moving", true);
            } else {
                targetPosition = Vector3.zero;
                character.Rigidbody.velocity = Vector3.zero;
                if (GameController.Instance.anim.GetBool("Moving")){
                    GameController.Instance.anim.SetBool("Moving", false);
                }
            }
        }
    }
}
