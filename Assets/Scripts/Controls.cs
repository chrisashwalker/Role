using UnityEngine;

public static class Controls{
    public static KeyCode MapZoom = KeyCode.V;
    public static KeyCode Interact = KeyCode.E;
    public static KeyCode ScrollLeft = KeyCode.Comma;
    public static KeyCode ScrollRight = KeyCode.Period;
    public static KeyCode UseItem = KeyCode.Slash;

    public static void MoveCharacter(UnityCharacter character){
        character.Rigidbody.velocity = new Vector3(Input.GetAxis("Horizontal") * 10,0,Input.GetAxis("Vertical") * 20);
        if (character.Rigidbody.velocity.z > 0){
            character.Rigidbody.MoveRotation(Quaternion.Euler(0,0,180.0f));
        } else if (character.Rigidbody.velocity.z < 0){
            character.Rigidbody.MoveRotation(Quaternion.Euler(0,0,0.0f));
        } else if (character.Rigidbody.velocity.x > 0){
            character.Rigidbody.MoveRotation(Quaternion.Euler(0,0,270.0f));
        } else if (character.Rigidbody.velocity.x < 0){
            character.Rigidbody.MoveRotation(Quaternion.Euler(0,0,90.0f));
        }
    }
}
