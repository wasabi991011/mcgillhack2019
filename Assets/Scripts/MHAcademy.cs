using UnityEngine;
using System.Collections.Generic;
using MLAgents;

public class MHAcademy : Academy
{
    public MHArea[] mhAreas;
    void Start()
    {
        mhAreas = GameObject.FindObjectsOfType<MHArea>();
    }
    public override void AcademyReset()
    {
        foreach(MHArea mhArea in mhAreas){
            mhArea.maxStartAngle = resetParameters["max_start_angle"];
            mhArea.massRatio = resetParameters["mass_ratio"];
        }
    }
}
