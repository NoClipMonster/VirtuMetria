using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Entity
{
    public Entity()
    {
        dots = new List<Dot>();
        dotsOnPlane = new DotsOnPlane[2];
    }
    public List<Dot> dots;
    public DotsOnPlane[] dotsOnPlane;
    public class Dot : Entity
    {
        public Vector3 vector3;
        public List<int> SimilarDots = new List<int>();
        public GameObject LayOutDot;
        public Color defaultColor;
        public List<Vector3> Norms;
        public Dot(Vector3 vector, List<int> simDots, List<Vector3> norms, GameObject layDot)
        {
            vector3 = vector;
            SimilarDots = simDots;
            Norms = norms;
            LayOutDot = layDot;
            defaultColor = layDot.GetComponent<Renderer>().material.color;
        }

        public Vector3 Vector3
        {
            get { return vector3; }
            set
            {
                vector3 = value;
                LayOutDot.transform.localPosition = value;
            }
        }

    }
    public class DotsOnPlane
    {
        public Transform Transform;
        public Vector3 Normal;
        public List<int> indexesOfDots;
        public List<Dot> Dots;
        public bool HasPlane = false;
        public Collider hand;
        public SteamVR_Input_Sources side;
        public DotsOnPlane() { }

        public DotsOnPlane(List<Dot> inDots, RaycastHit point, Transform transform, Collider controller)
        {
            hand = controller;
            side = hand.GetComponent<SteamVR_Behaviour_Pose>().inputSource;
            if (HasPlane)
            {
                Debug.LogError("Повторное задание плоскости");
                return;
            }
            Transform = transform;
            Normal = Converter(transform.InverseTransformDirection(point.normal.normalized));
            Dots = inDots;
            HasPlane = true;
            indexesOfDots = new List<int>();
            Plane pl = new Plane(Normal,transform.InverseTransformPoint(point.point));
            for (int i = 0; i < Dots.Count; i++)
            {
                float fl = pl.GetDistanceToPoint(Dots[i].vector3);
                if (Mathf.Abs(fl) < 0.0001)
                {
                   
                    indexesOfDots.Add(i);
                    if(side == SteamVR_Input_Sources.LeftHand)
                    Dots[i].LayOutDot.GetComponent<Renderer>().material.color = Color.red;
                    else Dots[i].LayOutDot.GetComponent<Renderer>().material.color = Color.yellow;
                }
                    

            }
            Vector3 Converter(Vector3 vect)
            {
                string vectr = vect.normalized.ToString();
                vectr = vectr.Remove(vectr.Length - 1);
                vectr = vectr.Remove(0, 1);

                string[] vectr2 = vectr.Split(',');
                for (int i = 0; i < vectr2.Length; i++)
                    vectr2[i] = vectr2[i].Replace(".", ",");

                float x = float.Parse(vectr2[0], System.Globalization.NumberStyles.Float);
                float y = float.Parse(vectr2[1], System.Globalization.NumberStyles.Float);
                float z = float.Parse(vectr2[2], System.Globalization.NumberStyles.Float);
                return new Vector3(x, y, z);
            }
        }

        public void Translate(Vector3 vector3)
        {
            vector3 = Transform.InverseTransformDirection(vector3);
            if (!HasPlane)
            {
                Debug.LogError("Изменение удалённого объекта");
                return;
            }

            foreach (int i in indexesOfDots)
            {

                Vector3 V = new Vector3(vector3.x * Mathf.Abs(Normal.x), vector3.y * Mathf.Abs(Normal.y), vector3.z * Mathf.Abs(Normal.z));
                Dots[i].Vector3 += V;
            }
        }

        public void Excuse()
        {
            if (HasPlane)
            {
                HasPlane = false;
                foreach (int i in indexesOfDots)
                    Dots[i].LayOutDot.GetComponent<Renderer>().material.color = Dots[i].defaultColor;
                Normal = Vector3.zero;
                indexesOfDots = null;
                Dots = null;

            }
            else
                Debug.LogError("Удаление удалённого объекта");

        }

    }
    public void TransformSize(float amount)
    {
        foreach (var dot in dots)
        {
            dot.Vector3 += dot.Vector3*amount;
        }
    }
}

