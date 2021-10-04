using Unity.Jobs;
using Jobs;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

public class BoidsBehaviour : MonoBehaviour
{
    [SerializeField] private int _count;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _treshold;
    [SerializeField] private int _velocityLimit;
    [SerializeField] private Vector3 _areaSize;
    [SerializeField] private Vector3 _weights;

    private TransformAccessArray _transformAccessArray;

    private NativeArray<Vector3> _velocities;
    private NativeArray<Vector3> _positions;
    private NativeArray<Vector3> _accelerations;


    private void Start()
    {
        CreateNativeArrays();

        InitTransformAccessArray();
        InitStartVelocity();
        
        
    }

    private void Update() => Move();

    private void Move()
    {
        var accelerationJob = CreateAccelerationJob();
        var boundsJob = CreateBoundsJob();
        var moveJob = CreateMoveJob();

        JobHandle jobHandle = boundsJob.Schedule(_count, 0);
        JobHandle accelerationJobHandle = accelerationJob.Schedule(_count, 0, jobHandle);
        JobHandle moveJobHandle = moveJob.Schedule(_transformAccessArray, accelerationJobHandle);
        moveJobHandle.Complete();
    }

    private BoundsJob CreateBoundsJob()
    {
        return new BoundsJob()
        {
            Accelerations = _accelerations,
            Positions = _positions,
            AreaSize = _areaSize
        };
    }

    private MoveJob CreateMoveJob()
    {
        return new MoveJob()
        {
            Velocities = _velocities,
            Accelerations = _accelerations,
            DeltaTime = Time.deltaTime,
            Positions = _positions,
            VelocityLimit = _velocityLimit
        };
    }
    
    private AccelerationJob CreateAccelerationJob()
    {
        return new AccelerationJob()
        {
            Accelerations = _accelerations,
            Positions = _positions,
            DestinationTreshold = _treshold,
            Velocities = _velocities,
            Weights = _weights
        };
    }

    private void OnDestroy()
    {
        _transformAccessArray.Dispose();
        _velocities.Dispose();
        _positions.Dispose();
        _accelerations.Dispose();
    }

    private void InitStartVelocity()
    {
        for (int i = 0; i < _count; i++)
            _velocities[i] = Random.insideUnitSphere;
    }

    private void InitTransformAccessArray()
    {
        var transforms = new Transform[_count];
        for (int i = 0; i < _count; i++) 
            transforms[i] = Instantiate(_prefab).transform;

        _transformAccessArray = new TransformAccessArray(transforms);
    }

    private void CreateNativeArrays()
    {
        _velocities = new NativeArray<Vector3>(_count, Allocator.Persistent);
        _positions = new NativeArray<Vector3>(_count, Allocator.Persistent);
        _accelerations = new NativeArray<Vector3>(_count, Allocator.Persistent);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, _areaSize);
    }
}