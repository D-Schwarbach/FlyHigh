using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FlyHigh
{
public class BoundingBoxRenderer
    {
        #region Variables
        /// <summary>
        /// Vertex buffer
        /// </summary>
        public VertexBuffer Vertices;

        /// <summary>
        /// Vertex count
        /// </summary>
        public int VertexCount;

        /// <summary>
        /// Index buffer
        /// </summary>
        public IndexBuffer Indices;

        /// <summary>
        /// Primitive count
        /// </summary>
        public int PrimitiveCount;

        #endregion

        #region Constructor
        /// <summary>
        /// Creates all corners for the box
        /// </summary>
        /// <param name="boundingBox">The box to render</param>
        /// <param name="graphicsDevice">Device</param>
        /// <param name="color">Color of the box</param>
        /// <returns></returns>
        public BoundingBoxRenderer CreateBoundingBoxBuffers(BoundingBox boundingBox, GraphicsDevice graphicsDevice, Color color)
        {
            BoundingBoxRenderer bbRenderer = new BoundingBoxRenderer();

            bbRenderer.PrimitiveCount = 24;
            bbRenderer.VertexCount = 48;

            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice,
                typeof(VertexPositionColor), bbRenderer.VertexCount,
                BufferUsage.WriteOnly);
            List<VertexPositionColor> vertices = new List<VertexPositionColor>();

            // Linelength
            const float ratio = 1.0f; 

            Vector3 xOffset = new Vector3((boundingBox.Max.X - boundingBox.Min.X) / ratio, 0, 0); // X 
            Vector3 yOffset = new Vector3(0, (boundingBox.Max.Y - boundingBox.Min.Y) / ratio, 0); // Y 
            Vector3 zOffset = new Vector3(0, 0, (boundingBox.Max.Z - boundingBox.Min.Z) / ratio); // Z 
            Vector3[] corners = boundingBox.GetCorners();

            // Corner 1.
            AddVertex(vertices, corners[0], color);
            AddVertex(vertices, corners[0] + xOffset, color);
            AddVertex(vertices, corners[0], color);
            AddVertex(vertices, corners[0] - yOffset, color);
            AddVertex(vertices, corners[0], color);
            AddVertex(vertices, corners[0] - zOffset, color);

            // Corner 2.
            AddVertex(vertices, corners[1], color);
            AddVertex(vertices, corners[1] - xOffset, color);
            AddVertex(vertices, corners[1], color);
            AddVertex(vertices, corners[1] - yOffset, color);
            AddVertex(vertices, corners[1], color);
            AddVertex(vertices, corners[1] - zOffset, color);

            // Corner 3.
            AddVertex(vertices, corners[2], color);
            AddVertex(vertices, corners[2] - xOffset, color);
            AddVertex(vertices, corners[2], color);
            AddVertex(vertices, corners[2] + yOffset, color);
            AddVertex(vertices, corners[2], color);
            AddVertex(vertices, corners[2] - zOffset, color);

            // Corner 4.
            AddVertex(vertices, corners[3], color);
            AddVertex(vertices, corners[3] + xOffset, color);
            AddVertex(vertices, corners[3], color);
            AddVertex(vertices, corners[3] + yOffset, color);
            AddVertex(vertices, corners[3], color);
            AddVertex(vertices, corners[3] - zOffset, color);

            // Corner 5.
            AddVertex(vertices, corners[4], color);
            AddVertex(vertices, corners[4] + xOffset, color);
            AddVertex(vertices, corners[4], color);
            AddVertex(vertices, corners[4] - yOffset, color);
            AddVertex(vertices, corners[4], color);
            AddVertex(vertices, corners[4] + zOffset, color);

            // Corner 6.
            AddVertex(vertices, corners[5], color);
            AddVertex(vertices, corners[5] - xOffset, color);
            AddVertex(vertices, corners[5], color);
            AddVertex(vertices, corners[5] - yOffset, color);
            AddVertex(vertices, corners[5], color);
            AddVertex(vertices, corners[5] + zOffset, color);

            // Corner 7.
            AddVertex(vertices, corners[6], color);
            AddVertex(vertices, corners[6] - xOffset, color);
            AddVertex(vertices, corners[6], color);
            AddVertex(vertices, corners[6] + yOffset, color);
            AddVertex(vertices, corners[6], color);
            AddVertex(vertices, corners[6] + zOffset, color);

            // Corner 8.
            AddVertex(vertices, corners[7], color);
            AddVertex(vertices, corners[7] + xOffset, color);
            AddVertex(vertices, corners[7], color);
            AddVertex(vertices, corners[7] + yOffset, color);
            AddVertex(vertices, corners[7], color);
            AddVertex(vertices, corners[7] + zOffset, color);


            vertexBuffer.SetData(vertices.ToArray());
            bbRenderer.Vertices = vertexBuffer;

            IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice,
                IndexElementSize.SixteenBits,
                bbRenderer.VertexCount,
                BufferUsage.WriteOnly);
            indexBuffer.SetData(Enumerable.Range(0, bbRenderer.VertexCount).Select(i => (short)i).ToArray());
            bbRenderer.Indices = indexBuffer;

            return bbRenderer;
        }
        #endregion

        #region Render Box
        private static void AddVertex(List<VertexPositionColor> vertices, Vector3 position, Color color)
        {
            vertices.Add(new VertexPositionColor(position, color));
        }

        public BoundingBox CreateBoundingBox(Model spaceThog, Matrix worldTransform)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in spaceThog.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i+1], vertexData[i+2]), worldTransform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            // Create and return bounding box
            return new BoundingBox(min, max);
        }
        #endregion
    }
}
