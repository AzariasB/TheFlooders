using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode {

    public bool Sinked;

    public Vector3 Position;

    private List<Edge> _edges;

    private GameObject debug;

    /// <summary>
    /// Weight of this node, the higher, the better
    /// The weight is the relation between the position of 
    /// the node and the destination point of the level
    /// </summary>
    public float Weight { get; set; }


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
        Object.Destroy(debug);
    }

    public void DebugTrace(string name = "")
    {
        if(this.debug != null)
            Object.Destroy(this.debug);
        
        Material greenMat = Resources.Load("RedMaterial", typeof(Material)) as Material;
        Material redMat = Resources.Load("GreenMaterial", typeof(Material)) as Material;

        debug = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        if(!string.IsNullOrEmpty(name) )
            debug.name = name;

        Object.Destroy(debug.GetComponent<SphereCollider>());
        debug.layer = LayerMask.NameToLayer("Ignore Raycast");
        debug.transform.position = Position;
        debug.GetComponent<Renderer>().material = Sinked ? redMat : greenMat;
       // Debug.Log(string.Format("x : {0} z : {1}", Position.x, Position.z));
    }

}
