using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class ModelMeshEditor : MonoBehaviour
{

    // ������ ����������� �����
    public float pointScale = 1.0f;
    private float lastPointScale = 1.0f;

    Mesh mesh;

    // ������ ������
    List<Vector3> positionList = new List<Vector3>();

    // ������ �������� ���������� ��������
    List<GameObject> positionObjList = new List<GameObject>();

    /// <summary>
    /// ����: ������ �������
    /// ��������: ������� ������� � ������
    /// </summary>
    Dictionary<string, List<int>> pointmap = new Dictionary<string, List<int>>();

    // Use this for initialization
    void Start()
    {
        lastPointScale = pointScale;
        mesh = GetComponent<MeshFilter>().sharedMesh;
        CreateEditorPoint();
    }

    // ������� ����������� �����
    public void CreateEditorPoint()
    {

        positionList = new List<Vector3>(mesh.vertices);

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            string vstr = Vector2String(mesh.vertices[i]);

            if (!pointmap.ContainsKey(vstr))
            {
                pointmap.Add(vstr, new List<int>());
            }
            pointmap[vstr].Add(i);
        }

        foreach (string key in pointmap.Keys)
        {

            GameObject editorpoint = (GameObject)Resources.Load("Prefabs/Sphere");
            editorpoint = Instantiate(editorpoint);
            editorpoint.transform.parent = transform;
            editorpoint.transform.localPosition = String2Vector(key);
            editorpoint.transform.localScale = new Vector3(1f, 1f, 1f);

            MeshEditorPoint editorPoint = editorpoint.GetComponent<MeshEditorPoint>();
            editorPoint.onMove = PointMove;
            editorPoint.pointid = key;

            positionObjList.Add(editorpoint);
        }
    }

    // ���� ����� ���������� ��� ����������� ������� �������
    public void PointMove(string pointid, Vector3 position)
    {
        if (!pointmap.ContainsKey(pointid))
        {
            return;
        }

        List<int> _list = pointmap[pointid];

        for (int i = 0; i < _list.Count; i++)
        {
            positionList[_list[i]] = position;
        }

        mesh.vertices = positionList.ToArray();
        mesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {
        // ����������, ��������� �� ������ ����������� �����
        if (Math.Abs(lastPointScale - pointScale) > 0.1f)
        {
            lastPointScale = pointScale;
            for (int i = 0; i < positionObjList.Count; i++)
            {
                positionObjList[i].transform.localScale = new Vector3(pointScale, pointScale, pointScale);
            }
        }
    }

    string Vector2String(Vector3 v)
    {
        StringBuilder str = new StringBuilder();
        str.Append(v.x).Append(",").Append(v.y).Append(",").Append(v.z);
        return str.ToString();
    }

    Vector3 String2Vector(string vstr)
    {
        try
        {
            string[] strings = vstr.Split(',');
            return new Vector3(float.Parse(strings[0]), float.Parse(strings[1]), float.Parse(strings[2]));
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            return Vector3.zero;
        }
    }
}