using UnityEngine;

public static class Controls{
    public static KeyCode MapZoom{get;} = KeyCode.V;
    public static KeyCode Interact{get;} = KeyCode.E;
    public static KeyCode ScrollLeft{get;} = KeyCode.Comma;
    public static KeyCode ScrollRight{get;} = KeyCode.Period;
    public static KeyCode UseItem{get;} = KeyCode.Slash;
    public static KeyCode Buy{get;} = KeyCode.LeftBracket;
    public static KeyCode Sell{get;} = KeyCode.RightBracket;

    public static void MoveCharacter(UnityCharacter character){
        character.Rigidbody.velocity = new Vector3(Input.GetAxis("Horizontal") * 10,0,Input.GetAxis("Vertical") * 20);
        if (character.Rigidbody.velocity.z > 0){
            character.Rigidbody.MoveRotation(Quaternion.Euler(0,180.0f,0));
        } else if (character.Rigidbody.velocity.z < 0){
            character.Rigidbody.MoveRotation(Quaternion.Euler(0,0,0));
        } else if (character.Rigidbody.velocity.x > 0){
            character.Rigidbody.MoveRotation(Quaternion.Euler(0,270.0f,0));
        } else if (character.Rigidbody.velocity.x < 0){
            character.Rigidbody.MoveRotation(Quaternion.Euler(0,90.0f,0));
        }
    }
}
