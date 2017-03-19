using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode {

	public bool Sinked { get; set; }

    public Vector3 Position;

    private ICollection<Edge> _edges;


    public ICollection<Edge> Edges
    {
        get
        {
            return _edges;
        }
    }

    
    public GraphNode(Vector3 position)
    {
        Position = position;
        _edges = new Collection<Edge>();
    }

    public void AddEge(Edge nwEdge)
    {
        _edges.Add(nwEdge);
    }

    public void RemoveEdge(Edge toRemove)
    {
        _edges.Remove(toRemove);
    }


    public bool AlreadyConnectedTo(GraphNode other)
    {
        if (other == this)
            return true;

        foreach (Edge e in _edges)
        {
            if (e.GetOpposite(this) == other)
                return true;
        }

        return false;
    }


}
