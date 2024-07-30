using System;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCut
{
    public class MeshCutter : MonoBehaviour
    {
        [SerializeField] private Sliceable theTarget;
        [SerializeField] private Transform planeTransform;

        [ContextMenu("TestCut")]
        private void TestCut()
        {
            Cut(theTarget, planeTransform.up, Vector3.zero);
        }

        public void Cut(Sliceable target, Vector3 planeNormal, Vector3 planePosition)
        {
            Plane plane = new Plane(
                target.transform.InverseTransformDirection(planeNormal),
                target.transform.InverseTransformPoint(planePosition)
                );

            CutGeneratedMesh leftMesh = new CutGeneratedMesh();     // mesh on the positive side of the plane
            CutGeneratedMesh rightMesh = new CutGeneratedMesh();    // mesh on the negative side of the plane

            bool split = false;

            // loop for each submesh in the target's mesh
            for (int i = 0; i < target.mesh.subMeshCount; i++)
            {
                int[] submeshTriangles = target.mesh.GetTriangles(i);

                // loop for each triangle in the current submesh
                for (int j = 0; j < submeshTriangles.Length; j+=3)
                {
                    MeshTriangle currentTriangle = GetTriangle(target.mesh, i, submeshTriangles, j);

                    bool[] side = new bool[]
                    {
                        plane.GetSide(currentTriangle.vertices[0]),
                        plane.GetSide(currentTriangle.vertices[1]),
                        plane.GetSide(currentTriangle.vertices[2])
                    };

                    if (side[0] == side[1] && side[1] == side[2])
                    {
                        // all vertices are on the same side of the plane (triangle can be added without cutting it)
                        if (side[0])
                            leftMesh.AddTriangle(currentTriangle);
                        else
                            rightMesh.AddTriangle(currentTriangle);
                    }
                    else
                    {
                        // split the current triangle
                        SplitTriangle(currentTriangle, side, plane, leftMesh, rightMesh);
                        split = true;
                    }
                }
            }

            // don't continue if no triangle has been cut
            if (!split)
                return;

            MeshRenderer targetRenderer = target.GetComponent<MeshRenderer>();
            FillMeshHoles(leftMesh, rightMesh, plane, targetRenderer.materials.Length);

            leftMesh.InitMesh();
            rightMesh.InitMesh();

            // right mesh will take the place of the current mesh  of the target
            // remove all original colliders
            Collider[] targetOriginalColliders = target.GetComponents<Collider>();
            foreach (Collider collider in targetOriginalColliders)
                Destroy(collider);

            target.GetComponent<MeshFilter>().mesh = rightMesh.mesh;
            target.gameObject.AddComponent<MeshCollider>().sharedMesh = rightMesh.mesh;
            Material[] newMaterials = new Material[targetRenderer.materials.Length + 1];
            targetRenderer.materials.CopyTo(newMaterials, 0);
            newMaterials[^1] = target.fillMaterial;     // ^1 access the last element of the array, it's the same as newMaterials[newMaterials.Length-1]
            targetRenderer.materials = newMaterials;

            // left mesh will be the new generated object
            GameObject leftSideGO = new GameObject(target.name+" [Left Cut]");
            leftSideGO.transform.position = target.transform.position;
            leftSideGO.transform.rotation = target.transform.rotation;
            leftSideGO.transform.localScale = target.transform.localScale;
            leftSideGO.AddComponent<MeshFilter>().mesh = leftMesh.mesh;
            leftSideGO.AddComponent<MeshRenderer>().materials = targetRenderer.materials;
            MeshCollider leftMeshCollider = leftSideGO.AddComponent<MeshCollider>();
            leftMeshCollider.sharedMesh = leftMesh.mesh;
            leftMeshCollider.convex = true;
            leftSideGO.AddComponent<Rigidbody>().AddForce(plane.normal * 50.0f);
        }

        private void FillMeshHoles(CutGeneratedMesh leftMesh, CutGeneratedMesh rightMesh, Plane plane, int subMeshIndex)
        {
            Debug.Log(leftMesh.intersectionPoints.Count);

            if (leftMesh.intersectionPoints.Count == 0) // if there are no intersection points return
                return;

            Vector3 holeCenter = Vector3.zero;
            for (int i = 0; i < leftMesh.intersectionPoints.Count; i++)
                holeCenter += leftMesh.intersectionPoints[i].vertex;
            holeCenter /= leftMesh.intersectionPoints.Count;

            for (int i = 0; i < leftMesh.intersectionPoints.Count; i += 2)
            {
                Vector3[] leftVertices = new Vector3[] { leftMesh.intersectionPoints[i].vertex, leftMesh.intersectionPoints[i + 1].vertex, holeCenter };
                Vector3[] normals = new Vector3[] { -plane.normal, -plane.normal, -plane.normal };
                Vector2[] leftUvs = new Vector2[] { leftMesh.intersectionPoints[i].uv, leftMesh.intersectionPoints[i + 1].uv, new Vector2(0.5f, 0.5f) };            // TODO: calcolare correttamente le uv altrimenti la texture nella parte tagliata si vede male !!!!!!!!!!

                MeshTriangle leftSideTriangle = new MeshTriangle(leftVertices, normals, leftUvs, subMeshIndex);

                if (IsFacingWrongSide(leftVertices, normals[0]))
                    FlipTriangle(leftSideTriangle);

                leftMesh.AddTriangle(leftSideTriangle);

                // flip the normals
                for (int j = 0; j < 3; j++)
                    normals[j] *= -1;


                Vector3[] rightVertices = new Vector3[] { rightMesh.intersectionPoints[i].vertex, rightMesh.intersectionPoints[i + 1].vertex, holeCenter };
                Vector2[] rightUvs = new Vector2[] { rightMesh.intersectionPoints[i].uv, rightMesh.intersectionPoints[i + 1].uv, new Vector2(0.5f, 0.5f) };
                MeshTriangle rightSideTriangle = new MeshTriangle(rightVertices, normals, rightUvs, subMeshIndex);


                if (IsFacingWrongSide(rightVertices, normals[0]))
                    FlipTriangle(rightSideTriangle);

                rightMesh.AddTriangle(rightSideTriangle);
            }
        }

        private void SplitTriangle(MeshTriangle targetTriangle, bool[] side, Plane plane, CutGeneratedMesh leftMesh, CutGeneratedMesh rightMesh)
        {
            IntersectionPoint[] intersectionPoints = new IntersectionPoint[2];

            int intersectionIndex = 0;
            List<int> positiveSideVertexIndex = new List<int>();
            List<int> negativeSideVertexIndex = new List<int>();

            // find intersection vertices
            for (int i = 0; i < 3; i++)
            {
                if (side[i])
                    positiveSideVertexIndex.Add(i);
                else
                    negativeSideVertexIndex.Add(i);

                int nextSide = (i + 1) % 3;
                if (side[i] == side[nextSide])      // skip if vertices are on the same side
                    continue;

                float distanceFromPlane;
                Vector3 direction = targetTriangle.vertices[nextSide] - targetTriangle.vertices[i];
                Ray ray = new Ray(targetTriangle.vertices[i], direction);
                plane.Raycast(ray, out distanceFromPlane);

                /*          RICONTROLLARE QUESTO !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                Debug.Log(distanceFromPlane);
                if (distanceFromPlane < 0)
                {
                    // CHECK IF THIS MUST BE DONE
                    Debug.Break();
                    distanceFromPlane = -distanceFromPlane;
                }
                 */

                distanceFromPlane = distanceFromPlane / direction.magnitude;    // normalize

                intersectionPoints[intersectionIndex] = new IntersectionPoint();
                intersectionPoints[intersectionIndex].vertex = Vector3.Lerp(targetTriangle.vertices[i], targetTriangle.vertices[nextSide], distanceFromPlane);
                intersectionPoints[intersectionIndex].normal = Vector3.Lerp(targetTriangle.normals[i], targetTriangle.normals[nextSide], distanceFromPlane);
                intersectionPoints[intersectionIndex].uv = Vector2.Lerp(targetTriangle.uvs[i], targetTriangle.uvs[nextSide], distanceFromPlane);
                intersectionPoints[intersectionIndex].directlyConnectedVertices[0] = targetTriangle.vertices[i];
                intersectionPoints[intersectionIndex].directlyConnectedVertices[1] = targetTriangle.vertices[nextSide];
                intersectionIndex++;
            }

            // TODO: capire come evitare di aggiungere gli stessi punti di intersezione più volte (modificare funzione AddIntersectionsToMesh)  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            leftMesh.intersectionPoints.AddRange(intersectionPoints);
            rightMesh.intersectionPoints.AddRange(intersectionPoints);

            //AddIntersectionsToMesh(leftMesh, intersectionPoints);
            //AddIntersectionsToMesh(rightMesh, intersectionPoints);

            if (positiveSideVertexIndex.Count == 1)
            {
                // handle single vertex on positive side
                leftMesh.AddTriangle(CreateSingleSideTriangle(targetTriangle, intersectionPoints, positiveSideVertexIndex[0]));

                // handle double vertices on negative side
                AddDoubleSideTriangles(targetTriangle, rightMesh, intersectionPoints, negativeSideVertexIndex);
            }
            else if (negativeSideVertexIndex.Count == 1)
            {
                // handle single vertex on negative side
                rightMesh.AddTriangle(CreateSingleSideTriangle(targetTriangle, intersectionPoints, negativeSideVertexIndex[0]));

                // handle double vertices on positive side
                AddDoubleSideTriangles(targetTriangle, leftMesh, intersectionPoints, positiveSideVertexIndex);
            }
        }

        private void AddIntersectionsToMesh(CutGeneratedMesh mesh, IntersectionPoint[] intersectionPoints)
        {
            for (int i = 0; i < intersectionPoints.Length; i++)
                if (!mesh.intersectionPoints.Contains(intersectionPoints[i]))
                    mesh.intersectionPoints.Add(intersectionPoints[i]);
        }

        private void AddDoubleSideTriangles(MeshTriangle targetTriangle, CutGeneratedMesh targetMesh, IntersectionPoint[] intersectionPoints, List<int> sideVertexIndex)
        {
            // first triangle:
            Vector3[] firstTriangleVertices = new Vector3[3] { targetTriangle.vertices[sideVertexIndex[0]], intersectionPoints[0].vertex, intersectionPoints[1].vertex };
            Vector3[] firstTriangleNormals = new Vector3[3] { targetTriangle.normals[sideVertexIndex[0]], intersectionPoints[0].normal, intersectionPoints[1].normal };
            Vector2[] firstTriangleUvs = new Vector2[3] { targetTriangle.uvs[sideVertexIndex[0]], intersectionPoints[0].uv, intersectionPoints[1].uv };
            MeshTriangle firstTriangle = new MeshTriangle(firstTriangleVertices, firstTriangleNormals, firstTriangleUvs, targetTriangle.subMeshIndex);

            if (IsFacingWrongSide(firstTriangleVertices, firstTriangleNormals[0]))
                FlipTriangle(firstTriangle);

            targetMesh.AddTriangle(firstTriangle);

            // second triangle:
            int intersectionPointIndex = 0;
            for (int i = 0; i < 2; i++)
                if (Array.IndexOf<Vector3>(intersectionPoints[i].directlyConnectedVertices, targetTriangle.vertices[sideVertexIndex[1]]) != -1)
                    intersectionPointIndex = i;

            Vector3[] secondTriangleVertices = new Vector3[3] { targetTriangle.vertices[sideVertexIndex[1]], targetTriangle.vertices[sideVertexIndex[0]], intersectionPoints[intersectionPointIndex].vertex };
            Vector3[] secondTriangleNormals = new Vector3[3] { targetTriangle.normals[sideVertexIndex[1]], targetTriangle.normals[sideVertexIndex[0]], intersectionPoints[intersectionPointIndex].normal };
            Vector2[] secondTriangleUvs = new Vector2[3] { targetTriangle.uvs[sideVertexIndex[1]], targetTriangle.uvs[sideVertexIndex[0]], intersectionPoints[intersectionPointIndex].uv };
            MeshTriangle secondTriangle = new MeshTriangle(secondTriangleVertices, secondTriangleNormals, secondTriangleUvs, targetTriangle.subMeshIndex);

            if (IsFacingWrongSide(secondTriangleVertices, secondTriangleNormals[0]))
                FlipTriangle(secondTriangle);

            targetMesh.AddTriangle(secondTriangle);
        }

        private MeshTriangle CreateSingleSideTriangle(MeshTriangle targetTriangle, IntersectionPoint[] intersectionPoints, int index)
        {
            Vector3[] newTriangleVertices = new Vector3[3] { targetTriangle.vertices[index], intersectionPoints[0].vertex, intersectionPoints[1].vertex };
            Vector3[] newTriangleNormals = new Vector3[3] { targetTriangle.normals[index], intersectionPoints[0].normal, intersectionPoints[1].normal };
            Vector2[] newTriangleUvs = new Vector2[3] { targetTriangle.uvs[index], intersectionPoints[0].uv, intersectionPoints[1].uv };

            MeshTriangle output = new MeshTriangle(newTriangleVertices, newTriangleNormals, newTriangleUvs, targetTriangle.subMeshIndex);

            if (IsFacingWrongSide(newTriangleVertices, newTriangleNormals[0]))
                FlipTriangle(output);

            return output;
        }

        private bool IsFacingWrongSide(Vector3[] triangleVertices, Vector3 triangleNormal)
        {
            return Vector3.Dot(Vector3.Cross(triangleVertices[0] - triangleVertices[1], triangleVertices[0] - triangleVertices[2]), triangleNormal) < 0;
        }

        private void FlipTriangle(MeshTriangle triangle)
        {
            Vector3 temp = triangle.vertices[0];

            triangle.vertices[0] = triangle.vertices[1];
            triangle.vertices[1] = temp;

            temp = triangle.normals[0];
            triangle.normals[0] = triangle.normals[1];
            triangle.normals[1] = temp;

            Vector2 temp2 = triangle.uvs[0];

            triangle.uvs[0] = triangle.uvs[1];
            triangle.uvs[1] = temp2;

        }

        private MeshTriangle GetTriangle(Mesh targetMesh, int submeshIndex, int[] submeshTriangles ,int triangleIndexOffset)
        {
            Vector3[] vertices = new Vector3[3];
            Vector3[] normals = new Vector3[3];
            Vector2[] uvs = new Vector2[3];

            for (int i = 0; i < 3; i++)
            {
                int index = submeshTriangles[triangleIndexOffset + i];
                vertices[i] = targetMesh.vertices[index];
                normals[i] = targetMesh.normals[index];
                uvs[i] = targetMesh.uv[index];
            }

            return new MeshTriangle(vertices, normals, uvs, submeshIndex);
        }
    }
}
