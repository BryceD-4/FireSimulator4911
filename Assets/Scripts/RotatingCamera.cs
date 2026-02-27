using UnityEngine;
using UnityEngine.InputSystem;

//Rotating camera about a space
//REF: https://www.youtube.com/watch?v=4Jq5w5wlfsk
public class RotatingCamera : MonoBehaviour
{
    //target = the item we want to rotate around = center of the  terrain
    public Transform target;
    //We preset how fast the rotation occurs
    public float rotationSpeed = 60f;
    public float rotationDirection = 0f;

    void Update()
    {
        // //The user can rotate the camera by pressing Q or E to rotate around the terrain
        // if (Input.GetKey(KeyCode.Q))
        // {
        //     //Rotate around the center of the terrain
        //     //Vector3.up is shorthand ffor 0,1,0 or rotate around y axis
        //     //Set the speed to go appropriate for the delta time speed
        //     transform.RotateAround(target.position, Vector3.up, -rotationSpeed * Time.deltaTime);
        // }

        // if (Input.GetKey(KeyCode.E))
        // {
        //     //Same as above
        //     transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);
        // }

        //Modified to the more modern version of control
        ReadInput();
        ApplyCameraRotation();

    }

    private void ReadInput()
    {
        var keyboardObj = Keyboard.current;

        if(keyboardObj == null)
        {
            return;   
        }
        
        //Set to 0 for if user is not pressing
        rotationDirection = 0f;
        if (keyboardObj.qKey.isPressed)
        {
            rotationDirection = -1f;
        }
        if (keyboardObj.eKey.isPressed)
        {
            rotationDirection = 1f;
        }
    }
    private void ApplyCameraRotation()
    {
        //If it is 0, we do nothing
        if(rotationDirection != 0)
        {
            transform.RotateAround(target.position, Vector3.up, rotationDirection*rotationSpeed*Time.deltaTime);
        }
    }
}
