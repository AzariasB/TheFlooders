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

        Recalculate();

        if (CloseDistance == 0)
            CloseDistance = 1;
        
	}
	
    void DebugPrint()
    {
        foreach(GraphNode g in _path)
        {
            g.DebugTrace(true);
        }
    }

    void NextTarget()
    {
        if(_currentTarget != null)
            _currentTarget.RemoveDebug();
        
        if(_path.Count > 0)
            _path.RemoveAt(0);

            
        if (_path.Count > 0)
        {
            _currentTarget = _path[0];
            
        }

        if(_currentTarget == _finalTarget)
        {
            print("ended");
            Destroy(gameObject);
            //Increase survivors
			Debug.Log("Finished !");
			//GameObject.Find("saved_player_text").GetComponent<PowerUsesCounter>().Increment();
        }
    }

    public void FixedUpdate()
    {
        Recalculate();
    }
    
    public void Recalculate()
    {
        TileMap tm = GameObject.Find("TileMap").GetComponent<TileMap>();
        GraphNode currentPosition = tm.GetNodeAtCoords(gameObject.transform.position.x, gameObject.transform.position.z);

        if (currentPosition != null)
        {
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
        Vector3 mPos = gameObject.transform.position;
        return (LevelInfo.Instance.Ground_eau.GetHeight(mPos.x, mPos.z) >=
            LevelInfo.Instance.Ground.GetHeight(mPos.x, mPos.z));
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
                GetComponent<Rigidbody>().velocity = direction /  10;
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
