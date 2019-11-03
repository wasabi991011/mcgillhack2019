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
            mhArea.tolerance = resetParameters["tolerance"];
            mhArea.massRatio = resetParameters["mass_ratio"];
        }
    }
}
