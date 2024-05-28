using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
[CreateAssetMenu(fileName = "New Ability", menuName = "My Assets/Ability")]
public class Ability : ScriptableObject
{
    public string description;

    [SerializeReference, SubclassSelector] public AbilityEffect[] effects;
}
