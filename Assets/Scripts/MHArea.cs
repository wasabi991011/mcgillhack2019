using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class MHArea : Area
{
    public MHAgent mhAgent;
    public List<GameObject> masses;
    public int numberOfMasses;
    public int stringLength;
    public string logString;

    public override void ResetArea()
    {

    }

    void resetMasses()
    {
        if(masses.Count == numberOfMasses){
            foreach(GameObject mass in masses){

            }
        }
    }

    Vector3 randomSphericalCoordinate(Vector3 origin){
        float phi = Random.Range(0f, Mathf.PI / 2f);
        float theta = Random.Range(0f, 2f * Mathf.PI);
        return new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta));
    }
    public bool IsStable() {
        return false;
    }
}
