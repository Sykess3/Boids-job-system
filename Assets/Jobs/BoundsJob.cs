using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Jobs
{
    [BurstCompile]
    public struct BoundsJob : IJobParallelFor
    {
        public NativeArray<Vector3> Accelerations;
        public NativeArray<Vector3> Positions;

        public Vector3 AreaSize;

        public void Execute(int index)
        {
            Vector3 position = Positions[index];
            Vector3 size = AreaSize * 0.5f;

            Accelerations[index] += Compensate(-size.x - position.x, Vector3.right)
                                   + Compensate(size.x - position.x, Vector3.left)
                                   + Compensate(-size.y - position.y, Vector3.up)
                                   + Compensate(size.y - position.y, Vector3.down)
                                   + Compensate(-size.z - position.z, Vector3.forward)
                                   + Compensate(size.z - position.z, Vector3.back);
        }

        private Vector3 Compensate(float distance, Vector3 direction)
        {
            const float treshold = 3f;
            const float multiplier = 100f;

            distance = Mathf.Abs(distance);

            if (distance > treshold)
                return Vector3.zero;

            return direction * (1 - distance / treshold) * multiplier;
        }
    }
}