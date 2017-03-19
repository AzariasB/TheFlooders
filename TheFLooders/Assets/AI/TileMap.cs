using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using UnityEngine;

public class TileMap : MonoBehaviour {

    private GraphNode[][] _nodes;



    private float _startX;
    private float _startZ;
    private float _stepX;
    private float _stepZ;
    private float _mapWidth;
    private float _mapHeight;

    public int NodeRows;
    public int NodeColumns;

	// Use this for initialization
	void Awake () {
        _nodes = new GraphNode[NodeRows][];
        for(int i = 0; i < NodeRows; i++)
        {
            _nodes[i] = new GraphNode[NodeColumns];
        }

        TerrainHeightMap heightMap = GameObject.Find("Terrain generator").GetComponent<TerrainHeightMap>();


        _mapWidth = heightMap.Width;
        _mapHeight = heightMap.Height;

        _stepX = _mapWidth / (NodeColumns);
        _stepZ = _mapHeight / (NodeRows);

        _startX = -(_mapWidth / 2) + (_stepX / 2);
        _startZ = -(_mapHeight / 2) + (_stepZ / 2);

        //Create nodes
        for (int z = 0; z < NodeRows;z++)
        {
            for(int x = 0; x < NodeColumns; x++)
            {
                float zPos = (z * _stepZ) + _startZ;
                float xPos = (x * _stepX) + _startX;
                float yPos = (float)heightMap.GetHeight(xPos, zPos);
                Vector3 nodePos = new Vector3(xPos, yPos, zPos);
                //Check water height
                _nodes[z][x] = new GraphNode(nodePos);
            }
        }

        //Create edges
        for(int z =0; z < NodeRows; z++)
        {
            for(int x = 0; x < NodeColumns; x++)
            {
                GenerateEdge(x, z);
            }
        }

        //Debug print
        CheckBase();
	}

    /// <summary>
    /// Sinks the nodes that already are underwater
    /// </summary>
    private void CheckBase()
    {
        Material redMat = Resources.Load("RedMaterial", typeof(Material)) as Material;
        FloodControl Control = GameObject.Find("Flood water mover").GetComponent<FloodControl>();

        float waterHeight = Control.stillWaterPlane.transform.position.y;


        //Shows all the nodes
        for (int z = 0; z < NodeRows; z++)
        {
            for(int x = 0; x < NodeColumns; x++)
            {
                GraphNode mNode = _nodes[z][x];
                if (mNode != null)
                {
                    if(mNode.Position.y <= waterHeight)
                    {
                        mNode.Sink();
                    }
                }
            }
        }
    }

    private void GenerateEdge(int x, int z)
    {
        GraphNode mNode = _nodes[z][x];
        if(CanConnect(x-1, z, mNode ))
        {
            new Edge(mNode, _nodes[z][x-1]);
        }
        if(CanConnect(x+1, z, mNode) )
        {
            new Edge(mNode, _nodes[z][x + 1]);
        }
        if(CanConnect(x, z-1, mNode))
        {
            new Edge(mNode, _nodes[z - 1][x]);
        }
        if(CanConnect(x, z-1, mNode))
        {
            new Edge(mNode, _nodes[z + 1][x]);
        }
    }

    /// <summary>
    /// Checks wether a connection can be done with the node at the given coordinates
    /// If the given node already has a connection, whill return false
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="connector"></param>
    /// <returns></returns>
    private bool CanConnect(int x, int z, GraphNode connector)
    {
        return IsValidCoord(x, z) && _nodes[z][x] != null && !connector.AlreadyConnectedTo(_nodes[z][x]);
    }

    private bool IsValidCoord(int x, int z)
    {
        return x >= 0 && x < NodeColumns && z >= 0 && z < NodeRows;
    }

    public GraphNode GetNodeAtCoords(float x, float z)
    {
        if (x < _startX || x > _startX + _mapWidth || z < _startZ || z > _startZ + _mapHeight)
            return null; //404 not found

        x -= _startX;
        z -= _startZ;
        int xIndex =  (int)(x / _stepX);
        int zIndex =  (int)(z / _stepZ);

        if (xIndex < 0 || xIndex >= NodeColumns || zIndex < 0 || zIndex >= NodeRows)
            return null;

        return _nodes[zIndex][xIndex];
    }

    void Update()
    {
        FloodControl Control = GameObject.Find("Flood water mover").GetComponent<FloodControl>();
        float waterHeight = Control.stillWaterPlane.transform.position.y;
        //Shows all the nodes
        for (int z = 0; z < NodeRows; z++)
        {
            for (int x = 0; x < NodeColumns; x++)
            {


                GraphNode mNode = _nodes[z][x];
                if (mNode != null)
                {
                    foreach (Edge e in mNode.Edges)
                    {
                        if (e.IsNode1(mNode))
                        {
                            Debug.DrawLine(mNode.Position, e.Node2.Position, Color.green);
                        }
                    }

                    if (!mNode.Sinked)
                    {
                        bool rayCast;
                        RaycastHit hitPoint;

                        Vector3 target = mNode.Position;
                        Vector3 flatMapPosition = GameObject.Find("FlatMap").gameObject.transform.position;
                        Vector3 origin = new Vector3(target.x, flatMapPosition.y - 5, target.z);
                        Vector3 direction = target - origin;

                        Debug.DrawRay(origin, direction);


                        //Ray ray = Camera.main.ScreenPointToRay(mNode.Position);
                        rayCast = Physics.Raycast(origin, direction,out hitPoint, Mathf.Infinity);
                        if (rayCast &&  hitPoint.collider.gameObject.name != "HeightMap")//Can't see it => underwater
                        {
                            //Debug.DrawLine(origin, hitPoint.collider.gameObject.transform.position, Color.red);
                            mNode.Sink();
                        }

                    }
                }


            }
        }
    }

    public Queue<GraphNode> GetPath(GraphNode from, GraphNode to)
    {
        Queue<GraphNode> TraverseOrder = new Queue<GraphNode>();

        Queue<GraphNode> Q = new Queue<GraphNode>();
        HashSet<GraphNode> S = new HashSet<GraphNode>();

        Q.Enqueue(from);
        while(Q.Count > 0)
        {
            GraphNode g = Q.Dequeue();
            TraverseOrder.Enqueue(g);

            if (g == to)
            {
                print("Found dest");
                return TraverseOrder;
            }
                

            foreach(Edge e in g.Edges)
            {
                GraphNode gN = e.GetOpposite(g);
                if (!S.Contains(gN) && !gN.Sinked)
                {
                    Q.Enqueue(gN);
                    S.Add(gN);
                }
            }
        }

        return TraverseOrder;
    }
}
