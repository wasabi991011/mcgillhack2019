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
            for (int i = 0; i < masses.Count; i++)
            {
                if (i == 0)
                {
                    masses[i].transform.position = RandomSphericalCoordinate(anchor.transform.position, rodLength);
                }
                else
                {
                    masses[i].transform.position = RandomSphericalCoordinate(masses[i - 1].transform.position, rodLength);
                }
                masses[i].GetComponent<Rigidbody>().velocity = Vector3.zero;

                Normal normalDistribution = new Normal(10, massRatio);

                masses[i].GetComponent<Rigidbody>().mass = (float)normalDistribution.Sample();
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

        for(int i = 0; i < numberOfMasses; i++){
            GameObject temporaryObject = Instantiate(massPrefab, new Vector3(transform.position.x, -5f * (i + 1), transform.position.z), Quaternion.Euler(0, 0, 0));

            if(i == 0){
                anchor.GetComponent<ConfigurableJoint>().connectedBody = temporaryObject.GetComponent<Rigidbody>();
            } else {
                masses[i - 1].GetComponent<ConfigurableJoint>().connectedBody = temporaryObject.GetComponent<Rigidbody>();
            }
    
            temporaryObject.transform.parent = this.transform;

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

    public Vector2[] GetSphericalCoords()
    {
        Vector2[] coords = new Vector2[numberOfMasses];

        Vector3 prevPos = anchor.transform.position;
        Vector3 newPos;
        Vector3 diffPos;
        float theta;
        float phi;
        for (int i=0; i<numberOfMasses; i++)
        {
            newPos = masses[i].transform.position;
            diffPos = newPos - prevPos;

            theta = Mathf.Atan2(diffPos[2], diffPos[0]);
            phi = Mathf.Asin(Mathf.Sqrt(Mathf.Pow(diffPos[2], 2) + Mathf.Pow(diffPos[0], 2)) / rodLength);

            coords[i] = new Vector2(theta, phi);
            newPos = prevPos;
        }

        return coords;

    }
}
