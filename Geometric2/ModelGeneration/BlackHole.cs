using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Geometric2.Global;
using Geometric2.Helpers;
using Geometric2.MatrixHelpers;
using Geometric2.RasterizationClasses;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Geometric2.ModelGeneration
{
    public class BlackHole : Element
    {
        public float[] Vertices { get; protected set; }
        public uint[] Indices { get; protected set; }

        public int cubeVBO, cubeVAO, cubeEBO;
        private Camera _camera;
        TextureCube textureCube;
        public GlobalPhysicsData first_globalPhysicsData;

        private float mass = 1f;
        private Vector3 blackHolePosition = new Vector3(0, 0, 0);
        private int _width, _height;
        private float size = 500f;

        public BlackHole(Camera _camera, int width, int height)
        {
            this._camera = _camera;
            this._width = width;
            this._height = height;
        }

        public override void CreateGlElement(Shader _shader)
        {
            InitializeSkyBox(_shader);
            (string Path, TextureTarget side)[] textures =
            {
                ("right.png", TextureTarget.TextureCubeMapNegativeX),
                ("left.png", TextureTarget.TextureCubeMapNegativeY),
                ("top.png", TextureTarget.TextureCubeMapNegativeZ),
                ("bottom.png", TextureTarget.TextureCubeMapPositiveX),
                ("back.png", TextureTarget.TextureCubeMapPositiveY),
                ("front.png", TextureTarget.TextureCubeMapPositiveZ),
            };
            textureCube = new TextureCube(textures);

            GL.ClearColor(Color.LightCyan);
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, GlobalPhysicsData globalPhysicsData)
        {
            _shader.Use();
            textureCube.Use();

            Matrix4 viewMatrix = _camera.GetViewMatrix();
            Matrix4 projectionMatrix = _camera.GetProjectionMatrix();
            _shader.SetMatrix4("view", viewMatrix);
            _shader.SetMatrix4("projection", projectionMatrix);
            //_shader.SetVector3("position", _camera.GetCameraPosition());
            _shader.SetVector2("resolution", new Vector2(_width, _height));
            _shader.SetMatrix4("invView", _camera.GetProjectionViewMatrix().Inverted());
            _shader.SetFloat("mass", mass);
            //_shader.SetFloat("size", size);
            _shader.SetVector3("blackHolePosition", blackHolePosition);

            GL.BindVertexArray(cubeVAO);
            GL.ActiveTexture(TextureUnit.Texture0);
            _shader.SetInt("sampler", 0);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        private void InitializeSkyBox(Shader _shader)
        {
            _shader.Use();
            var skyBoxVertices = new float[]
               {
                -size, -size, -size,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
                 size, -size, -size,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
                 size,  size, -size,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
                 size,  size, -size,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
                -size,  size, -size,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
                -size, -size, -size,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

                -size, -size,  size,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
                 size, -size,  size,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
                 size,  size,  size,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
                 size,  size,  size,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
                -size,  size,  size,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,
                -size, -size,  size,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,

                -size,  size,  size, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                -size,  size, -size, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
                -size, -size, -size, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                -size, -size, -size, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                -size, -size,  size, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
                -size,  size,  size, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

                 size,  size,  size,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                 size,  size, -size,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
                 size, -size, -size,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                 size, -size, -size,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                 size, -size,  size,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
                 size,  size,  size,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

                -size, -size, -size,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
                 size, -size, -size,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
                 size, -size,  size,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
                 size, -size,  size,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
                -size, -size,  size,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
                -size, -size, -size,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

                -size,  size, -size,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
                 size,  size, -size,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
                 size,  size,  size,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
                 size,  size,  size,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
                -size,  size,  size,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
                -size,  size, -size,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
               };

            uint[] _skyBoxIndices = new uint[36];
            for (int i = 0; i < _skyBoxIndices.Length; i++)
            {
                _skyBoxIndices[i] = (uint)i;
            }

            Vertices = skyBoxVertices;
            Indices = _skyBoxIndices;

            cubeVAO = GL.GenVertexArray();
            cubeVBO = GL.GenBuffer();
            cubeEBO = GL.GenBuffer();
            GL.BindVertexArray(cubeVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, cubeVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, cubeEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);
            var a_Position_Location = _shader.GetAttribLocation("a_Position");
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);
            var aNormal = _shader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(aNormal);
            GL.VertexAttribPointer(aNormal, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            var aTexCoords = _shader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(aTexCoords);
            GL.VertexAttribPointer(aTexCoords, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        }
    }
}
