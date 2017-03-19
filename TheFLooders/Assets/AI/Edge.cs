using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Edge of non-oriented graph
/// 1-----2
/// </summary>
public class Edge {

    private GraphNode _node1;
    private GraphNode _node2;

    /// <summary>
    /// Cost of this edge, might be useless
    /// </summary>
    public int Cost { get; set; }

    public Edge(GraphNode node1 = null, GraphNode node2 = null)
    {
        _node1 = node1;
        _node2 = node2;
    }

    

}
