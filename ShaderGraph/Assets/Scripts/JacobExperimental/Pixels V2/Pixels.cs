using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JacobExperimental.PixelUI.V2
{
    [System.Serializable]
    public struct Pixel
    {
        public Vector2 size, position, velocity, constantVelocity;
        public float endLife;
        public Color color;
        [HideInInspector] public int bufferIndex; // Helps with finding the right draw lod.
    }

    [RequireComponent(typeof(CanvasRenderer))]
    [ExecuteAlways]
    public class Pixels : Graphic
    {
        public List<Pixel> pixelRenderBuffer = new List<Pixel>();

        public void AddParticle(Pixel p)
        {
            pixelRenderBuffer.Add(p);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            // Vertex's are better drawn CLOCKWISE!
            for (int i = 0; i < pixelRenderBuffer.Count; i++)
            {
                Pixel p = pixelRenderBuffer[i];
                p.bufferIndex = i*4;

                DrawVerticesForPixel(p, vh);
            }
        }

        private void DrawVerticesForPixel(Pixel p, VertexHelper vh)
        {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = p.color;

            // We want to position the mesh around the position not drawn from it; basically as if the position was an anchor.
            Vector2 halfedSize = p.size / 2;

            // Bottom Left
            vertex.position = p.position - halfedSize;
            vh.AddVert(vertex);

            // Top Left
            vertex.position = new Vector3(p.position.x - halfedSize.x, p.position.y + halfedSize.y);
            vh.AddVert(vertex);

            // Top Right
            vertex.position = new Vector3(p.position.x + halfedSize.x, p.position.y + halfedSize.y);
            vh.AddVert(vertex);

            // Bottom Right
            vertex.position = new Vector3(p.position.x + halfedSize.x, p.position.y - halfedSize.y);
            vh.AddVert(vertex);

            vh.AddTriangle(p.bufferIndex + 0, p.bufferIndex + 1, p.bufferIndex + 2);
            vh.AddTriangle(p.bufferIndex + 2, p.bufferIndex + 3, p.bufferIndex + 0);
        }
    }

}
