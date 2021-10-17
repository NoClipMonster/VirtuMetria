using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    // ����������� ����������
    public float mouseSensitivity = 3f;
    public float speed = 5f;
    public float minimumX = -360f;
    public float maximumX = 360f;
    public float minimumY = -60f;
    public float maximumY = 60f;

    public Texture texture;

    Keyboard keyboard;
    Mouse mouse;
    Gamepad gamepad;
    Vector3 transfer;
    float rotationX = 0f;
    float rotationY = 0f;
    Quaternion originalRotation;
    void OnGUI()
    {
        GUI.DrawTexture(new Rect((Screen.width / 2) - texture.width/2, (Screen.height / 2) - texture.height/2, texture.width, texture.height), texture);
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalRotation = transform.rotation;
        foreach (var item in InputSystem.devices)
            Debug.Log(item.name);

        keyboard = Keyboard.current;
        mouse = Mouse.current;
        gamepad = Gamepad.current;
    }

    void Update()
    {

        if (mouse.leftButton.wasPressedThisFrame ||gamepad.rightShoulder.wasPressedThisFrame)
        {
            Ray ray = GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            RaycastHit hit;
            Physics.Raycast(ray, out hit, 200);
            
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
                    if (hit.collider.gameObject.tag == "destroyAble")
                    Destroy(hit.collider.gameObject);
            }
        }
        float ToCube(float f)
        {
            return f * f * f;
        }

        // �������� ���� -> �������� ������

            rotationX += (mouse.delta.x.ReadValue() + ToCube(gamepad.rightStick.x.ReadValue()) * 500) * mouseSensitivity;
            rotationY += (mouse.delta.y.ReadValue() + ToCube(gamepad.rightStick.y.ReadValue()) * 250) * mouseSensitivity;

            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
            transform.rotation = originalRotation * xQuaternion * yQuaternion;



            // ��������� ��� ������� ������� Shift
            
            if (keyboard.leftShiftKey.wasPressedThisFrame|| gamepad.rightTrigger.wasPressedThisFrame)
                speed *= 10;
            else if (keyboard.leftShiftKey.wasReleasedThisFrame||gamepad.rightTrigger.wasReleasedThisFrame)
                speed /= 10;

            // �������� � ��������� ������
            Vector3 newPos = new Vector3(0, 1, 0);
            if (keyboard.qKey.isPressed||gamepad.crossButton.isPressed)
                transform.position += newPos * speed * Time.deltaTime;
            else if (keyboard.eKey.isPressed||gamepad.circleButton.isPressed)
                transform.position -= newPos * speed * Time.deltaTime;

            // ����������� ������
            transfer = transform.forward * (((keyboard.wKey.isPressed ? 1 : 0 * Time.deltaTime) - (keyboard.sKey.isPressed ? 1 : 0 * Time.deltaTime)) + gamepad.leftStick.y.ReadValue());
            transfer += transform.right * (((keyboard.dKey.isPressed ? 1 : 0 * Time.deltaTime) - (keyboard.aKey.isPressed ? 1 : 0 * Time.deltaTime)) + gamepad.leftStick.x.ReadValue());
            transform.position += transfer * speed * Time.deltaTime;
        
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) angle += 360F;
        if (angle > 360F) angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
