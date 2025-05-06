using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyconMovement : MonoBehaviour
{
    // --- Joycon references and settings ---
    private List<Joycon> joycons;
    public int jc_ind = 0;
    public Joycon j;

    // --- Values made available via Unity ---
    public float[] stick;
    public Vector3 gyro;
    public Vector3 accel;
    public Quaternion orientation;
    public Vector3 direction;

    private Quaternion adjustedOrientation;
    private Vector3 adjustedAcceleration;
    private Vector3 defaultDirection;
    public Vector3 finalAccel;

    // Smoothing factor for rotation
    public float rotationSmoothing = 0.1f;

    [SerializeField]
    bool front_perspective;

    void Start()
    {
        gyro = new Vector3(0, 0, 0);
        accel = new Vector3(0, 0, 0);

        // get the public Joycon array attached to the JoyconManager in scene
        joycons = JoyconManager.Instance.j;
        if (joycons.Count < jc_ind + 1)
        {
            Destroy(gameObject);
        }

        defaultDirection.Set(0.094f, 1.011f, -0.03f);
    }

    // Update is called once per frame
    void Update()
    {
        // make sure the Joycon only gets checked if attached
        if (joycons.Count > 0)
        {
            j = joycons[jc_ind];

            // GetButtonDown checks if a button has been pressed (not held)
            if (j.GetButtonDown(Joycon.Button.SHOULDER_2))
            {
                Debug.Log("Shoulder button 2 pressed");

                // GetStick returns a 2-element vector with x/y joystick components
                Debug.Log(string.Format("Stick x: {0:N} Stick y: {1:N}", j.GetStick()[0], j.GetStick()[1]));

                // Joycon has no magnetometer, so it cannot accurately determine its yaw value. Joycon.Recenter allows the user to reset the yaw value.
                gameObject.transform.position = j.Recenter();
            }

            // GetButtonDown checks if a button has been released
            if (j.GetButtonUp(Joycon.Button.SHOULDER_2))
            {
                Debug.Log("Shoulder button 2 released");
            }

            // GetButtonDown checks if a button is currently down (pressed or held)
            if (j.GetButton(Joycon.Button.SHOULDER_2))
            {
                Debug.Log("Shoulder button 2 held");
            }

            /*if (j.GetButtonDown(Joycon.Button.DPAD_DOWN))
            {
                Debug.Log("Rumble");

                // Rumble for 200 milliseconds, with low frequency rumble at 160 Hz and high frequency rumble at 320 Hz. For more information check:
                // https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/rumble_data_table.md
                j.SetRumble(160, 320, 0.6f, 200);

                // The last argument (time) in SetRumble is optional. Call it with three arguments to turn it on without telling it when to turn off.
                // (Useful for dynamically changing rumble values.)
                // Then call SetRumble(0,0,0) when you want to turn it off.
            }*/

            // Update values
            stick = j.GetStick();
            gyro = j.GetGyro();
            accel = j.GetAccel();
            orientation = j.GetVector();

            /*if (j.GetButton(Joycon.Button.DPAD_UP))
            {
                gameObject.GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                gameObject.GetComponent<Renderer>().material.color = Color.blue;
            }*/

            // Rücken perspektive
            // Adjust the orientation to match Unity's coordinate system
            // Swap Y and Z axes and invert Z to convert from Joycon's right-handed to Unity's left-handed coordinate system
            adjustedOrientation = new Quaternion(
                orientation.x,
                -orientation.z, // Swap Z and Y correctly
                orientation.y,
                orientation.w
            );

            if (front_perspective) // if für verschiedene Kameraansichten, warscheinlich besser mit switch als if
            {
                // gegenüber perspektive
                // Assuming the camera is rotated 180 degrees
                Quaternion cameraRotation = Quaternion.Euler(0, 180, 180); // 180-degree rotation on the Y-axis

                // Apply camera rotation to adjusted orientation
                adjustedOrientation = cameraRotation * adjustedOrientation;
            }

            // Smoothly interpolate towards the target rotation
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, adjustedOrientation, rotationSmoothing);

            // gameObject.transform.position = j.GetAccel() * 10;
            adjustedAcceleration.Set(
                j.GetAccel().y,
                -j.GetAccel().z, // Swap Z and Y correctly
                j.GetAccel().x
            );

            if (adjustedAcceleration.magnitude > 0.25f)
            {
                finalAccel = (adjustedAcceleration - defaultDirection) * 2; // += villeicth wenn das offset nicht so schlimm ist
                gameObject.transform.position = finalAccel;
            }
        }
    }

    public void Rumble(float low_frequency, float high_frequency, float amp, int time)
    {
        j.SetRumble(low_frequency, high_frequency, amp, time);
    }
}
