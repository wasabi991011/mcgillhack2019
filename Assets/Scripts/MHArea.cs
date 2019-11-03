using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MathNet.Numerics.Distributions;

public class MHArea : Area
{
    public MHAgent mhAgent;
    public GameObject anchor;
    public List<GameObject> masses;
    public GameObject massPrefab;
    public int numberOfMasses;
    public int rodLength;
    public string logString;
    public float maxStartAngle;
    public float massRatio;

    public override void ResetArea()
    {
        ResetMasses();
    }

    void ResetMasses()
    {
        if (masses.Count == numberOfMasses)
        {
            Vector3 direction = RandomSphericalCoordinate(Vector3.zero, 1);

            for (int i = 0; i < masses.Count; i++)
            {
                Normal normalDistribution = new Normal(10, massRatio);

                masses[i].transform.GetChild(0).GetComponent<Rigidbody>().mass = (float)normalDistribution.Sample();

                masses[i].transform.position = direction * (i + 1) + anchor.transform.position;

                masses[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        else
        {
            InitializeMasses();
        }
    }

    void InitializeMasses()
    {
        masses = new List<GameObject>();

        for (int i = 0; i < numberOfMasses; i++)
        {
            GameObject temporaryObject = Instantiate(massPrefab, new Vector3(transform.position.x, -4f * i, transform.position.z), transform.rotation);

            temporaryObject.transform.parent = this.transform;

            if (i == 0)
            {
                temporaryObject.GetComponent<SpringJoint>().connectedBody = anchor.GetComponent<Rigidbody>();
            }
            else
            {
                temporaryObject.GetComponent<SpringJoint>().connectedBody = masses[i - 1].transform.GetChild(0).GetComponent<Rigidbody>();
            }

            masses.Add(temporaryObject);
        }

        ResetMasses();
    }

    public static Vector3 RandomSphericalCoordinate(Vector3 origin, float length)
    {
        float phi = Random.Range(0f, Mathf.PI);
        float theta = Random.Range(0f, 2f * Mathf.PI);
        return length * new Vector3(Mathf.Cos(theta) * Mathf.Sin(phi), Mathf.Cos(phi) * Mathf.Sin(theta), Mathf.Cos(phi));
    }

    public bool IsStable()
    {
        return false;
    }
    
   /**
    * Returns the coordinate of the mass with respect to its attachement point 
    * (either the anchor or mass that is one ord closer to the anchor)
    **/
    public Vector3[] GetDiffCoords()
    {
        Vector3[] coords = new Vector3[numberOfMasses];
        
        Vector3 prevPos = anchor.transform.position;
        Vector3 newPos;
        for (int i = 0; i < numberOfMasses; i++)
        {
            newPos = masses[i].transform.position;
            coords[i] = newPos - prevPos;
            newPos = prevPos;
        }

        return coords;

    }
}
