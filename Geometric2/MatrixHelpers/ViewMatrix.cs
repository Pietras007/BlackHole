using Geometric2.RasterizationClasses;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.MatrixHelpers
{
    public static class ViewMatrix
    {
        public static Matrix4 CreateViewMatrix(Camera camera, float eyeDist = 0)
        {
            return TranslationMatrix.CreateTranslationMatrix(-camera.CameraCenterPoint) * RotationMatrix.CreateRotationMatrix_Y(camera.RotationY) * RotationMatrix.CreateRotationMatrix_X(camera.RotationX) * TranslationMatrix.CreateTranslationMatrix(new Vector3(eyeDist, 0, -camera.CameraDist));
        }
    }
}
