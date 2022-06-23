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

        private Vector3 blackHolePosition = new Vector3(0,0,1000);
        private int _width, _height;

        public BlackHole(Camera _camera, int width, int height)
        {
            this._camera = _camera;
            this._width = width;
            this._height = height;
        }

        public override void CreateGlElement(Shader _shader)
        {
            InitializeSkyBox(_shader);

            string[] textures =
            {
                "right.png",
                "left.png", 
                "top.png", 
                "bottom.png",
                "back.png",
                "front.png",
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

            _shader.SetInt("sampler", 0);
            _shader.SetVector2("resolution", new Vector2(_width, _height));
            _shader.SetMatrix4("invView", _camera.GetProjectionViewMatrix().Inverted());
            _shader.SetFloat("mass", globalPhysicsData.blackHoleMass);
            _shader.SetVector3("blackHolePosition", globalPhysicsData.blackHolePosition);

            GL.BindVertexArray(cubeVAO);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        private void InitializeSkyBox(Shader _shader)
        {
            Vertices = new float[] {
                1,  1, 0,
                1, -1, 0,
                -1, -1, 0,
                -1,  1, 0
            };
            Indices = new uint[] {
                0, 1, 3,
                1, 2, 3
            };

            cubeVAO = GL.GenVertexArray();
            GL.BindVertexArray(cubeVAO);

            cubeVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, cubeVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            var vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
    }
}
