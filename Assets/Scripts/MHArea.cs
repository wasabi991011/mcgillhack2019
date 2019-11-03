using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class MHArea : Area
{
    public MHAgent mhAgent;
    public GameObject anchor;
    public List<GameObject> masses;
    public GameObject massPrefab;
    public int numberOfMasses;
    public int stringLength;
    public string logString;

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
                    masses[i].transform.position = RandomSphericalCoordinate(anchor.transform.position);
                }
                else
                {
                    masses[i].transform.position = RandomSphericalCoordinate(masses[i - 1].transform.position);
                }
                masses[i].transform.rotation = Quaternion.Euler(0, 0, 0);
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

        for(int i = 0; i < numberOfMasses; i++){
            masses.Add(Instantiate(massPrefab, new Vector3(transform.position.x, -5f * i, transform.position.z), Quaternion.Euler(0, 0, 0)));
        }

        ResetMasses();
    }

    Vector3 RandomSphericalCoordinate(Vector3 origin)
    {
        float phi = Random.Range(0f, Mathf.PI);
        float theta = Random.Range(0f, 2f * Mathf.PI);
        return stringLength * new Vector3(Mathf.Cos(theta) * Mathf.Sin(phi), Mathf.Cos(phi), Mathf.Cos(phi) * Mathf.Sin(theta));
    }

    public bool IsStable()
    {
        return false;
    }
}
