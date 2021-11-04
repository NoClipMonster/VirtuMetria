using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Entity:MonoBehaviour
{
    public Entity()
    {
        dots = new List<Dot>();
        dotsOnPlane = new DotsOnPlane();
    }
    public List<Dot> dots;
    public DotsOnPlane dotsOnPlane;
    public class Dot : Entity
    {
        public Vector3 vector3;
        public List<int> SimilarDots = new List<int>();
        public GameObject LayOutDot;
        public List<Vector3> Norms;
        public Dot(Vector3 vector, List<int> simDots, List<Vector3> norms, GameObject layDot)
        {
            vector3 = vector;
            SimilarDots = simDots;
            Norms = norms;
            LayOutDot = layDot;
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

        public DotsOnPlane() { }

        public DotsOnPlane(List<Dot> inDots, Vector3 normal, Transform transform)
        {
            if (HasPlane)
            {
                Debug.LogError("Повторное задание плоскости");
                return;
            }
            Transform = transform;
            Normal = Converter(transform.InverseTransformDirection(normal.normalized));
            Dots = inDots;
            HasPlane = true;
            indexesOfDots = new List<int>();
            for (int i = 0; i < Dots.Count; i++)
            {
                foreach (var norm in Dots[i].Norms)
                {
                    if (norm.normalized == Normal)
                    {
                        indexesOfDots.Add(i);
                        Dots[i].LayOutDot.GetComponent<Renderer>().material.color = Color.red;
                        break;
                    }
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

                Vector3 V = new Vector3(vector3.x * Normal.x, vector3.y * Normal.y, vector3.z * Normal.z);
                Dots[i].Vector3 += V;
            }


        }

        public void Excuse()
        {
            if (HasPlane)
            {
                HasPlane = false;
                foreach (int i in indexesOfDots)
                    Dots[i].LayOutDot.GetComponent<Renderer>().material.color = Color.blue;
                Normal = Vector3.zero;
                indexesOfDots = null;
                Dots = null;
            }
            else
                Debug.LogError("Удаление удалённого объекта");


        }

    }

}

