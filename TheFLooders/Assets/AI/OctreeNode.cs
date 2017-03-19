using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Four childs : 
/// 1 | 2
/// ------
/// 3 | 4
/// </summary>
public class OctreeNode {

    private OctreeNode[] _childs;

    //Vector3 Center ... ?
    private Vector3 _center;

    public GraphNode GraphNode;


    public OctreeNode()
    {
        _childs = new OctreeNode[4];
    }

    public bool IsLeaf()
    {
        foreach(OctreeNode child in _childs)
        {
            if (child != null)
                return false;
        }
        return true;
    }

    public GraphNode GetGraphNode(Vector3 position)
    {
        return null;
    }

}
