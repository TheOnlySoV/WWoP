using System;
using UnityEngine;

public interface AbilityEffect
{
    public void Activate()
    {

    }
}

[Serializable]
public class TypePull : AbilityEffect
{
    public Type typeToAttract;

    void AbilityEffect.Activate()
    {

    }
}

[Serializable]
public class FlameBody : AbilityEffect
{
    void AbilityEffect.Activate()
    {

    }
}

[Serializable]
public class TypeBoost : AbilityEffect
{
    public Type typeToAttract;
    [Range(1, 100)] public int value;

    void AbilityEffect.Activate()
    {

    }
}

[Serializable]
public class HealthCondition : AbilityEffect
{
    [Range(0, 100)] public int valueToWatch;

    void AbilityEffect.Activate()
    {

    }
}