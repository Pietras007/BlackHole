using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.Helpers
{
    public static class QuaternionExtensions
    {
        public static Quaternion ConvertToQuaternion(this Quaterniond quaternion)
        {
            var q = new Quaternion((float)quaternion.X, (float)quaternion.Y, (float)quaternion.Z, (float)quaternion.W);
            return q;
        }

        public static Quaternion ConvertToQuaternionAndNormalize(this Quaterniond quaternion)
        {
            var q = new Quaternion((float)quaternion.X, (float)quaternion.Y, (float)quaternion.Z, (float)quaternion.W);
            q.Normalize();
            return q;
        }
    }
}
