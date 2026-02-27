using UnityEngine;

public class CompassCubeRotate : MonoBehaviour
{
    //This holds a reference to the camera itself
    public Transform cameraTransform;
    //This is the offset of the cube from the camera position
    public Vector3 offset = new Vector3(0f, 14f, 30f);
    //This is the center of the terrain itself
    public Transform terrainCenter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
     void LateUpdate()
    {
        //Get the cube to always be oriented with the camera
        //The offset places the cube in the same spot on the screen at all times
        transform.position = cameraTransform.position + 
                             cameraTransform.right * offset.x +
                             cameraTransform.up * offset.y +
                             cameraTransform.forward * offset.z;
        
        //convert the quaternion angle to a readbale euler angle
        //This gets the y-angle of the terrain object
        float terrainYAngle = terrainCenter.eulerAngles.y;

        //.euler(x, y, z) -->xyz are degrees
        //Set the objects rotation to be the same of that of the terrain Y rotation
        //Negative as the rotation is opposite to that of the camera.
        //REF: https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Quaternion.Euler.html
        transform.rotation = Quaternion.Euler(0f, -terrainYAngle, 0f);
    }
}
