using UnityEngine;
using MLAgents;
using System.Collections.Generic;

public class MHAgent : Agent
{
    public MHArea area;
    public float magnitudeMultiplier = 1;
    private string logString = "";

    void Start()
    {
    }

    public override void CollectObservations()
    {
        Vector3[] diffCoords = area.GetDiffCoords();
        for (int i=0; i<area.numberOfMasses; i++)
        {
            AddVectorObs(diffCoords[i]);
            AddVectorObs(area.masses[i].GetComponent<Rigidbody>().velocity);
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-area.DistanceFromDown() / this.agentParameters.maxStep);

        int bodyIndex = PickFromNChoices(Mathf.Clamp(vectorAction[0], -1, 1), -1, 1, area.numberOfMasses);
        Vector3 unitRod = Vector3.Normalize(area.masses[bodyIndex].transform.GetChild(0).position);
        Vector3 unitX = new Vector3(1.0f, 0, 0);
        Vector3 unitControl = Vector3.Cross(unitRod, unitX);
        area.masses[bodyIndex].transform.GetChild(0).GetComponent<Rigidbody>().AddForce(unitControl * vectorAction[1] * magnitudeMultiplier);

        area.logString += bodyIndex + ", ";

        if (area.IsStable())
        {
            AddReward(1.0f);
            Done();
        }
        
    }

    public override void AgentReset()
    {
        area.ResetArea();
    }

    public override void AgentOnDone()
    {
        this.AgentReset();
    }


    //Helper methods
    /**
     * Input: a float between min and max
     * 
     */
    private int PickFromNChoices(float flo, float min, float max, int n)
    {

        if (flo < min || flo > max)
        {
            throw new System.ArgumentException("When picking from n choices, it is necessary that" +
                " the float is between the specified min and max");
        }

        //Resize range to 0, 1
        float range = max - min;
        flo = (flo - min) / range;

        float separator = 1.0f / n;
        for (int i = 1; i <= n; i++)
        {
            if (flo <= i * separator) //so i*separator = 1/n, 2/n, 3/n, ... , (n-1)/n, 1
            {
                return i - 1;
            }
        }
        return 0;
    }
}
