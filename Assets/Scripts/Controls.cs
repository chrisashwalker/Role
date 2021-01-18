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
            character.Rigidbody.velocity = Vector3.zero;
            Vector3 inputPosition = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical")) * speed;
            if (inputPosition == Vector3.zero){
                if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, ground)){
                    targetPosition = hit.point;
                } else if (targetPosition != Vector3.zero && character.Rigidbody.position != targetPosition){
                    character.Rigidbody.position = Vector3.MoveTowards(character.Rigidbody.position, targetPosition, speed * Time.fixedDeltaTime);
                }
            } else {
                targetPosition = Vector3.zero;
                character.Rigidbody.velocity = inputPosition;
            }
            if (character.Rigidbody.velocity != Vector3.zero){
                Quaternion angle = Quaternion.LookRotation(character.Rigidbody.velocity);
                character.Rigidbody.MoveRotation(angle);
                GameController.Instance.anim.SetBool("Moving", true);
            } else {
                GameController.Instance.anim.SetBool("Moving", false);
            }
        }
    }
}
