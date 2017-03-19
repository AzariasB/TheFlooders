using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Edge of non-oriented graph
/// 1-----2
/// </summary>
public class Edge {

    public GraphNode Node1;
    public GraphNode Node2;

    /// <summary>
    /// Cost of this edge, might be useless
    /// </summary>
    public int Cost { get; set; }

    public Edge(GraphNode node1 = null, GraphNode node2 = null)
    {
        Node1 = node1;
        Node2 = node2;

        Node1.AddEge(this);
        Node2.AddEge(this);
    }

    /// <summary>
    /// Called when two graph nodes are not connected together
    /// </summary>
    public void BreakEdge()
    {
        Node1.RemoveEdge(this);
        Node2.RemoveEdge(this);
    }

    public GraphNode GetOpposite(GraphNode from)
    {
        return from == Node1 ? Node2 : Node1;
    }
}
