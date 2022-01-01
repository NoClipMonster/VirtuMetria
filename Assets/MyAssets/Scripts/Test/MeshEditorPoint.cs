using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshEditorPoint : MonoBehaviour
{

    // ID �������, (��������� ������� ������� � ������)
    [HideInInspector] public string pointid;

    // ���������� ��������� ������������ ��������� ������������ �����, ����� ����������, ������������� �� ����������� �����
    [HideInInspector] private Vector3 lastPosition;


    public delegate void MoveDelegate(string pid, Vector3 pos);

    // �������� ����� ��� ����������� ����������� �����
    public MoveDelegate onMove = null;

    // Use this for initialization
    void Start()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != lastPosition)
        {
            if (onMove != null) onMove(pointid, transform.localPosition);
            lastPosition = transform.position;
        }
    }
}