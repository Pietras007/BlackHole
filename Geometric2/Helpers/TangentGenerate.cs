using Geometric2.Global;
using OpenTK;
using System.Collections.Generic;
using System.Linq;

namespace Geometric2.Helpers
{
    public static class TangentGenerate
    {
        public static List<Vertex> GenerateModelWithTangent(float[] model, uint[] indices)
        {
            Vertex[] vertices = new Vertex[model.Length / 8];
            for (int i = 0; i < indices.Length; i += 3)
            {

                Vector3 v0 = new Vector3(model[8 * indices[i]], model[8 * indices[i] + 1], model[8 * indices[i] + 2]);
                Vector3 v1 = new Vector3(model[8 * indices[i + 1]], model[8 * indices[i + 1] + 1], model[8 * indices[i + 2] + 2]);
                Vector3 v2 = new Vector3(model[8 * indices[i + 2]], model[8 * indices[i + 2] + 1], model[8 * indices[i + 2] + 2]);

                Vector2 uv0 = new Vector2(model[8 * indices[i] + 6], model[8 * indices[i] + 7]);
                Vector2 uv1 = new Vector2(model[8 * indices[i + 1] + 6], model[8 * indices[i + 2] + 7]);
                Vector2 uv2 = new Vector2(model[8 * indices[i + 2] + 6], model[8 * indices[i + 2] + 7]);

                Vector3 deltaPos1 = v1 - v0;
                Vector3 deltaPos2 = v2 - v0;

                Vector2 deltaUV1 = uv1 - uv0;
                Vector2 deltaUV2 = uv2 - uv0;

                Vector3 tangent;
                Vector3 bitangent;
                float f = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV2.X * deltaUV1.Y);

                tangent.X = f * (deltaUV2.Y * deltaPos1.X - deltaUV1.Y * deltaPos2.X);
                tangent.Y = f * (deltaUV2.Y * deltaPos1.Y - deltaUV1.Y * deltaPos2.Y);
                tangent.Z = f * (deltaUV2.Y * deltaPos1.Z - deltaUV1.Y * deltaPos2.Z);

                bitangent.X = f * (-deltaUV2.X * deltaPos1.X + deltaUV1.X * deltaPos2.X);
                bitangent.Y = f * (-deltaUV2.X * deltaPos1.Y + deltaUV1.X * deltaPos2.Y);
                bitangent.Z = f * (-deltaUV2.X * deltaPos1.Z + deltaUV1.X * deltaPos2.Z);

                for (int k = 0; k < 3; k++)
                {
                    vertices[indices[i + k]].Position = new Vector3(model[8 * indices[i + k]], model[8 * indices[i + k] + 1], model[8 * indices[i + k] + 2]);
                    vertices[indices[i + k]].Normal = new Vector3(model[8 * indices[i + k] + 3], model[8 * indices[i + k] + 4], model[8 * indices[i + k] + 5]);
                    vertices[indices[i + k]].TextureCoords = new Vector2(model[8 * indices[i + k] + 6], model[8 * indices[i + k] + 7]);
                    vertices[indices[i + k]].Tangent += tangent;
                    vertices[indices[i + k]].Bitangent += bitangent;
                }
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Tangent.Normalize();
                vertices[i].Bitangent.Normalize();
            }

            return vertices.ToList();
        }
    }
}
