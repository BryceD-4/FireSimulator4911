/**
Forest Fire Simulator - Unity
Bryce Dixon T00054766 Comp 4911 Capstone March 2026

This program handles the user input to allow the terrain to rotate within the camera.
Code was adapted from the following:
REFERENCES:
//Rotating camera about a space
//REF: https://www.youtube.com/watch?v=4Jq5w5wlfsk

//Modern input system modifications:
//https://www.youtube.com/watch?v=cSkOx35Khlw&t=2s
*/
using UnityEngine;
using UnityEngine.InputSystem;


public class RotatingCamera : MonoBehaviour
{
    //target = the item we want to rotate around = center of the  terrain
    public Transform target;
    //We preset how fast the rotation occurs
    public float rotationSpeed = 60f;
    //Handles directional input
    public float rotationDirection = 0f;

    //Called once per frame
    void Update()
    {
        //Modified to the more modern version of control
        ReadInput();
        ApplyCameraRotation();
    }

    private void ReadInput()
    {
       //Using modern input to get the keyboard object in use
       //REF: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Keyboard.html
        var keyboardObj = Keyboard.current;

        //If the keyboard is not setup yet, return
        if(keyboardObj == null)
        {
            return;   
        }
        
        //Set to 0 for if user is not pressing
        //this way pressing is not cumulative
        rotationDirection = 0f;
        //Getting the key from the current keyboard obtained from the following example
        //REF: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.12//api/UnityEngine.InputSystem.Keyboard.html
        if (keyboardObj.qKey.isPressed)
        {
            //Using -= as just = means if both q and e are pressed, one wins instead of cancelling out.
            rotationDirection -= 1f;
        }
        if (keyboardObj.eKey.isPressed)
        {
            rotationDirection += 1f;
        }
    }
    private void ApplyCameraRotation()
    {
        //If it is 0, we do nothing
        if(rotationDirection != 0)
        {
            //Rotate around allows us to rotate an item about an axis through a point in the world.
            //The item = center of terrain, axis = y axis, rotation amount depends on key press and consistent with deltatime
            //REF: https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Transform.RotateAround.html
            transform.RotateAround(target.position, Vector3.up, rotationDirection*rotationSpeed*Time.deltaTime);
        }
    }
}
