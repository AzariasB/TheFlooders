using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour {

    public float StartTime;

    private float _currentTime;

    public float CloseDistance;

    private GraphNode _finalTarget;

    private GraphNode _currentTarget;

    private List<GraphNode> _path;

	// Use this for initialization
	void Start () {
        TileMap tm = GameObject.Find("TileMap").GetComponent<TileMap>();


        _finalTarget = tm.GetNodeAtCoords(30, -80);
       // print(string.Format("Arrival at  x : {0} y : {1}", LevelInfo.Instance.Destination.x, LevelInfo.Instance.Destination.y));

        Recaculate();

        if (CloseDistance == 0)
            CloseDistance = 5;
        
	}
	
    void DebugPrint()
    {
        foreach(GraphNode g in _path)
        {
            //g.DebugTrace();
        }
    }

    void NextTarget()
    {
        _path.RemoveAt(0);
        if (_path.Count > 0)
            _currentTarget = _path[0];

        if(_currentTarget == _finalTarget)
        {
            print("ended");
            Destroy(gameObject);
            //Increase survivors
            GameObject.Find("saved_player_text").GetComponent<TextBinding>().Increment();
        }
    }

    public void Recaculate()
    {
        TileMap tm = GameObject.Find("TileMap").GetComponent<TileMap>();
        GraphNode currentPosition = tm.GetNodeAtCoords(gameObject.transform.position.x, gameObject.transform.position.z);

        if (currentPosition != null)
        {
            //startPosition.DebugTrace();
            _path = tm.GetPath(currentPosition, _finalTarget).ToList<GraphNode>();
            NextTarget();
            DebugPrint();
        }
        else
        {
            Debug.LogError("Start position not found in graph");
        }
    }

    /// <summary>
    /// Checks if under the sea
    /// </summary>
    /// <returns></returns>
    private bool IsDying()
    {
        bool rayCast;
        RaycastHit hitPoint;

        Vector3 mPos = gameObject.transform.position;
        Vector3 target = new Vector3(mPos.x, 0, mPos.z);
        Vector3 flatMapPosition = GameObject.Find("FlatMap").gameObject.transform.position;
        Vector3 origin = new Vector3(target.x, flatMapPosition.y - 5, target.z);
        Vector3 direction = target - origin;

        Debug.DrawRay(origin, direction);


        //Ray ray = Camera.main.ScreenPointToRay(mNode.Position);
        rayCast = Physics.Raycast(origin, direction, out hitPoint, Mathf.Infinity);
        if (rayCast && hitPoint.collider.gameObject.name != "Terrain generator")//Can't see it => underwater
        {
            return true;
        }
        return false;
    }

	// Update is called once per frame
	void Update () {
        if (IsDying())
        {
            //Dying sound
            Destroy(gameObject);
            return;
        }

        if(_currentTime < StartTime)
        {
            _currentTime += Time.deltaTime;
        }else
        {
            if (CloseEnough())
            {
                NextTarget();
            }
            else if (_currentTarget != null)
            {
                Vector3 mPos = gameObject.transform.position;
                Vector3 flatTarget = new Vector3(_currentTarget.Position.x, 0, _currentTarget.Position.z);
                Vector3 flatPos = new Vector3(mPos.x, 0, mPos.z);
                Vector3 direction = flatTarget - flatPos;
                //Divide ... ?
                GetComponent<Rigidbody>().velocity = direction;
            }
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
