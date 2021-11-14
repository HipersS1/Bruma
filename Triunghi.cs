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
    class Triunghi
    {
        private Vector3 v0;
        private Vector3 v1;
        private Vector3 v2;
        
        public Triunghi() { }
 
        public Triunghi(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            this.v0 = new Vector3(v0);
            this.v1 = new Vector3(v1);
            this.v2 = new Vector3(v2);
        }

        public Vector3 getV0() { return v0; }
        public Vector3 getV1() { return v1; }
        public Vector3 getV2() { return v2; }

        public void DrawTriangle()
        {
            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(Color.Cyan);
            GL.Vertex3(v0);
            GL.Vertex3(v1);
            GL.Vertex3(v2);
            GL.End();
        }
        public static void DrawTriangle(Triunghi t, Color colorV0 = default(Color))
        {
            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(colorV0);
            GL.Vertex3(t.getV0());

            GL.Vertex3(t.getV1());
            GL.Vertex3(t.getV2());
            GL.End();
        }

        public static void DrawTriangle(Triunghi t, Color4 color4)
        {
            GL.Begin(PrimitiveType.Triangles);
            GL.Color4(color4);
            GL.Vertex3(t.getV0());
            GL.Vertex3(t.getV1());
            GL.Vertex3(t.getV2());
            GL.End();
        }

        public static void DrawTriangle(Triunghi t, Color colorV0 = default(Color), Color colorV1 = default(Color), Color colorV2 = default(Color))
        {
            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(colorV0);
            GL.Vertex3(t.getV0());

            GL.Color3(colorV1);
            GL.Vertex3(t.getV1());

            GL.Color3(colorV2);
            GL.Vertex3(t.getV2());
            GL.End();
        }
        public static void DrawTriangle(Triunghi t, int colorRed, int colorGreen, int colorBlue)
        {
            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(Color.FromArgb(colorRed, 0, 0));
            GL.Vertex3(t.getV0());

            GL.Color3(Color.FromArgb(0, colorGreen, 0));
            GL.Vertex3(t.getV1());

            GL.Color3(Color.FromArgb(0, 0, colorBlue));
            GL.Vertex3(t.getV2());
            GL.End();
        }

        public static void DrawTriangle(Triunghi t)
        {
            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Color3(Color.Red);
            GL.Vertex3(t.getV0());

            GL.Color3(Color.Green);
            GL.Vertex3(t.getV1());

            GL.Color3(Color.Blue);
            GL.Vertex3(t.getV2());
            GL.End();
        }


        public void Translate(Vector3 translateVector)
        {
            v0 += translateVector;
            v1 += translateVector;
            v2 += translateVector;
        }

        public static Triunghi ReadFileTriangle(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
            string[] infoCord;
            Triunghi triunghiFisier;
            Vector3[] vectorList = new Vector3[3];
            int i = 0;
            foreach (string line in lines)
            {
                infoCord = line.Split(' ');
                vectorList[i] = new Vector3((float)Convert.ToDouble(infoCord[0]), (float)Convert.ToDouble(infoCord[1]), (float)Convert.ToDouble(infoCord[2]));
                i++;
            }
            triunghiFisier = new Triunghi(vectorList[0], vectorList[1], vectorList[2]);
            return triunghiFisier;
        }
    }
}
