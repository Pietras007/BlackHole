using Geometric2.Global;
using Geometric2.ModelGeneration;
using Geometric2.RasterizationClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using OpenTK;

namespace Geometric2
{
    public partial class Form1 : Form
    {
        bool isProgramWorking = true;

        public Form1()
        {
            InitializeComponent();
            Thread thread = new Thread(() =>
            {
                while (isProgramWorking)
                {
                    glControl1.Invalidate();
                    Thread.Sleep(16);
                }
            });

            thread.Start();
            this.glControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private Shader _shader;
        private Shader _shaderXyz;
        private Camera _camera;

        private BlackHole blackHole;// = new BlackHole(_camera, glControl1.Width, glControl1.Height);
        private XyzLines xyzLines = new XyzLines();
        private List<Element> Elements = new List<Element>();
        public GlobalPhysicsData globalPhysicsData = new GlobalPhysicsData();
        int prev_xPosMouse = -1, prev_yPosMouse = -1;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isProgramWorking = false;
        }

        private void blackHoleR_Scroll(object sender, EventArgs e)
        {

        }
    }
}
