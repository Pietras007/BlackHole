﻿using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geometric2.RasterizationClasses
{
    public class Mesh
    {
        private int Vao { get; }
        private List<int> Vbos { get; } = new List<int>();
        private PrimitiveType Type { get; }
        private int Count { get; }

        public Mesh(PrimitiveType type, int[] indices, params (float[] data, int index, int size)[] buffers)
        {
            Type = type;
            Count = indices.Length;
            Vao = GL.GenVertexArray();
            GL.BindVertexArray(Vao);
            foreach (var dt in buffers.OrderBy(buffer => buffer.index))
            {
                LoadData(dt.data, dt.index, dt.size);
            }
                
            LoadIndices(indices);
            GL.BindVertexArray(0);
        }

        private void LoadData(float[] data, int index, int size)
        {
            var vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Vbos.Add(vbo);
        }

        private void LoadIndices(int[] data)
        {
            var vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(int), data, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Vbos.Add(vbo);
        }

        public void Render()
        {
            GL.BindVertexArray(Vao);
            GL.DrawElements(Type, Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
}
