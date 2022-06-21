using OpenTK;

namespace Geometric2.Helpers
{
    public static class HelpFunctions
    {
        public static float DeKastilio(float[] vert, float t, int degree)
        {
            for (int i = 0; i < degree; i++)
            {
                for (int j = 0; j < degree - i - 1; j++)
                {
                    vert[j] = (1 - t) * vert[j] + t * vert[j + 1];
                }
            }

            return vert[0];
        }

        public static Vector3 RotatePoint(Vector3 point, ref Quaternion rotation, ref Quaternion rotationConj)
        {
            var pointRotated = rotationConj * new Quaternion(point, 0.0f) * rotation;
            return pointRotated.Xyz;
        }
    }
}
