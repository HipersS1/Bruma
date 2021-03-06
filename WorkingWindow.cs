using System;
using System.Windows.Forms;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;
using System.Threading;
//Grupa 3131B Bruma Sebastian

//Laborator 2
//Apasarea tastei W afiseaza forma de tip cub, tastsa A roteste in stanga , D roteste in dreapta,
//cubul se rotestes sus/jos in functie de in ce parte a ecranului este situat mouseul

//Laborator 3
//Pentru schimbarea culorilor tineti apasat tasta R/G/B si sageata Sus/Jos
//Pentru miscarea camerei stanga-dreapta tineti apasat click-stanga si miscati mouseul stanga sau dreapta

namespace LaboratorEGC
{
    class WorkingWindow: GameWindow
    {
        private const int XYZ_SIZE = 75;
        private float rotation_speed = 90.0f, angle;

        bool moveLeft, moveRight, showCube;
        bool moveMouseUp, moveMouseDown, moveMouseLeft, moveMouseRight;

        float eyeX = 5, eyeY = 6, eyeZ = 20; //Variabile pentru Matrix.LookAt();
        int colorRed = 0, colorGreen = 0, colorBlue = 0;//Variabile ce contin informatii despre culorile RGB

        KeyboardState lastKeyPress;
        Vector3 v0, v1, v2;//Vectori pentru desenarea unui triunghi
        string fileName = @"D:\Facultate\EGC\L2\LaboratorEGC\Triunghi.txt";
        string fileNameCube = @"D:\Facultate\EGC\L2\LaboratorEGC\Cube.txt";

        private Color[] cubeColors;
        Randomizer randomColors;

        //Miscare obiect
        bool startAnimatieDown;
        bool startAnimatieUp;
        bool mouseClicked;
        //private ulong updatesCounter;
        private int maxHeight = 25;

        //Forme
        Cube cub, cub2;
        MassiveObject obj;
        Axes coordAxes;
        Triunghi trFis, tr, trFis2;
        //Constante
        private const int maxColor = 255;
        private const int minColor = 0;

        /// Camera 3d
        private readonly Camera3D cam;

        public WorkingWindow() : base(1280, 768, new GraphicsMode(32, 24, 0, 8))
        {
            VSync = VSyncMode.Adaptive;
            KeyDown += Keyboard_KeyDown;

            cam = new Camera3D();
            coordAxes = new Axes();

            obj = new MassiveObject(Color.Red);
            obj.Translate(new Vector3(10, 5, 0));

            trFis = Triunghi.ReadFileTriangle(fileName);
            v0 = new Vector3(5, 0, 0);
            v1 = new Vector3(10, 0, 0);
            v2 = new Vector3(10, 10, 0);

            tr = new Triunghi(v0, v1, v2);

            cub = new Cube(fileNameCube); // cub
            cub2 = new Cube(fileNameCube); // cub animatie
            startAnimatieUp = true;
            startAnimatieDown = false;

            cub.CubeTranslate(new Vector3(-14, 0, 0));
            cub2.CubeTranslate(new Vector3(0, maxHeight+6, 0));

            trFis2 = Triunghi.ReadFileTriangle(fileName);
            trFis2.Translate(new Vector3(10, 0, 0));

            ///initializare culori pentru cub
            randomColors = new Randomizer();
            cubeColors = new Color[6];
            for (int i = 0; i < 6; i++)
            {
                cubeColors[i] = randomColors.RandomColor();
            }

            TextMenu();
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
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.ClearColor(Color.Gray);

            // set viewport
            GL.Viewport(0, 0, this.Width, this.Height);

            // set perspective
            Matrix4 perspectiva = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)this.Width / (float)this.Height, 1, 1024);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspectiva);

            // set the eye
            cam.SetCamera();
        }
        
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            KeyboardState keyboardInput = OpenTK.Input.Keyboard.GetState();
            MouseState mouse = OpenTK.Input.Mouse.GetState();
            if (keyboardInput[OpenTK.Input.Key.H] && !keyboardInput.Equals(lastKeyPress))
            {
                TextMenu();// Afisare meniu
            }


            #region Laborator 5 Camera 3D
            // camera control (isometric mode)
            if (keyboardInput[Key.W])
            {
                cam.MoveForward();
            }
            if (keyboardInput[Key.S])
            {
                cam.MoveBackward();
            }
            if (keyboardInput[Key.A])
            {
                cam.MoveLeft();
            }
            if (keyboardInput[Key.D])
            {
                cam.MoveRight();
            }
            if (keyboardInput[Key.Q])
            {
                cam.MoveUp();
            }
            if (keyboardInput[Key.E])
            {
                cam.MoveDown();
            }
            //Set camera far
            if (keyboardInput[Key.F] && !keyboardInput.Equals(lastKeyPress))
            {
                cam.SetCameraFar();
            }
            if (keyboardInput[Key.C] && !keyboardInput.Equals(lastKeyPress))
            {
                cam.SetCameraClose();
            }

            #endregion
            if (keyboardInput[OpenTK.Input.Key.O] && !keyboardInput.Equals(lastKeyPress))
            {
                obj.ToggleVisibility();
            }


            #region Laborator 5 animatie cadere
            if (mouse[MouseButton.Left])
            {
                mouseClicked = true;
            }
            #endregion

            #region Laborator 2 rotirea formei din tasta
            moveLeft = false; moveRight = false;
            if(keyboardInput[OpenTK.Input.Key.ControlLeft])
            {
                if (keyboardInput[OpenTK.Input.Key.A])
                {
                    moveLeft = true;
                }
                else if (keyboardInput[OpenTK.Input.Key.D] )
                {
                    moveRight = true;
                }
                else if (keyboardInput[OpenTK.Input.Key.W] && !keyboardInput.Equals(lastKeyPress))
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
            }
            
            #endregion
            
            #region Schimbarea culorii vertexurilor prima modalitate LABORATOR 3
            //  RX
            //GZ   BC
            //moveUp = false;
            /*
            if (keyboardInput[OpenTK.Input.Key.Z])
            {
                if (CheckIfInRangeColor(colorGreen))
                {
                    colorGreen++;
                    if (CheckIfInRangeColor(colorBlue-1))
                        colorBlue--;
                    if (CheckIfInRangeColor(colorRed-1))
                        colorRed--;
                }
            }
            if (keyboardInput[OpenTK.Input.Key.X])
            {
                if (CheckIfInRangeColor(colorRed))
                {
                    colorRed++;
                    if (CheckIfInRangeColor(colorBlue-1))
                        colorBlue--;
                    if (CheckIfInRangeColor(colorGreen-1))
                        colorGreen--;
                }
            }
            if (keyboardInput[OpenTK.Input.Key.C])
            {
                if (CheckIfInRangeColor(colorBlue))
                {
                    colorBlue++;
                    if (CheckIfInRangeColor(colorRed-1))
                        colorRed--;
                    if (CheckIfInRangeColor(colorGreen-1))
                        colorGreen--;
                }
            }
            */
            #endregion

            #region Schimbarea culorii vertexurilor a doua modalitate Laborator 3 Punctul 8

            if (keyboardInput[OpenTK.Input.Key.G] )
            {
                if(keyboardInput[OpenTK.Input.Key.Up])
                {
                    if (CheckIfInRangeColor(colorGreen))
                    {
                        colorGreen++;
                        Console.WriteLine("R: " + colorRed + " G: " + colorGreen + " B: " + colorBlue);
                    }
                }
                else if (keyboardInput[OpenTK.Input.Key.Down])
                {
                    if (CheckIfInRangeColor(colorGreen-1))
                    {
                        colorGreen--;
                        Console.WriteLine("R: " + colorRed + " G: " + colorGreen + " B: " + colorBlue);
                    }
                }
            }

            if (keyboardInput[OpenTK.Input.Key.R])
            {
                if (keyboardInput[OpenTK.Input.Key.Up])
                {
                    if (CheckIfInRangeColor(colorRed))
                    {
                        colorRed++;
                        Console.WriteLine("R: " + colorRed + " G: " + colorGreen + " B: " + colorBlue);
                    }
                }
                else if (keyboardInput[OpenTK.Input.Key.Down])
                {
                    if (CheckIfInRangeColor(colorRed - 1))
                    {
                        colorRed--;
                        Console.WriteLine("R: " + colorRed + " G: " + colorGreen + " B: " + colorBlue);
                    }
                }
            }

            if (keyboardInput[OpenTK.Input.Key.B])
            {
                if (keyboardInput[OpenTK.Input.Key.Up])
                {
                    if (CheckIfInRangeColor(colorBlue))
                    {
                        colorBlue++;
                        Console.WriteLine("R: " + colorRed + " G: " + colorGreen + " B: " + colorBlue);
                    }
                }
                else if (keyboardInput[OpenTK.Input.Key.Down])
                {
                    if (CheckIfInRangeColor(colorBlue - 1))
                    {
                        colorBlue--;
                        Console.WriteLine("R: " + colorRed + " G: " + colorGreen + " B: " + colorBlue);
                    }
                }
            }

            #endregion

            #region Schimbarea culorii unei fete a cubului Laborator 4 Punctul 1,3
            if (keyboardInput[OpenTK.Input.Key.ControlLeft])
            {
                Randomizer faceColor = new Randomizer();
                if(keyboardInput[OpenTK.Input.Key.Number1] && !keyboardInput.Equals(lastKeyPress))
                {
                    cubeColors[0] = faceColor.RandomColor();
                }
                if (keyboardInput[OpenTK.Input.Key.Number2] && !keyboardInput.Equals(lastKeyPress))
                {
                    cubeColors[1] = faceColor.RandomColor();
                }
                if (keyboardInput[OpenTK.Input.Key.Number3] && !keyboardInput.Equals(lastKeyPress))
                {
                    cubeColors[2] = faceColor.RandomColor();
                }
                if (keyboardInput[OpenTK.Input.Key.Number4] && !keyboardInput.Equals(lastKeyPress))
                {
                    cubeColors[3] = faceColor.RandomColor();
                }
                if (keyboardInput[OpenTK.Input.Key.Number5] && !keyboardInput.Equals(lastKeyPress))
                {
                    cubeColors[4] = faceColor.RandomColor();
                }
                if (keyboardInput[OpenTK.Input.Key.Number6] && !keyboardInput.Equals(lastKeyPress))
                {
                    cubeColors[5] = faceColor.RandomColor();
                }
            }

            #endregion

            #region Move camera left-right
            if (OpenTK.Input.Mouse.GetState()[MouseButton.Left])
            {

                moveMouseUp = false; moveMouseDown = false; moveMouseLeft = false; moveMouseRight = false;
                //if (mouse.Y > 50 && eyeY < 20)
                //{
                //    moveMouseUp = true;
                //    Console.WriteLine("Sus" + mouse.Y);

                //    eyeY += 0.5f;
                //}
                //else if (mouse.Y < -50 && eyeY > -10)
                //{
                //    moveMouseDown = true;
                //    Console.WriteLine("Jos");
                //    eyeY -= 0.5f;
                //}
                if (mouse.X > 50 && eyeX < 30)
                {
                    moveMouseRight = true;
                    eyeX += 0.5f;
                }
                else if (mouse.X < -50 && eyeX > -30)
                {
                    eyeX -= 0.5f;
                    moveMouseLeft = true;
                }
            }
            #endregion Move Camera Left-Right

            #region Laborator 2 mouse
            /*Laborator 2 rotire in functie de mouse.
            moveUp = false; moveDown = false;
            if (mouse.Y  > 0)
            {
                moveUp = true;
            }
            else if (mouse.Y  < 0)
            {
                moveDown = true;
            }
            */
            #endregion

            lastKeyPress = keyboardInput;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            obj.Draw();
            //Matrix4 lookat = Matrix4.LookAt(eyeX, eyeY, eyeZ, 0, 0, 0, 0, 1, 0);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(ref lookat);

            coordAxes.setWidth(5);
            coordAxes.DrawAxes();
            Triunghi.DrawTriangle(trFis, Color.FromArgb(2, colorRed, colorGreen, colorBlue), Color.FromArgb(colorBlue, colorRed, colorGreen), Color.FromArgb(colorGreen, colorRed, colorBlue));
            Triunghi.DrawTriangle(tr);

            
            cub.DrawCube(cubeColors);
            if(mouseClicked)
                this.MoveCube(cub2);
            cub2.DrawCube(cubeColors);
            #region Laborator 2 afisare cub / rotire
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
                if (moveMouseUp == true)
                {
                    GL.Rotate(angle, 1.0f, 0.0f, 0.0f);
                }
                else if (moveMouseDown == true)
                {
                    GL.Rotate(angle, -1.0f, 0.0f, 0.0f);
                }
                //DrawCube(); 
                cub.DrawCube(cubeColors);
            }
            #endregion
            Triunghi.DrawTriangle(trFis2, colorRed, colorGreen, colorBlue);

            this.SwapBuffers();
            Thread.Sleep(1);
        }

        private void MoveCube(Cube c)
        {
            if (startAnimatieDown == true && c.getHeight() >= 0)
            {
                c.CubeTranslate(new Vector3(0, -0.2f, 0));
                //Console.WriteLine(c.getHeight());
                if (c.getHeight() == 0)
                {
                    startAnimatieUp = true;
                    startAnimatieDown = false;
                    mouseClicked = false;
                }
            }
            if (startAnimatieUp == true && c.getHeight() <= maxHeight)
            {
                c.CubeTranslate(new Vector3(0, +0.2f, 0));
                if (c.getHeight() == maxHeight)
                {
                    startAnimatieUp = false;
                    startAnimatieDown = true;
                    mouseClicked = false;
                }
            }
        }


        /// Laborator 3 desenarea axelor cu un singur begin
        private void DrawAxes()
        {
            GL.LineWidth(20.0f);

            // Desenează axa Ox (cu roșu).
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(XYZ_SIZE, 0, 0);
            
            // Desenează axa Oy (cu galben).
            GL.Color3(Color.Yellow);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, XYZ_SIZE, 0); ;
            
            // Desenează axa Oz (cu verde).
            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, XYZ_SIZE);
            GL.End();
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
        private void DrawForm3()
        {
            GL.Begin(PrimitiveType.Quads);

            GL.Color3(Color.Red);
            GL.Vertex3(-3f, 2f, 0);
            GL.Vertex3(2f, 2f, 0);
            GL.Color3(Color.Blue);
            GL.Vertex3(2f, 3f, 0);
            GL.Color3(Color.Yellow);
            GL.Vertex3(-2f, 3f, 0);

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


        public void TextMenu()
        {
            Console.Clear();
            Console.WriteLine("Modificare culoare triunghi:\n"
                            + "Tasta R, G, B + sageata sus/jos\n\n"
                            + "Tineti apasat controlLeft + un numar de  la 1-6 pentru schimbarea culorii fetei cubului\n"
                            + "F - far camera | C - close camera | Click cubul cade\n"
                            + "W A S D camera movement| Q E up down camera movement\n"
                            + "\n");
        }
        public bool CheckIfInRangeColor(int color)
        {
            if (color >= minColor && color < maxColor)
                return true;
            return false;
        }

    }
}

