
// este es otro ejemplo usando openGl
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.Input;
using System.Numerics;
using Silk.NET.OpenAL;
using System.Runtime.CompilerServices;
using Silk.NET.Vulkan;
using Figuras;
using Mapas;
namespace EjemploSilk
{

    public class Program
    {



        public static IWindow ventana;
        public static IInputContext inputContext;

        public static GL gL;


        public static uint program;

        public static int ubicacionDeMatrizTraslacion;
        public static int ubicacionVectorColor;

        public static float desX = 0.0f;
        public static float desY = 0.0f;

        public static float anguloRotacion = 0;

        public static Figura figura = new Figura();
        public static Rayo rayo = new Rayo();

         public static Muro muro = new Muro();

        public static Mapa mapas = new Mapa();

        public static Barra barra = new Barra();

        public static void Main(string[] args)
        {
            // obtenemos unas opciones para configurar la ventana
            // antes lo habiamos configurado con with
            WindowOptions options = WindowOptions.Default;

            options.Title = "Ejemplo ahora con input";
            options.Size = new Vector2D<int>(1024, 800);
            options.FramesPerSecond = 120;
            // creamos una ventana con las opciones elegidas
            ventana = Window.Create(options);

            ventana.Load += OnLoad;
            ventana.Render += OnRender;

            ventana.Run();

            ventana.Dispose();
        }

        public unsafe static void OnLoad()
        {

            // creamos una instancia del API de OpenGl
            gL = ventana.CreateOpenGL();

            // seteamos un fondo con el que limpiar la pantalla
            gL.ClearColor(System.Drawing.Color.Chocolate);

            // para tener input del usuario, usamos un Input context
            // que es creado usando el metodo createInput de nuestra ventana
            inputContext = ventana.CreateInput();

            // detecto los diferentes mouse que tengo conectados
            // debe ser asi porque no se sabe cuantos podria tener
            foreach (IMouse mouse in inputContext.Mice)
            {
                // ahora para las acciones del mouse, debemos mapearle callbacks

                mouse.Click += (IMouse mouse_clickeado, MouseButton boton, System.Numerics.Vector2 vector) =>
                {
                    if (boton == MouseButton.Left)
                    {
                        Console.WriteLine("hice click izquierdo");
                    }
                    else if (boton == MouseButton.Right)
                    {
                        Console.WriteLine("hice click derecho");
                    }
                };
            }

            // y de esta forma puedo detectar ahora el teclado
            foreach (IKeyboard keyboard in inputContext.Keyboards)
            {
                keyboard.KeyChar += (IKeyboard teclado, char letra) =>
                {
                    if (letra == 'a') {
                        desX = desX - (float)0.05;
                    } else if (letra == 'd') {
                        desX = desX + (float)0.05;
                    } else if (letra == 'w') {
                        desY = desY + (float)0.05;
                    } else if (letra == 's') {
                        desY = desY - (float)0.05;
                    } else if (letra == 'r') {
                        
                        anguloRotacion = anguloRotacion + (float)0.03;
                        if (anguloRotacion > 6.28) {
                            anguloRotacion = 0;
                        }else if (anguloRotacion < -1) {
                            anguloRotacion = 6.28f;
                        }
                    } else if (letra == 'q') {
                        
                        anguloRotacion = anguloRotacion - (float)0.03;
                        if (anguloRotacion < -1) {
                            anguloRotacion = 6.28f;
                        }
                    } 
                };
            }

            figura.verticesArray = [ -0.5f , 0.5f, 0,
                                      0.5f, -0.5f, 0,
                                      0.5f,  0.5f, 0, 
                                     -0.5f,  -0.5f, 0,
                                     ];

            // PARA CADA VERTICE, definimos un color rgb

            figura.coloresArray = [  1.0f , 0.0f, 1.0f,1.0f,
                                     0.5f, 0.0f, 1.0f,1.0f,
                                    0.0f, 1.0f, 1.0f,1.0f,
                                    0.0f, 1.0f, 1.0f,1.0f ];

            figura.indicesArray =  [0, 1, 2, 0, 3, 1]; 

            // ahora necesitamos shaders, se programa usando GLSL

            // notar una cosa en el shader de vertex
            // si nosotros queremos pasar datos al fragmentShader, tiene que ser atravez del vertexCode
            // y que este se lo pase como valor interpolado
            // usando out vec4 vertexColor; esto se pasara al siguiente shader y asi
            const string vertexCode = @"
                #version 330 core
                layout (location = 0) in vec3 aPos;
                layout (location = 1) in vec4 aColor;

                uniform mat4 traslationMatrix;
                uniform vec4 colorVectorTransformation;

                out vec4 vertexColor;
                void main()
                {
                gl_Position = traslationMatrix * vec4(aPos, 1.0);
                vertexColor = aColor * colorVectorTransformation;
                }";

            const string fragmentCode = @"
            #version 330 core
            in vec4 vertexColor; 
            out vec4 FragColor;  // Color final del píxel, es asi porque ya es la etapa final antes de pintar la pantalla
            // no se pasa a otro shader, en esta encadenacion, un out no siempre significa lo mismo

            void main()
            {
                FragColor = vertexColor; // Pintamos el píxel con el color recibido
                }
            ";
            // creamos un shader para guardar el codigo para el vertexShader
            uint vertexShader = gL.CreateShader(ShaderType.VertexShader);
            gL.ShaderSource(vertexShader, vertexCode); // le asigno el codigo al vertexShader

            // una vez asignado el codigo al shader, hay que compilarlo
            gL.CompileShader(vertexShader);

            // podemos obtener informacion del shader, como el resultado de su compilacion con getShader
            gL.GetShader(vertexShader, ShaderParameterName.CompileStatus, out int VcompStatus);
            if (VcompStatus != (int)GLEnum.True)
                throw new Exception("Se fallo la compilacion del shader de vertices");

            uint fragmentShader = gL.CreateShader(ShaderType.FragmentShader);
            gL.ShaderSource(fragmentShader, fragmentCode);
            gL.CompileShader(fragmentShader);

            gL.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out int FcompStatus);
            if (FcompStatus != (int)GLEnum.True)
                throw new Exception("Se fallo la compilacion del shader de fragmentos");


            // finalmente, lo shaders se deben asignar a un programa desde donde se ejecutaran
            program = gL.CreateProgram();

            gL.AttachShader(program, vertexShader);
            gL.AttachShader(program, fragmentShader);

            // ahora, le decimos a OpenGl que ejecute este programa para los shaders con linkProgram

            gL.LinkProgram(program);

            gL.GetProgram(program, ProgramPropertyARB.LinkStatus, out int lStatus);
            if (lStatus != (int)GLEnum.True)
                throw new Exception("Program failed to link: " + gL.GetProgramInfoLog(program));

            // finalmente, para mas adelante, obtengo la ubicacion del valor uniform que cree
            // para guardar la matriz de desplazamiento, para poder acceder a ella mas adelannte
            // en el OnRender
            ubicacionDeMatrizTraslacion = gL.GetUniformLocation(program, "traslationMatrix");
            ubicacionVectorColor = gL.GetUniformLocation(program, "colorVectorTransformation");
            // una vez el programa esta linkeado, ya no hace falta que tengamos atacheados los shaders
            gL.DetachShader(program, vertexShader);
            gL.DetachShader(program, fragmentShader);
            gL.DeleteShader(vertexShader);
            gL.DeleteShader(fragmentShader);

            rayo.Load(gL);
            figura.Load(gL);
            muro.Load(gL);
            barra.Load(gL);

            mapas.Load(muro);
        }

        public static void OnUpdate()
        {
            // en el evento OnUpdate, el thread que lo ejecuta no estara considerando
            // OpenGL o si esta creado o no, asi que aqui no se recomienda 
            // trabajar con OpenGL
            
        }

        public static unsafe void OnRender(double deltaTime)
        {
            // aqui si trabajamos con OpenGl  
            gL.Clear(ClearBufferMask.ColorBufferBit);
            //figura.Dibujar(gL, PrimitiveType.Triangles, program, ubicacionDeMatrizTraslacion, anguloRotacion, desX, desY);
            

            //foreach (float[] pos in mapas.map_loaded) {
            //    muro.Dibujar(gL, PrimitiveType.Triangles, program, ubicacionDeMatrizTraslacion, 0, pos[0],  pos[1]);  
            //}
            mapas.desX = desX;
            mapas.desY = desY;
            mapas.anguloRotacion = anguloRotacion;
            mapas.presicion = 0.0125f;
            mapas.distancia = 1000;
            mapas.CalcularColisiones();

            //mapas.DibujarCalculos(gL, rayo, program, ubicacionDeMatrizTraslacion, figura);
            
            float x = 2f / mapas.longitudes_rayos.ToArray().Length;
            barra.xsize = x / 2;
            //Console.WriteLine(x);
            float acumPosX = -1f;
            foreach(float[] d in mapas.longitudes_rayos.ToArray()) {

                barra.size = 1f / d[0];
                barra.Dibujar(gL, PrimitiveType.Triangles, program, ubicacionDeMatrizTraslacion,ubicacionVectorColor, 0f, acumPosX, 0f);
                acumPosX += x;
               
            }

        }   

        // este evento, se da cuando se cambia el tamaño o resolucion de la pantalla
        // es util para cambiar los tamaños de las figuras y demas ante estos cambios
        public static void OnFrameBufferResize(Vector2D<int> newSize)
        {

        }
    }
}