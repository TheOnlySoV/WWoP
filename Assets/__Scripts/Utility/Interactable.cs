using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public string interactTag = string.Empty;

    public UnityEvent interactEvent;

    public virtual void Interact()
    {
        //print($"{name} was interacted with");

        interactEvent.Invoke();
    }

    public virtual void Interact(Transform user)
    {
        //print($"{user.name} performed {interactTag} on {name}");

        interactEvent.Invoke();
    }
}
