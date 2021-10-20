using System;
using System.Windows.Forms;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Threading;

namespace LaboratorEGC
{
    class WorkingWindow: GameWindow
    {
        private float rotation_speed = 90.0f;
        private float angle;
        bool moveLeft, moveRight, showCube, moveUp, moveDown;
        KeyboardState lastKeyPress;

        //MouseState current, previous;

        
        public WorkingWindow() : base(1280, 720)
        {
            
            VSync = VSyncMode.Adaptive;
            KeyDown += Keyboard_KeyDown;
        }

        [STAThread]
        static void Main(string[] args)
        {
            using (WorkingWindow laborator = new WorkingWindow())
            {
                laborator.Title = "Laborator EGC";
                laborator.Run(60.0, 0.0);
                
            }
        }

        void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Exit();

            if (e.Key == Key.F11)
                if (this.WindowState == WindowState.Fullscreen)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Fullscreen;
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color.CadetBlue);
        }


        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);

            double aspect_ratio = Width / (double)Height;

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);

        }
        
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            KeyboardState keyboardInput = OpenTK.Input.Keyboard.GetState();
            MouseState mouse = OpenTK.Input.Mouse.GetState();
            

            moveLeft = false; moveRight = false;
            if(keyboardInput[OpenTK.Input.Key.A])
            {
                moveLeft = true;
            }
            else if(keyboardInput[OpenTK.Input.Key.D])
            {
                moveRight = true;
            }
            else if (keyboardInput[OpenTK.Input.Key.W]  && !keyboardInput.Equals(lastKeyPress))
            {
                // Ascundere comandată, prin apăsarea unei taste - cu verificare de remanență! Timpul de reacțieuman << calculator.
                if (showCube == true)
                {
                    showCube = false;
                }
                else
                {
                    showCube = true;
                }
            }
            lastKeyPress = keyboardInput;

            moveUp = false; moveDown = false;
            //current = new MouseState();
            if (mouse.Y  > 0)
            {
                moveUp = true;
            }
            else if (mouse.Y  < 0)
            {
                moveDown = true;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 lookat = Matrix4.LookAt(0, 1, 10, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);
            //GL.Translate(new Vector3(-5, 0, 2));
            DrawForm2();

            if (showCube == true)
            {
                angle += rotation_speed * (float)e.Time;
                if (moveLeft == true)
                {
                    GL.Rotate(angle, 0.0f, 1.0f, 0.0f);
                }
                else if (moveRight == true)
                {
                    GL.Rotate(angle, 0.0f, -1.0f, 0.0f);
                }
                if (moveUp == true)
                {
                    GL.Rotate(angle, 1.0f, 0.0f, 0.0f);
                }
                else if (moveDown == true)
                {
                    GL.Rotate(angle, -1.0f, 0.0f, 0.0f);
                }
                DrawCube(); 
            }
            this.SwapBuffers();
            //Thread.Sleep(1);
        }


        private void DrawForm()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.Red);
            GL.Vertex2(-0.5f, 0.5f);
            GL.Vertex2(0.5f, 0.5f);
            GL.Color3(Color.Blue);
            GL.Vertex2(0.5f, 0f);
            GL.Color3(Color.Yellow);
            GL.Vertex2(-0.5f, 0f);

            GL.End();
        }
        private void DrawForm2()
        {
            //
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Begin(PrimitiveType.Quads);

            GL.Color3(Color.Red);
            GL.Vertex2(-3f, 2f);
            GL.Vertex2(2f, 2f);
            GL.Color3(Color.Blue);
            GL.Vertex2(2f, 3f);
            GL.Color3(Color.Yellow);
            GL.Vertex2(-2f, 3f);

            GL.End();

        }
        private void DrawCube() 
        {
            GL.Begin(PrimitiveType.Quads);

            GL.Color3(Color.Blue);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);

            GL.Color3(Color.Honeydew);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);

            GL.Color3(Color.Violet);

            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);

            GL.Color3(Color.IndianRed);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);

            GL.Color3(Color.Yellow);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);

            GL.Color3(Color.ForestGreen);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);

            GL.End();
        }
    }
}
