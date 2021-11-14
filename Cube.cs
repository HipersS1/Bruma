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
    class Cube
    {
        private Vector3[] cube;
        
        public Cube ()
        {

        }

        public Cube(string fileName)
        {
            this.cube = CreateCubeFromFile(fileName);
        }

        public Cube (Vector3[] cube)
        {
            this.cube = cube;
        }

        public int getHeight() { return (int)cube[0].Y ; }

        public void DrawCube(Color[] colors)
        {
            int j = 0;
            for(int i = 0; i < 6; i++)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Color3(colors[i]);
                GL.Vertex3(cube[j++]);
                GL.Vertex3(cube[j++]);
                GL.Vertex3(cube[j++]);
                GL.Vertex3(cube[j++]);
            }
            GL.End();
        }

        public void CubeTranslate(Vector3 translateVector)
        {
            for(int i = 0; i < cube.Length; i++)
            {
                cube[i] += translateVector;
            }
        }

        private Vector3[] CreateCubeFromFile(string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName))
            {
                string[] lines = File.ReadAllLines(fileName);
                string[] infoCord;
                Vector3[] vectorList = new Vector3[24];//puncte cub
                int i = 0;
                foreach (string line in lines)
                {
                    infoCord = line.Split(' ');
                    vectorList[i] = new Vector3((float)Convert.ToDouble(infoCord[0]), (float)Convert.ToDouble(infoCord[1]), (float)Convert.ToDouble(infoCord[2]));
                    i++;
                }
                return vectorList;
            }
            return null;
        }

    }
}
