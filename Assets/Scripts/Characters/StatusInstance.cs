using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusInstance
{
    [SerializeField] Status status;
    [SerializeField] int duration;
    [SerializeField] EffectTarget target;

    // Properties
    public Status Status {get {return status;} }
    public int Duration {get {return duration;} }
    public EffectTarget Target {get {return target;} }

    // Status Methods
    public void ActivateStatusEffect()
    {

    }

}
