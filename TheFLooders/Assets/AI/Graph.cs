using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph  {

    public List<GraphNode> Nodes;
    public List<Edge> Edges;

    public Graph(GraphNode entryPoint)
    {
        Nodes = new List<GraphNode>();
        Nodes.Add(entryPoint);
        ExploreNode(entryPoint);
    }

    private void ExploreNode(GraphNode node)
    {
        foreach(Edge e in node.Edges)
        {
            if (!Edges.Contains(e))
            {
                Edges.Add(e);
            }

            GraphNode op = e.GetOpposite(node);
            if (!Nodes.Contains(op))
            {
                Nodes.Add(op);
                ExploreNode(op);
            }
        }
    }
}
