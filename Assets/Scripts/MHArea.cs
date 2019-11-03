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
    public float tolerance;
    public float massRatio;

    public override void ResetArea()
    {
        ResetMasses();
    }

    void ResetMasses()
    {
        if (masses.Count == numberOfMasses)
        {
            bool isLeft = Random.Range(0, 2) == 0;

            

            for (int i = 0; i < masses.Count; i++)
            {
                Normal normalDistribution = new Normal(5, massRatio);

                masses[i].transform.GetChild(0).GetComponent<Rigidbody>().mass = (float)normalDistribution.Sample();

                if (isLeft)
                {
                    masses[i].transform.position = new Vector3(transform.position.x, transform.position.y, -4f * i - rodLength);

                    masses[i].transform.rotation = Quaternion.Euler(90f, 0, 0);
                }
                else
                {
                    masses[i].transform.position = new Vector3(transform.position.x, transform.position.y, 4f * i + rodLength);

                    masses[i].transform.rotation = Quaternion.Euler(-90f, 0, 0);
                }

                masses[i].transform.GetChild(0).GetComponent<Rigidbody>().AddForce(new Vector3(0, Random.Range(-100f, 100f), 0), ForceMode.Impulse);
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
            GameObject temporaryObject = Instantiate(massPrefab, new Vector3(transform.position.x, -4f * i - rodLength, transform.position.z), transform.rotation);

            temporaryObject.transform.parent = this.transform;

            if (i == 0)
            {
                temporaryObject.GetComponent<HingeJoint>().connectedBody = anchor.GetComponent<Rigidbody>();
            }
            else
            {
                temporaryObject.GetComponent<HingeJoint>().connectedBody = masses[i - 1].transform.GetChild(0).GetComponent<Rigidbody>();
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

    public float DistanceFromDown()
    {
        float positionPenalty = 0;
        Vector3[] diffCoords = GetDiffCoords();
        Vector3 idealDiff = new Vector3(0, -rodLength, 0);
        for (int i=0; i<numberOfMasses; i++)
        {
            positionPenalty += Vector3.Distance(idealDiff, diffCoords[i]);
        }
        positionPenalty /= (2.0f * rodLength);

        return positionPenalty;
    }

    public float DistanceFromImmobile()
    {
        float velocityPenalty = 0;
        for (int i = 0; i < numberOfMasses; i++)
        {
            velocityPenalty += Vector3.Distance(masses[i].GetComponent<Rigidbody>().velocity, Vector3.zero);

        }
        return velocityPenalty;
    }

    public bool IsStable()
    {
        return (DistanceFromDown() <= tolerance/2.0f) && (DistanceFromImmobile() <= tolerance/2.0f);
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
