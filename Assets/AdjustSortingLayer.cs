using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdjustSortingLayer : MonoBehaviour
{
    public int sortingLayerID = 0;
    public int sortingOrder = 10;

    void Start()
    {
        gameObject.GetComponent<Renderer>().sortingLayerID = sortingLayerID;
        gameObject.GetComponent<Renderer>().sortingOrder = sortingOrder;
    }
}
