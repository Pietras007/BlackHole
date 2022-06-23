using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Geometric2.RasterizationClasses
{
    public class TextureCube
    {
        public readonly int Handle;

        public TextureCube(string[] textures)
        {
            Handle = GL.GenTexture();
            Use();
            int i = 0;
            foreach (var path in textures)
            {
                using (var image = new Bitmap("./../../../Resources/" + path))
                {
                    var data = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i,
                        0,
                        PixelInternalFormat.Rgba,
                        image.Width,
                        image.Height,
                        0,
                        PixelFormat.Rgba,
                        PixelType.UnsignedByte,
                        data.Scan0);
                }
                i++;
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
            GL.BindTexture(TextureTarget.TextureCubeMap, 0);
        }

        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.TextureCubeMap, Handle);
        }

        public void DeleteTexture()
        {
            GL.DeleteTexture(Handle);
            GC.SuppressFinalize(this);
        }
    }
}
