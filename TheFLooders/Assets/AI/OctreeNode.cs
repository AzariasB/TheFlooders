using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Four childs : 
/// 1 | 2
/// --c---
/// 3 | 4
/// </summary>
public class OctreeNode {

    private OctreeNode[] _childs;

    private int _childCount;

    private Vector3 _center;

    public GraphNode GraphNode;

    public OctreeNode(GraphNode gNode)
    {
        GraphNode = gNode;
        _childs = new OctreeNode[4];
    }

    public bool IsLeaf()
    {
        return _childCount == 0;
    }

    public bool IsComplete()
    {
        return _childCount == 4;
    }

    public void AddGraphNode(GraphNode toAdd)
    {
        if (this.IsLeaf())
        {
            _childs[0] = new OctreeNode(toAdd);
            _childCount++;
        }
        else if(IsComplete())
        {
            //...
        }
        else
        {
            _childs[_childCount] = new OctreeNode(toAdd);
        }
    }

    public GraphNode GetGraphNode(Vector3 position)
    {
        if (IsLeaf())
        {
            return GraphNode;
        }
        OctreeNode childConcerned;
        if(_center.x  < position.x)
        {
            childConcerned =  _center.z < position.z ? _childs[2] : _childs[4];
        }
        else
        {
            childConcerned = _center.z < position.z ? _childs[1] : _childs[3];
        }
        return childConcerned.GetGraphNode(position);
    }

}
