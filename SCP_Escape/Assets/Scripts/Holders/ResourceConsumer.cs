using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceConsumer : MonoBehaviour
{
    public bool IsMouseOver { get; private set; }

    private void OnMouseEnter()
    {
        IsMouseOver = true;
    }

    private void OnMouseExit()
    {
        IsMouseOver = false;
    }
}
