using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode {

    public bool Sinked;

    public Vector3 Position;

    private List<Edge> _edges;


    public List<Edge> Edges
    {
        get
        {
            return _edges;
        }
    }

    
    public GraphNode(Vector3 position)
    {
        Position = position;
        _edges = new List<Edge>();
        Sinked = false;
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

    public void Sink()
    {
        Sinked = true;
        for(int i = _edges.Count - 1; i >= 0; i--)
        {
            _edges[i].BreakEdge();
        }
    }

    public void DebugTrace(string name = "")
    {
        Material greenMat = Resources.Load("RedMaterial", typeof(Material)) as Material;
        Material redMat = Resources.Load("GreenMaterial", typeof(Material)) as Material;

        GameObject debug = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        if(!string.IsNullOrEmpty(name) )
            debug.name = name;

        GameObject.Destroy(debug.GetComponent<SphereCollider>());
        debug.layer = LayerMask.NameToLayer("Ignore Raycast");
        debug.transform.position = Position;
        debug.GetComponent<Renderer>().material = Sinked ? redMat : greenMat;
        Debug.Log(string.Format("x : {0} z : {1}", Position.x, Position.z));
    }

}
