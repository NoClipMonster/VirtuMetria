using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Присваиваем переменные
    public float mouseSensitivity = 3f;
    public float speed = 5f;
    public float minimumX = -360f;
    public float maximumX = 360f;
    public float minimumY = -60f;
    public float maximumY = 60f;

    Vector3 transfer;
    float rotationX = 0f;
    float rotationY = 0f;
    Quaternion originalRotation;
    Vector3 mousePosition;
    void Start()
    {
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            mousePosition = Input.mousePosition;
        if (Input.GetMouseButton(1))
        {
            rotationX += (Input.mousePosition.x - mousePosition.x) * mouseSensitivity;
            rotationY += (Input.mousePosition.y - mousePosition.y) * mouseSensitivity;
            mousePosition.x = Input.mousePosition.x;
            mousePosition.y = Input.mousePosition.y;

            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
            transform.rotation = originalRotation * xQuaternion * yQuaternion;

        }
        // Движения мыши -> Вращение камеры



        // Ускорение при нажатии клавиши Shift

        if (Input.GetKeyDown(KeyCode.RightShift))
            speed *= 10;
        else if (Input.GetKeyUp(KeyCode.RightShift))
            speed /= 10;

        // Поднятие и опускание камеры
        Vector3 newPos = new Vector3(0, 1, 0);
        if (Input.GetKey(KeyCode.Q))
            transform.position += speed * Time.deltaTime * newPos;
        else if (Input.GetKey(KeyCode.E))
            transform.position -= speed * Time.deltaTime * newPos;

        // перемещение камеры
        transfer = transform.forward * (((Input.GetKey(KeyCode.UpArrow) ? 1 : 0)) - ((Input.GetKey(KeyCode.DownArrow) ? 1 : 0)));
        transfer += transform.right * (((Input.GetKey(KeyCode.RightArrow) ? 1 : 0)) - ((Input.GetKey(KeyCode.LeftArrow) ? 1 : 0)));
        transfer += transform.up * (((Input.GetKey(KeyCode.Keypad0) ? 1 : 0)) - ((Input.GetKey(KeyCode.Keypad1  ) ? 1 : 0)));
        transform.position += speed * Time.deltaTime * transfer;
    }
  

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) angle += 360F;
        if (angle > 360F) angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
