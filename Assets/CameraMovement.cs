using System.Collections;
using System.Collections.Generic;
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
        GUI.DrawTexture(new Rect(Screen.width / 2-4, Screen.height / 2-4, 7, 7), texture);
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalRotation = transform.rotation;
        foreach (var item in InputSystem.devices)
        {
            Debug.Log(item.name);
            switch (item.name)
            {
                case "Keyboard":
                    item.MakeCurrent();
                    break;
                case "Mouse":
                    item.MakeCurrent();
                    break;
            }
           
        }
        keyboard = Keyboard.current;
        mouse = Mouse.current;
        
    }

    void Update()
    {
        
        if (mouse.leftButton.isPressed)
        {
            Ray ray = GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            // Запись объекта, в который пришел луч, в переменную
            RaycastHit hit;
            Physics.Raycast(ray, out hit,20);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "destroyAble")
                    Destroy(hit.collider.gameObject);
            }
            else Debug.Log("СОСИ");
        }
        
        // Движения мыши -> Вращение камеры
        if (mouse.rightButton.isPressed || Cursor.visible == false)
        {
            rotationX += mouse.delta.x.ReadValue() * mouseSensitivity;
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
            transfer = transform.forward * ((keyboard.wKey.isPressed ? 1 : 0 * Time.deltaTime) - (keyboard.sKey.isPressed ? 1 : 0 * Time.deltaTime));
            transfer += transform.right * ((keyboard.dKey.isPressed ? 1 : 0 * Time.deltaTime) - (keyboard.aKey.isPressed ? 1 : 0 * Time.deltaTime));
            transform.position += transfer * speed * Time.deltaTime;
        }
          
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) angle += 360F;
        if (angle > 360F) angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
