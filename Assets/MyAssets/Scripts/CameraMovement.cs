using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    // Присваиваем переменные
    public float mouseSensitivity = 3f;
    public float speed = 5f;
    public float minimumX = -360f;
    public float maximumX = 360f;
    public float minimumY = -60f;
    public float maximumY = 60f;

    public Texture texture;

    Keyboard keyboard;
    Mouse mouse;

    Vector3 transfer;
    float rotationX = 0f;
    float rotationY = 0f;
    Quaternion originalRotation;
    void OnGUI()
    {
        if(texture != null)
        GUI.DrawTexture(new Rect((Screen.width / 2) - texture.width/2, (Screen.height / 2) - texture.height/2, texture.width, texture.height), texture);
    }
    void Start()
    {
        originalRotation = transform.rotation;
        foreach (var item in InputSystem.devices)
        {
            item.MakeCurrent();
        }
        mouse = Mouse.current;
        keyboard = Keyboard.current;
    }

    void Update()
    {
        if (mouse.leftButton.wasPressedThisFrame)
        {
            Ray ray = GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 200);
            
            if (hit.collider != null)
            { 
                    if (hit.collider.gameObject.tag == "destroyAble")
                    Destroy(hit.collider.gameObject);
            }
        }
        // Движения мыши -> Вращение камеры

            rotationX += mouse.delta.x.ReadValue()  * mouseSensitivity;
            rotationY += mouse.delta.y.ReadValue() * mouseSensitivity;

            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
            transform.rotation = originalRotation * xQuaternion * yQuaternion;

            // Ускорение при нажатии клавиши Shift
            
            if (keyboard.leftShiftKey.wasPressedThisFrame)
                speed *= 10;
            else if (keyboard.leftShiftKey.wasReleasedThisFrame)
                speed /= 10;

            // Поднятие и опускание камеры
            Vector3 newPos = new Vector3(0, 1, 0);
            if (keyboard.qKey.isPressed)
                transform.position += newPos * speed * Time.deltaTime;
            else if (keyboard.eKey.isPressed)
                transform.position -= newPos * speed * Time.deltaTime;

            // перемещение камеры
            transfer = transform.forward * ((keyboard.upArrowKey.isPressed ? 1 : 0 * Time.deltaTime) - (keyboard.downArrowKey.isPressed ? 1 : 0 * Time.deltaTime));
            transfer += transform.right * ((keyboard.leftArrowKey.isPressed ? 1 : 0 * Time.deltaTime) - (keyboard.rightArrowKey.isPressed ? 1 : 0 * Time.deltaTime));
            transform.position += transfer * speed * Time.deltaTime;  
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) angle += 360F;
        if (angle > 360F) angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
