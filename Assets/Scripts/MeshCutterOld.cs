using System.Collections.Generic;
using UnityEngine;

namespace MeshCut
{
    public class MeshCutterOld : MonoBehaviour
    {
        public Material cutMaterial;
        public Sliceable target;


        [ContextMenu("TestCut")]
        private void TestCut()
        {
            Slice(target, new Plane(Vector3.left, 0.0f));
        }
         
        //left is used for vertices on the positive side of the plane
        private List<Vector3> leftSideVertices;
        private List<Vector3> leftSideNormals;
        private List<Vector2> leftSideUvs;

        //right is used for vertices on the negative side of the plane
        private List<Vector3> rightSideVertices;
        private List<Vector3> rightSideNormals;
        private List<Vector2> rightSideUvs;
        
        public void Slice(Sliceable target, Plane plane)
        {
            leftSideVertices = new List<Vector3>();
            leftSideNormals = new List<Vector3>();
            leftSideUvs = new List<Vector2>();

            rightSideVertices = new List<Vector3>();
            rightSideNormals = new List<Vector3>();
            rightSideUvs = new List<Vector2>();

            for (int i=0; i < target.mesh.triangles.Length; i += 3)
            {
                Vector3[] currentTriangleVertices = new Vector3[3];
                Vector3[] currentTriangleNormals = new Vector3[3];
                Vector2[] currentTriangleUvs = new Vector2[3];
                for (int j=0; j<3; j++)
                {
                    int triangleIndex = target.mesh.triangles[i + j];
                    currentTriangleVertices[j] = target.mesh.vertices[triangleIndex];
                    currentTriangleNormals[j] = target.mesh.normals[triangleIndex];
                    currentTriangleUvs[j] = target.mesh.uv[triangleIndex];
                }

                bool[] side = new bool[]
                { 
                    plane.GetSide(target.transform.TransformPoint(currentTriangleVertices[0])),
                    plane.GetSide(target.transform.TransformPoint(currentTriangleVertices[1])),
                    plane.GetSide(target.transform.TransformPoint(currentTriangleVertices[2]))
                };

                Debug.Log($"Vertex 1: {currentTriangleVertices[0]}, left side: {side[0]}");
                Debug.Log($"Vertex 2: {currentTriangleVertices[1]}, left side: {side[1]}");
                Debug.Log($"Vertex 3: {currentTriangleVertices[2]}, left side: {side[2]}");
                Debug.Log(leftSideVertices.Count);
                Debug.Log("===============================");

                if (side[0] == side[1] && side[1] == side[2])       // all vertices are on the same side
                    AddTriangle(currentTriangleVertices, currentTriangleNormals, currentTriangleUvs, side[0]);
                else
                {
                    //Split the triangle
                    Debug.Log("Taglio");
                    SplitTriangle(target, plane, side, currentTriangleVertices, leftSideVertices, leftSideNormals, leftSideUvs, rightSideVertices, rightSideNormals, rightSideUvs);
                }
            }

            CreateMesh("Left Mesh", leftSideVertices, leftSideNormals, leftSideUvs);
            CreateMesh("Right Mesh", rightSideVertices, rightSideNormals, rightSideUvs);
        }

        private void CreateMesh(string name, List<Vector3> vertices, List<Vector3> normals, List<Vector2> uvs)
        {
            GameObject obj = new GameObject(name);
            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshRenderer>();

            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(target.mesh.triangles, 0);

            obj.GetComponent<MeshFilter>().mesh = mesh;
            obj.GetComponent<MeshRenderer>().material = cutMaterial;
        }

        private void AddTriangle(Vector3[] verticesToAdd, Vector3[] normalsToAdd, Vector2[] uvsToAdd , bool positiveSide)
        {
            if (positiveSide)
                AddTriangle(verticesToAdd, normalsToAdd, uvsToAdd, leftSideVertices, leftSideNormals, leftSideUvs);
            else
                AddTriangle(verticesToAdd, normalsToAdd, uvsToAdd, rightSideVertices, rightSideNormals, rightSideUvs);
        }

        private void AddTriangle(Vector3[] verticesToAdd, Vector3[] normalsToAdd, Vector2[] uvsToAdd, List<Vector3> targetVerticesList, List<Vector3> targetNormalsList, List<Vector2> targetUvsList)
        {
            targetVerticesList.AddRange(verticesToAdd);
            targetNormalsList.AddRange(normalsToAdd);
            targetUvsList.AddRange(uvsToAdd);
        }

        private void SplitTriangle(Sliceable target, Plane plane, bool[] side, Vector3[] vertices, List<Vector3> leftVertices, List<Vector3> leftNormals, List<Vector2> leftUvs, List<Vector3> rightVertices, List<Vector3> rightNormals, List<Vector2> rightUvs)
        {
            Vector3[] intersectionPoints = new Vector3[2];
            Vector2[] intersectionUvs = new Vector2[2];
            int intersectionIndex = 0;

            for (int i = 0; i < 3; i++)
            {
                float enter;
                int index = (i + 1) % 3;

                if (side[i] != side[index])
                {
                    Ray ray = new Ray(vertices[i], vertices[index] - vertices[i]);
                    plane.Raycast(ray, out enter); // occhio qua che forse enter viene negativo
                    intersectionPoints[intersectionIndex] = Vector3.Lerp(vertices[i], vertices[index], enter);
                    intersectionUvs[intersectionIndex] = Vector3.Lerp(target.mesh.uv[i], target.mesh.uv[index], enter);
                    intersectionIndex++;
                }
            }


            //=================TEST======================

            // Separate vertices and uvs into left and right based on the plane
            List<Vector3> leftTempVertices = new List<Vector3>();
            List<Vector3> rightTempVertices = new List<Vector3>();
            List<Vector2> leftTempUvs = new List<Vector2>();
            List<Vector2> rightTempUvs = new List<Vector2>();

            for (int i = 0; i < 3; i++)
            {
                if (side[i])
                {
                    leftTempVertices.Add(vertices[i]);
                    leftTempUvs.Add(target.mesh.uv[i]);
                }
                else
                {
                    rightTempVertices.Add(vertices[i]);
                    rightTempUvs.Add(target.mesh.uv[i]);
                }
            }

            // Add intersection points to both sides
            leftTempVertices.Add(intersectionPoints[0]);
            leftTempUvs.Add(intersectionUvs[0]);
            leftTempVertices.Add(intersectionPoints[1]);
            leftTempUvs.Add(intersectionUvs[1]);

            rightTempVertices.Add(intersectionPoints[0]);
            rightTempUvs.Add(intersectionUvs[0]);
            rightTempVertices.Add(intersectionPoints[1]);
            rightTempUvs.Add(intersectionUvs[1]);

            // Forming new triangles
            if (leftTempVertices.Count == 4)
            {
                // Left side: Two new triangles
                AddTriangle(leftVertices, leftNormals, leftUvs, leftTempVertices, leftTempUvs, 0, 2, 3);
                AddTriangle(leftVertices, leftNormals, leftUvs, leftTempVertices, leftTempUvs, 0, 1, 2);

                // Right side: One new triangle
                AddTriangle(rightVertices, rightNormals, rightUvs, rightTempVertices, rightTempUvs, 0, 1, 2);
            }
            else if (rightTempVertices.Count == 4)
            {
                // Right side: Two new triangles
                AddTriangle(rightVertices, rightNormals, rightUvs, rightTempVertices, rightTempUvs, 0, 2, 3);
                AddTriangle(rightVertices, rightNormals, rightUvs, rightTempVertices, rightTempUvs, 0, 1, 2);

                // Left side: One new triangle
                AddTriangle(leftVertices, leftNormals, leftUvs, leftTempVertices, leftTempUvs, 0, 1, 2);
            }

            //===========================================
        }

        private void AddTriangle(List<Vector3> vertices, List<Vector3> normals, List<Vector2> uvs, List<Vector3> tempVertices, List<Vector2> tempUvs, int i1, int i2, int i3)
        {
            int startIndex = vertices.Count;

            vertices.Add(tempVertices[i1]);
            vertices.Add(tempVertices[i2]);
            vertices.Add(tempVertices[i3]);

            uvs.Add(tempUvs[i1]);
            uvs.Add(tempUvs[i2]);
            uvs.Add(tempUvs[i3]);

            /*
            normals.Add(startIndex);
            normals.Add(startIndex + 1);
            normals.Add(startIndex + 2);
             */
        }
    }
}
