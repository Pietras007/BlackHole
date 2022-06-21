using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.MatrixHelpers
{
    public static class ModelMatrix
    {
        public static Matrix4 CreateModelMatrix(float scale, float rotationX, float rotationY, float rotationZ, Vector3 translation)
        {
            return ScaleMatrix.CreateScaleMatrix(scale) * RotationMatrix.CreateRotationMatrix_X(rotationX) * RotationMatrix.CreateRotationMatrix_Y(rotationY) * RotationMatrix.CreateRotationMatrix_Z(rotationZ) * TranslationMatrix.CreateTranslationMatrix(translation);
        }

        public static Matrix4 CreateModelMatrix(float scale, Quaternion rotation, Vector3 translation, Vector3 rotationPoint, Quaternion tempRotation)
        {
            return ScaleMatrix.CreateScaleMatrix(scale) * Matrix4.CreateFromQuaternion(rotation) * TranslationMatrix.CreateTranslationMatrix(translation) * TranslationMatrix.CreateTranslationMatrix(-rotationPoint) * Matrix4.CreateFromQuaternion(tempRotation) * Matrix4.CreateTranslation(rotationPoint);
        }

        public static Matrix4 CreateModelMatrix(Vector3 scale, Quaternion rotation, Vector3 translation, Vector3 rotationPoint, Quaternion tempRotation)
        {
            return ScaleMatrix.CreateScaleMatrix(scale) * Matrix4.CreateFromQuaternion(rotation) * TranslationMatrix.CreateTranslationMatrix(translation) * TranslationMatrix.CreateTranslationMatrix(-rotationPoint) * Matrix4.CreateFromQuaternion(tempRotation) * Matrix4.CreateTranslation(rotationPoint);
        }
    }
}
