using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class TreeBranch
{
    public int numSegments;
    public float twistIntensity;
    public AnimationCurve twistCurve;
    public Vector3[] vertices;
    public int seed;


    public TreeBranch(int numSegments, float twistIntensity, AnimationCurve twistCurve, int seed)
    {
        this.numSegments = numSegments;
        this.twistIntensity = twistIntensity;
        this.twistCurve = twistCurve;
        this.seed = seed;
    }


    public void GenerateBranchVertices(Vector3 startPosition, float height, Vector3 growthDirection, float growingFactor)
    {
        Random.InitState(seed);
        // Random.InitState(10);
        int actualNumSegments = Mathf.FloorToInt(numSegments * growingFactor);
        float segmentHeight = height / numSegments;

        if (actualNumSegments < numSegments){
            vertices = new Vector3[actualNumSegments + 2];
        }else{
            vertices = new Vector3[actualNumSegments + 1];
        }

        float growingFactorSegment = (float) 1 / numSegments;


        Vector3 lastOffset = Vector3.zero;

        for (int i = 0; i <= actualNumSegments; i++)
        {
            float y = i * segmentHeight;
            float t = (float)i / numSegments;
            float twistAmount = twistCurve.Evaluate(t) * twistIntensity;

            Vector3 randomDirection = new Vector3(
                Mathf.PerlinNoise(y, 0) - 0.5f,
                0,
                Mathf.PerlinNoise(0, y) - 0.5f
            ).normalized;

            lastOffset += randomDirection * segmentHeight * twistAmount;
            vertices[i] = startPosition + growthDirection * y + lastOffset;
        }

        // Add interpolation
        if (actualNumSegments < numSegments)
        {
            float remainingFactor = Mathf.Repeat(growingFactor, growingFactorSegment) / growingFactorSegment;
            // float nextSegmentHeight = (actualNumSegments + 1) * segmentHeight + trunkTwistRandomNumber;
            
            float nextSegmentHeight = (actualNumSegments + 1) * segmentHeight;

            float interpolatedY = Mathf.Lerp(actualNumSegments * segmentHeight, nextSegmentHeight, remainingFactor);
            float t = (float)(actualNumSegments + 1) / numSegments;
            float growingTwistIntensity = twistIntensity * growingFactor;
            float twistAmount = twistCurve.Evaluate(t) * growingTwistIntensity;

            float y_normalized = (float) nextSegmentHeight / height;

            Vector3 randomDirection = new Vector3(
                Mathf.PerlinNoise(y_normalized, 0) - 0.5f,
                0,
                Mathf.PerlinNoise(0, y_normalized) - 0.5f
            );

            Vector3 interpolatedOffset = lastOffset + randomDirection * segmentHeight * twistAmount * remainingFactor;
            vertices[actualNumSegments + 1] = startPosition + growthDirection * interpolatedY + interpolatedOffset;
        }
    }

    public List<Vector3> GetLeafPositions(float growingFactor){

        List<Vector3> leafPositions = new List<Vector3>();

        // 根据growingFactor确定实际的叶子数量
        int actualLeafCount = Mathf.FloorToInt((vertices.Length - 1) * growingFactor);

        // 从第二个顶点开始（即排除底部的点），按照叶子生长的规律添加叶子位置
        for (int i = 1; i <= actualLeafCount; i++)
        {
            leafPositions.Add(vertices[i]);
        }

        return leafPositions;



    }
}