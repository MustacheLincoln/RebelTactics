using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class CursorGrid : MonoBehaviour
{
    public Camera _camera;
    [SerializeField] GameObject _gridTargetIcon;
    public GameObject _walkingRangeIcon;
    [SerializeField] GameManager _gameManager;

    readonly Queue<GameObject> _moveIcons = new Queue<GameObject>();

    LineRenderer _lineRenderer;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _gridTargetIcon = Instantiate(_gridTargetIcon, Vector3.zero, Quaternion.Euler(90, 0, 0));
    }

    public IEnumerator CalculateRange(EntityController entity)
    {
        ClearRange();
        yield return new WaitForSeconds(.1f);
        var pos = entity.transform.position;
        var max = entity.maxMoveDistance;
        for (float x = pos.x - max; x <= pos.x + max; x++)
            for( float y = pos.y - max; y <= pos.y + max; y++)
                for (float z = pos.z - max; z <= pos.z + max; z++)
                {
                    Vector3 _targetPosition = new Vector3(x, y + .01f, z);
                    if (IsSpaceFree(entity, pos,_targetPosition, false))
                        MovementIconQueue(_targetPosition);
                }
    }

    bool IsSpaceFree(EntityController entity, Vector3 _sourcePosition, Vector3 _targetPosition, bool drawing)
    {
        if (_targetPosition == new Vector3(_sourcePosition.x, _sourcePosition.y + .01f, _sourcePosition.z))
            return false;
        if (NavMesh.SamplePosition(_targetPosition, out NavMeshHit navMesHit, .5f, NavMesh.AllAreas))
        {
            var path = new NavMeshPath();
            NavMesh.CalculatePath(_sourcePosition, navMesHit.position, NavMesh.AllAreas, path);
            float _pathLength = 0;
            for (int i = 1; i < path.corners.Length; ++i)
                _pathLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            if (_pathLength <= entity.maxMoveDistance && path.status == NavMeshPathStatus.PathComplete)
            {
                if (drawing)
                    DrawPath(path);
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }

    void MovementIconQueue(Vector3 pos)
    {
        if (_moveIcons.Count == 0 || _moveIcons.Peek().activeInHierarchy)
        {
            var icon = Instantiate(_walkingRangeIcon, pos, Quaternion.Euler(90, 0, 0));
            _moveIcons.Enqueue(icon);
        }
        else
        {
            var icon = _moveIcons.Dequeue();
            icon.SetActive(true);
            icon.transform.position = pos;
            _moveIcons.Enqueue(icon);
        }
    }

    public void CaptureCursor(EntityController entity)
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit rayCastHit) && (NavMesh.SamplePosition(rayCastHit.point, out NavMeshHit navMeshHit, .5f, NavMesh.AllAreas))) //Line flickers without this redundantcy
        {
            Vector3 position = Vector3Int.FloorToInt(navMeshHit.position);
            Vector3 gridCenterOffset = new Vector3(0.5f, 0.01f, 0.5f);
            Vector3 destination = position + gridCenterOffset;
            if (IsSpaceFree(entity, entity.transform.position, destination, true))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    entity.Move(destination);
                    ClearRange();
                    ClearPath();
                }
            }
            else
                ClearPath();
        }
        else
            ClearPath();
    }

    void DrawPath(NavMeshPath path)
    {
        Vector3[] corners = path.corners;
        _lineRenderer.SetPositions(corners);
        _lineRenderer.positionCount = corners.Length;
        var destination = path.corners[path.corners.Length - 1];
        _gridTargetIcon.SetActive(true);
        _gridTargetIcon.transform.position = destination;
    }

    public void ClearRange()
    {
        foreach (GameObject icon in _moveIcons)
            icon.SetActive(false);
    }

    public void ClearPath()
    {
        _gridTargetIcon.SetActive(false);
        _lineRenderer.positionCount = 0;
    }
}
