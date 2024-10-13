using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Highlightable : MonoBehaviour
{
    // This game object can be highlighted
    MeshRenderer renderer;
    int highlights = 0;

    [SerializeField] Material standardMat;
    [SerializeField] Material highlightMat;

    void Start()
    {
        renderer = GetComponentInChildren<MeshRenderer>();
    }

    public void Highlight()
    {
        highlights++;
        renderer.material = highlightMat;
    }

    public void Dehighlight()
    {
        highlights--;
        if (highlights == 0)
            renderer.material = standardMat;
    }
}
