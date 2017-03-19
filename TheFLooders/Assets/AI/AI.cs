using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour {

    public float CloseDistance;

    private GraphNode _finalTarget;

    private GraphNode _currentTarget;

    private List<GraphNode> _path;

	// Use this for initialization
	void Start () {
        TileMap tm = GameObject.Find("TileMap").GetComponent<TileMap>();
        _finalTarget = tm.GetNodeAtCoords(LevelInfo.Instance.Destination.x, LevelInfo.Instance.Destination.z);
        
        GraphNode startPosition = tm.GetNodeAtCoords(gameObject.transform.position.x, gameObject.transform.position.z);
        if(startPosition != null)
        {
            startPosition.DebugTrace();
            _path = tm.GetPath(startPosition, _finalTarget).ToList<GraphNode>();
            NextTarget();
            DebugPrint();
        }else
        {
            Debug.LogError("Start position not found in graph");
        }

        if (CloseDistance == 0)
            CloseDistance = 5;
        
	}
	
    void DebugPrint()
    {
        foreach(GraphNode g in _path)
        {
            g.DebugTrace();
        }
    }

    void NextTarget()
    {
        _path.RemoveAt(0);
        if (_path.Count > 0)
            _currentTarget = _path[0];
    }

	// Update is called once per frame
	void Update () {
        if (CloseEnough())
        {
            NextTarget();
        }
        else
        {
            Vector3 mPos = gameObject.transform.position;
            Vector3 flatTarget = new Vector3(_currentTarget.Position.x, 0, _currentTarget.Position.z);
            Vector3 flatPos = new Vector3(mPos.x, 0, mPos.z);
            Vector3 direction =  flatTarget - flatPos;
            //Divide ... ?
            GetComponent<Rigidbody>().velocity = direction;
        }
	}

    private bool CloseEnough()
    {
        if (_path.Count == 0)
            return false;

        Vector3 next = _path[0].Position ;
        Vector3 mPos = gameObject.transform.position;
        float distBetween = Vector3.Distance(new Vector3(next.x, 0, mPos.z), new Vector3(mPos.x, 0, mPos.z));
        return distBetween <= CloseDistance;
    }

}
