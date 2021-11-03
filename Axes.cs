using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;
using System.Threading;

namespace LaboratorEGC
{
    class Axes
    {
        private float xLength;
        private float yLength;
        private float zLength;
        private float width = 1;


        public Axes()
        {
            xLength = 30;
            yLength = 30;
            zLength = 30;
        }

        public Axes(float x, float y, float z)
        {
            xLength = x;
            yLength = y;
            zLength = z;
        }
        public Axes(Vector3 xyzSize)
        {
            xLength = xyzSize.X;
            yLength = xyzSize.Y;
            zLength = xyzSize.Z;
        }

        public void setWidth(float w) { width = w;  }

        public void DrawAxes()
        {
            GL.LineWidth(width);
            // Desenează axa Ox (cu roșu).
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(xLength, 0, 0);

            // Desenează axa Oy (cu galben).
            GL.Color3(Color.Yellow);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, yLength, 0); 

            // Desenează axa Oz (cu verde).
            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, zLength);
            GL.End();
        }

    }
}
