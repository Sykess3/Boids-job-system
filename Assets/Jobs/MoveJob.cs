using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace Jobs
{
    [BurstCompile]
    public struct MoveJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> Accelerations;
        [WriteOnly]
        public NativeArray<Vector3> Positions;
        
        public NativeArray<Vector3> Velocities;

        public int VelocityLimit;

        public float DeltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            
            var velocity = Velocities[index] + Accelerations[index] * DeltaTime;
            var direction = velocity.normalized;

            velocity = direction * Mathf.Clamp(velocity.magnitude, 1f, VelocityLimit);

            transform.position += velocity * DeltaTime;
            transform.rotation = Quaternion.LookRotation(direction);

            Positions[index] = transform.position;
            Velocities[index] = velocity;
            Accelerations[index] = Vector3.zero;
        }
    }
}