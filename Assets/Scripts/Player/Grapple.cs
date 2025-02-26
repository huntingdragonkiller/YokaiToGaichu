using UnityEngine;

public class Grapple : MonoBehaviour
{
    private LineRenderer _lr;
    private Vector3 _grapplePoint;
    public LayerMask grappleLayer;
    public Transform grappleStart, player;
    private float _maxDistance = 5f;
    private SpringJoint _joint;
    public GameObject grappleIndicator;
    private ActiveObject _activeObject;

    void Awake()
    {
        _lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        GrappleIndicator();
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartGrapple();
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            StopGrapple();
        }
    }

    void LateUpdate()
    {
        DrawRope();
    }


    // Call whenever we want to start a grapple
    void StartGrapple()
    {
        Collider[] grappleObjects = Physics.OverlapSphere(transform.position, _maxDistance, grappleLayer);
        foreach (Collider grappleObject in grappleObjects)
        {
            _grapplePoint = grappleObject.transform.position;
            _joint = player.gameObject.AddComponent<SpringJoint>();
            _joint.autoConfigureConnectedAnchor = false;
            _joint.connectedAnchor = _grapplePoint;
            
            float distanceFromPoint = Vector3.Distance(gameObject.transform.position, _grapplePoint);
            
            // The distance grapple will try to keep from grapple point
            _joint.maxDistance = distanceFromPoint * 0.8f;
            _joint.minDistance = distanceFromPoint * 0.25f;

            // Change these values to fit game
            _joint.spring = 4.5f;
            _joint.damper = 7f;
            _joint.massScale = 4.5f;
            
            _lr.positionCount = 2;
        }
    }

    // Call whenever we want to stop a grapple
    void StopGrapple()
    {
        _lr.positionCount = 0;
        Destroy(_joint);
    }
    
    void DrawRope()
    {
        if (!_joint) return;
        _lr.SetPosition(0, grappleStart.position);
        _lr.SetPosition(1, _grapplePoint);
    }

    void GrappleIndicator()
    {
        Collider[] grappleObjects = Physics.OverlapSphere(transform.position, _maxDistance, grappleLayer);

        if (grappleObjects.Length > 0)
        {
            GameObject firstGrapplePoint = grappleObjects[0].gameObject; // Get the first grapple point's GameObject
            //Debug.Log("Found Grapple Point: " + firstGrapplePoint.name);
            float distanceToPlayer = Vector3.Distance(player.position, firstGrapplePoint.transform.position);
            _activeObject = firstGrapplePoint.GetComponent<ActiveObject>();

            if (distanceToPlayer <= _maxDistance)
            {
                _activeObject.ToggleOn();
            }
            else
            {
                _activeObject.ToggleOff();
            }
        }
        
    }

}
