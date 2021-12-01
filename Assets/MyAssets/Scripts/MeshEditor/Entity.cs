using System;
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
    public float BigestEdge;
    public List<Dot> dots;
    public DotsOnPlane[] dotsOnPlane;
    public class Dot : Entity
    {
        public Vector3 vector3;
        public List<int> SimilarDots = new List<int>();
        public List<int> triangles;
  
        public Dot(Vector3 vector)
        {
            vector3 = vector;
            SimilarDots = new List<int>();
            triangles = new List<int>();
        }

        public Vector3 Vector3
        {
            
            get { return vector3; }
            set { vector3 = value; }
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
            Plane pl = new Plane(Normal, transform.InverseTransformPoint(point.point));
            for (int i = 0; i < Dots.Count; i++)
            {
                float fl = pl.GetDistanceToPoint(Dots[i].vector3);
                if (Mathf.Abs(fl) < 0.0001)
                {
                    indexesOfDots.Add(i);
                }
            }

            static Vector3 Converter(Vector3 ve)
            {
                //return ve;
                return new Vector3((float)Math.Round(ve.x, 3), (float)Math.Round(ve.y, 3), (float)Math.Round(ve.z, 3));
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
            dot.Vector3 += dot.Vector3 * amount;
        }
    }
}

