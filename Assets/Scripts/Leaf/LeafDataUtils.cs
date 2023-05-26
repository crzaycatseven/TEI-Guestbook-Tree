using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;


public static class LeafDataUtils
{
    public static LeafData CreateLeafDataByID(string path, int targetID)
    {
        string[] lines = File.ReadAllLines(path);

        if (targetID >= 1 && targetID < lines.Length)
        {
            string[] values = lines[targetID].Split(',');

            if (values.Length >= 13)
            {
                int id = int.Parse(values[0]);

                LeafData leafData = new LeafData
                {
                    id = id,
                    controlPointX = float.Parse(values[1]),
                    controlPointY = float.Parse(values[2]),
                    controlPoint2X = float.Parse(values[3]),
                    controlPoint2Y = float.Parse(values[4]),
                    leafHeight = float.Parse(values[5]),
                    leafThickness = float.Parse(values[6]),
                    resolution = int.Parse(values[7]),
                    leafHue = float.Parse(values[8]),
                    leafSaturation = float.Parse(values[9]),
                    leafBrightness = float.Parse(values[10]),
                    creationDate = values[11]
                };

                return leafData;
            }
        }

        // 如果找不到对应行数的 LeafData，则返回 null 或者抛出异常，具体根据需求决定
        return null;
    }


    public static int GetLeafDataCount(string path)
    {
        string[] lines = File.ReadAllLines(path);
        return lines.Length - 1; // 减去头部行
    }


    public static List<LeafData> GetAllLeafData(string path)
    {
        List<LeafData> leafDataList = new List<LeafData>();

        string[] lines = File.ReadAllLines(path);

        for (int i = 1; i < lines.Length; i++) // 从第二行开始，跳过头部行
        {
            string[] values = lines[i].Split(',');

            if (values.Length >= 13)
            {
                int id = int.Parse(values[0]);

                LeafData leafData = new LeafData
                {
                    id = id,
                    controlPointX = float.Parse(values[1]),
                    controlPointY = float.Parse(values[2]),
                    controlPoint2X = float.Parse(values[3]),
                    controlPoint2Y = float.Parse(values[4]),
                    leafHeight = float.Parse(values[5]),
                    leafThickness = float.Parse(values[6]),
                    resolution = int.Parse(values[7]),
                    leafHue = float.Parse(values[8]),
                    leafSaturation = float.Parse(values[9]),
                    leafBrightness = float.Parse(values[10]),
                    creationDate = values[11]
                };

                leafDataList.Add(leafData);
            }
        }

        return leafDataList;
    }


    public static Mesh CreateMeshFromLeafData(LeafData data){

        int resolution = data.resolution;
        float leafThickness = data.leafThickness;
        float leafHeight = data.leafHeight;
        Vector2[] controlPoints = new Vector2[2]
        {
            new Vector2(data.controlPointX, data.controlPointY),
            new Vector2(data.controlPoint2X, data.controlPoint2Y)
        };

        Mesh mesh = new Mesh();


        Vector3[] vertices = new Vector3[resolution * 16];
        int[] triangles = new int[resolution * 24];

        for (int i = 0; i < resolution; i++)
        {

            

            float t = i / (float)resolution;
            Vector2 point = BezierCurve(t, controlPoints, leafHeight);
            float next_t = (i + 1) / (float)resolution;
            Vector2 next_point = BezierCurve(next_t, controlPoints, leafHeight);

            float realLeafThickness = leafThickness /10;

            // Front face vertices
            vertices[i * 16] = new Vector3(point.y, point.x, -realLeafThickness / 2);
            vertices[i * 16 + 1] = new Vector3(-point.y, point.x, -realLeafThickness / 2);
            vertices[i * 16 + 2] = new Vector3(next_point.y, next_point.x, -realLeafThickness / 2);
            vertices[i * 16 + 3] = new Vector3(-next_point.y, next_point.x, -realLeafThickness / 2);

            // Back face vertices
            vertices[i * 16 + 4] = new Vector3(point.y, point.x, realLeafThickness / 2);
            vertices[i * 16 + 5] = new Vector3(-point.y, point.x, realLeafThickness / 2);
            vertices[i * 16 + 6] = new Vector3(next_point.y, next_point.x, realLeafThickness / 2);
            vertices[i * 16 + 7] = new Vector3(-next_point.y, next_point.x, realLeafThickness / 2);

            // Upper edge vertices
            vertices[i * 16 + 8] = new Vector3(point.y, point.x, -realLeafThickness / 2);
            vertices[i * 16 + 9] = new Vector3(next_point.y, next_point.x, -realLeafThickness / 2);
            vertices[i * 16 + 10] = new Vector3(point.y, point.x, realLeafThickness / 2);
            vertices[i * 16 + 11] = new Vector3(next_point.y, next_point.x, realLeafThickness / 2);

            // Lower edge vertices
            vertices[i * 16 + 12] = new Vector3(-point.y, point.x, -realLeafThickness / 2);
            vertices[i * 16 + 13] = new Vector3(-next_point.y, next_point.x, -realLeafThickness / 2);
            vertices[i * 16 + 14] = new Vector3(-point.y, point.x, realLeafThickness / 2);
            vertices[i * 16 + 15] = new Vector3(-next_point.y, next_point.x, realLeafThickness / 2);



            // Front face triangles
            triangles[i * 24] = i * 16 + 1;
            triangles[i * 24 + 1] = i * 16 + 2;
            triangles[i * 24 + 2] = i * 16;

            triangles[i * 24 + 3] = i * 16 + 3;
            triangles[i * 24 + 4] = i * 16 + 2;
            triangles[i * 24 + 5] = i * 16 + 1;

            // Back face triangles
            triangles[i * 24 + 6] = i * 16 + 4;
            triangles[i * 24 + 7] = i * 16 + 6;
            triangles[i * 24 + 8] = i * 16 + 5;

            triangles[i * 24 + 9] = i * 16 + 5;
            triangles[i * 24 + 10] = i * 16 + 6;
            triangles[i * 24 + 11] = i * 16 + 7;

            // Upper edge triangles
            triangles[i * 24 + 12] = i * 16 + 8;
            triangles[i * 24 + 13] = i * 16 + 11;
            triangles[i * 24 + 14] = i * 16 + 10;

            triangles[i * 24 + 15] = i * 16 + 8;
            triangles[i * 24 + 16] = i * 16 + 9;
            triangles[i * 24 + 17] = i * 16 + 11;

            // Lower edge triangles
            triangles[i * 24 + 18] = i * 16 + 12;
            triangles[i * 24 + 19] = i * 16 + 14;
            triangles[i * 24 + 20] = i * 16 + 13;

            triangles[i * 24 + 21] = i * 16 + 13;
            triangles[i * 24 + 22] = i * 16 + 14;
            triangles[i * 24 + 23] = i * 16 + 15;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    public static Vector2 BezierCurve(float t, Vector2[] controlPoints, float leafHeight)
    {

        float controlPointX = controlPoints[0].x;
        float controlPointY = controlPoints[0].y;
        float controlPoint2X = controlPoints[1].x;
        float controlPoint2Y = controlPoints[1].y;

        // Vector2 controlPoint1 = new Vector2(controlPointY * leafHeight, controlPointX * leafHeight);
        // Vector2 controlPoint2 = new Vector2(controlPoint2Y * leafHeight, controlPoint2X * leafHeight);

        Vector2 controlPoint1 = new Vector2(controlPointY, controlPointX);
        Vector2 controlPoint2 = new Vector2(controlPoint2Y, controlPoint2X);    

        float u = 1 - t;
        Vector2 point = u * u * u * Vector2.zero;
        point += 3 * u * u * t * controlPoint1;
        point += 3 * u * t * t * controlPoint2;
        point += t * t * t * new Vector2(leafHeight, 0);
        return point;
    }


}

