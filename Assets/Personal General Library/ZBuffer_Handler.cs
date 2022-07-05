using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZBuffer_Handler
{
    [SerializeField] int layerRange = 5;
    [SerializeField] float layerIndividualOffset = 0.0001f;

    List<ZBufferLayer> zLayers = new List<ZBufferLayer>();

    int currentLayerStart = 0;


    public ZBufferLayer AddLayer()
    {
        ZBufferLayer newLayer = new ZBufferLayer();
        zLayers.Add(newLayer);
        currentLayerStart += layerRange;
        return newLayer;
    }

    public float getCameraZPos()
    {
        return currentLayerStart;
    }


    [System.Serializable]
    public class ZBufferLayer
    {

    }
}
