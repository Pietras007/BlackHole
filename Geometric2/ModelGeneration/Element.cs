using System;
using System.Drawing;
using Geometric2.Global;
using Geometric2.Helpers;
using Geometric2.MatrixHelpers;
using Geometric2.RasterizationClasses;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Geometric2.ModelGeneration
{
    public abstract class Element
    {
        public Vector3 CenterPosition { get; set; }

        public Vector3 Translation;
        public Vector3 TemporaryTranslation;

        public Quaternion RotationQuaternion = Quaternion.FromEulerAngles(0.0f, 0.0f, 0.0f);
        public Quaternion TempRotationQuaternion = Quaternion.FromEulerAngles(0.0f, 0.0f, 0.0f);

        public float ElementScale = 1.0f;
        public float TempElementScale = 1.0f;

        public float ElementRotationX, ElementRotationY, ElementRotationZ;

        public virtual void CreateGlElement(Shader _shader)
        {

        }

        public virtual void RenderGlElement(Shader _shader, Vector3 rotationCentre, GlobalPhysicsData globalPhysicsData)
        {

        }
    }
}
