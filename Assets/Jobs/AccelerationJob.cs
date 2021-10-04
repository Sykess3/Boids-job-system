using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Jobs
{
    [BurstCompile]
    public struct AccelerationJob : IJobParallelFor
    {
        [ReadOnly] 
        public NativeArray<Vector3> Velocities;
        [ReadOnly] 
        public NativeArray<Vector3> Positions;
        
        public NativeArray<Vector3> Accelerations;

        public float DestinationTreshold;
        public Vector3 Weights;
        private int Count => Velocities.Length - 1;

        public void Execute(int index)
        {
            var velocitiesSum = Vector3.zero;
            var positionsSum = Vector3.zero;
            var spreadDirectionsSum = Vector3.one;
            for (int i = 0; i < Count; i++)
            {
                if (i == index)
                    continue;

                Vector3 target = Positions[i];
                
                spreadDirectionsSum += Direction(@from: target, to: Positions[index]);
                velocitiesSum += Velocities[i];
                positionsSum += target;
            }

            Accelerations[index] += 
                Average(spreadDirectionsSum) * Weights.x
                + Average(velocitiesSum) * Weights.y
                + (Average(positionsSum) - Positions[index]) * Weights.z;
        }

        private Vector3 Direction(Vector3 from, Vector3 to)
        {
            Vector3 difference = to - @from;
            return difference.magnitude > DestinationTreshold 
                ? Vector3.zero 
                : difference.normalized;
        }
        
        private Vector3 Average(Vector3 vector3) => vector3 / Count;
    }
}