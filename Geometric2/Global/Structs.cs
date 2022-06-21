using System.Reflection;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Geometric2.Global
{
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoords;
        public Vector3 Tangent;
        public Vector3 Bitangent;
    }
}
