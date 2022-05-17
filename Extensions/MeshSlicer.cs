using EzySlice;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public class SlicedMesh
    {
        public Material Material { get; set; }
        public Mesh Mesh { get; set; }

        public SlicedMesh(Material mat, Mesh mesh)
        {
            Material = mat;
            Mesh = mesh;
        }
    }

    public static class MeshSlicer
    {
        public static List<SlicedMesh> Slice(this GameObject gameObject, int count)
        {
            var meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            var result = new List<SlicedMesh>();

            foreach (var meshFilter in meshFilters)
            {
                var renderer = meshFilter.GetComponent<Renderer>();

                if (renderer == null)
                {
                    continue;
                }

                var slicedMeshes = meshFilter.sharedMesh.Slice(renderer, count);

                result.AddRange(slicedMeshes);
            }    

            return result;
        }

        private static List<SlicedMesh> Slice(this Mesh sharedMesh, Renderer renderer, int count)
        {
            var slicedMeshes = new List<SlicedMesh>();
            var meshes = sharedMesh.Slice(count);

            foreach (var mesh in meshes)
            {
                var slicedMesh = new SlicedMesh(renderer.material, mesh);
                slicedMeshes.Add(slicedMesh);
            }

            return slicedMeshes;
        }

        private static List<Mesh> Slice(this Mesh mesh, int count)
        {
            var meshes = new List<Mesh>();
            Slice(meshes, mesh, count);
            return meshes;
        }

        private static void Slice(ICollection<Mesh> meshes, Mesh mesh, int count)
        {
            if (count > 1)
            {
                var hull = Slice(mesh);

                var lowerHull = hull.lowerHull;
                var upperHull = hull.upperHull;

                Slice(meshes, lowerHull, count - 1);
                Slice(meshes, upperHull, count - 1);
            }
            else if (count == 1)
            {
                var hull = Slice(mesh);

                var lowerHull = hull.lowerHull;
                var upperHull = hull.upperHull;

                meshes.Add(lowerHull);
                meshes.Add(upperHull);
            }
        }

        private static SlicedHull Slice(Mesh mesh)
        {
            var textureRegion = new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f);
            var plane = GetPlane(mesh);

            return Slicer.Slice(mesh, plane, textureRegion, 0);
        }

        private static EzySlice.Plane GetPlane(Mesh mesh)
        {
            var bounds = mesh.bounds;
            var normal = Random.insideUnitSphere;

            return new EzySlice.Plane(bounds.center, normal);
        }
    }
}
