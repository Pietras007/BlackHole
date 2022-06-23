using System;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL4;
using OpenTK;
using System.Drawing;
using Geometric2.Global;
using Geometric2.RasterizationClasses;
using System.Collections.Generic;
using Geometric2.Helpers;
using Geometric2.ModelGeneration;

namespace Geometric2
{
    public partial class Form1 : Form
    {
        private void glControl1_Load(object sender, EventArgs e)
        {
            _camera = new Camera(new Vector3(0, 5, 15), glControl1.Width / (float)glControl1.Height);
            blackHole = new BlackHole(_camera, glControl1.Width, glControl1.Height);
            Elements.Add(blackHole);
            Elements.Add(xyzLines);
            blackHole.first_globalPhysicsData = globalPhysicsData;
            GL.ClearColor(Color.LightCyan);
            GL.Enable(EnableCap.DepthTest);
            _shader = new Shader("./../../../Shaders/VertexShader.vert", "./../../../Shaders/FragmentShader.frag");
            _shaderXyz = new Shader("./../../../Shaders/VertexShaderLines.vert", "./../../../Shaders/FragmentShaderLines.frag");

            foreach (var el in Elements)
            {
                if (el is XyzLines)
                {
                    el.CreateGlElement(_shaderXyz);
                }
                else
                {
                    el.CreateGlElement(_shader);
                }
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Matrix4 viewMatrix = _camera.GetViewMatrix();
            Matrix4 projectionMatrix = _camera.GetProjectionMatrix();
            RenderScene(viewMatrix, projectionMatrix);

            GL.Flush();
            glControl1.SwapBuffers();
        }

        private void RenderScene(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            _shaderXyz.Use();
            _shaderXyz.SetMatrix4("view", viewMatrix);
            _shaderXyz.SetMatrix4("projection", projectionMatrix);
            _shaderXyz.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));

            var camPos = _camera.GetCameraPosition();
            //_shaderLight.SetVector3("viewPos", camPos);

            foreach (var el in Elements)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                if (el is XyzLines)
                {
                    el.RenderGlElement(_shaderXyz, new Vector3(0, 0, 0), globalPhysicsData);
                }
                else
                {
                    el.RenderGlElement(_shader, new Vector3(0, 0, 0), globalPhysicsData);
                }
            }
        }

        private void glControl1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
        }

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            int xPosMouse, yPosMouse;
            if (e.Button == MouseButtons.Middle)
            {
                xPosMouse = e.X;
                yPosMouse = e.Y;
                if (prev_xPosMouse != -1 && prev_yPosMouse != -1)
                {
                    var deltaX = xPosMouse - prev_xPosMouse;
                    var deltaY = yPosMouse - prev_yPosMouse;

                    _camera.RotationX -= (float)(2 * Math.PI * deltaY / glControl1.Height);
                    _camera.RotationY += (float)(2 * Math.PI * deltaX / glControl1.Width);

                }

                prev_xPosMouse = xPosMouse;
                prev_yPosMouse = yPosMouse;
            }

            if (e.Button == MouseButtons.Right)
            {
                xPosMouse = e.X;
                yPosMouse = e.Y;
                if (prev_xPosMouse != -1 && prev_yPosMouse != -1)
                {
                    Matrix4 viewMatrix = _camera.GetViewMatrix();
                    Matrix4 projectionMatrix = _camera.GetProjectionMatrix();
                    Vector3 prevMousePos = GetCoursorGlobalPosition((prev_xPosMouse, prev_yPosMouse), viewMatrix, projectionMatrix, _camera);
                    Vector3 currentMousePos = GetCoursorGlobalPosition((xPosMouse, yPosMouse), viewMatrix, projectionMatrix, _camera);
                    Vector3 mouseMove = currentMousePos - prevMousePos;
                }

                prev_xPosMouse = xPosMouse;
                prev_yPosMouse = yPosMouse;
            }
        }

        private float clamp(float val, float min, float max)
        {
            if (val < min)
            {
                return min;
            }

            if (val > max)
            {
                return max;
            }

            return val;
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle || e.Button == MouseButtons.Left)
            {
                prev_xPosMouse = -1;
                prev_yPosMouse = -1;
            }
        }

        private void glControl1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 200;
            if (_camera.CameraDist - numberOfTextLinesToMove >= 0.0f)
            {
                _camera.CameraDist -= numberOfTextLinesToMove;
            }
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void glControl1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
        }

        private void glControl1_KeyUp(object sender, KeyEventArgs e)
        {
        }

        public Vector3 GetCoursorGlobalPosition((int, int) screenPos, Matrix4 viewMatrix, Matrix4 projectionMatrix, Camera _camera)
        {
            (float, float, float) screenFloatPos = GetScreenFloatPos(screenPos);
            return CountCoursorPos(screenFloatPos, viewMatrix, projectionMatrix, _camera);
        }

        private Vector3 CountCoursorPos((float, float, float) screenFloatPos, Matrix4 viewMatrix, Matrix4 projectionMatrix, Camera _camera)
        {
            Vector4 coursor = new Vector4(screenFloatPos.Item1, screenFloatPos.Item2, screenFloatPos.Item3, 1.0f) * Matrix4.Invert(projectionMatrix);
            coursor.Z = -1.0f;
            coursor.W = 0.0f;
            coursor = coursor * Matrix4.Invert(viewMatrix);
            coursor = coursor.Normalized();

            float R = _camera.CameraDist;
            coursor.X = _camera.GetCameraPosition().X + coursor.X * R;
            coursor.Y = _camera.GetCameraPosition().Y + coursor.Y * R;
            coursor.Z = _camera.GetCameraPosition().Z + coursor.Z * R;

            return coursor.Xyz;
        }
        private (float, float, float) GetScreenFloatPos((int, int) screenPos)
        {
            float x = (float)((2.0 / glControl1.Width) * screenPos.Item1 - 1);
            float y = -(float)((2.0 / glControl1.Height) * screenPos.Item2 - 1);
            float z = -1.0f;

            return (x, y, z);
        }
    }
}
