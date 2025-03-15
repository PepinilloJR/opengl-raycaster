// intento para implementar una forma automatizada de crear una figura o modificarla
using Silk.NET.OpenGL;
using Silk.NET.Vulkan;

namespace Figuras
{

    public class Figura
    {
        public static uint vao;
        public static uint vboVertexPositions;
        public static uint vboVertexColors;

        public static uint ebo;

        public float[] verticesArray;
        public uint[] indicesArray;

        public float[] coloresArray;

        public float size = 0.05f;

        public float desX = 0.0f;
        public float desY = 0.0f;


        public float[] Position = { 0.0f, 0.0f};

        public unsafe void Load(GL gL)
        {
            vao = gL.GenVertexArray();

            // ahora, bindeamos este VAO a openGL usando bindVertexArray
            // cada uno de estos objetos tendra un metodo para su bindeo

            // IMPORTANTE: al bindear este objeto, lo que estamos diciendo es que 
            // toda configuracion de atributos de vertices se quedaran registrados 
            // en este VAO, openGL no se maneja por referencias, sino con estos bindeos
            gL.BindVertexArray(vao);
            vboVertexPositions = gL.GenBuffer();
            gL.BindBuffer(BufferTargetARB.ArrayBuffer, vboVertexPositions); // bindeamos el buffer vbo

            // ahora, para poder cargar los datos de la posicion de los vertices, tenemos 
            // que usar BufferData, donde se lo estariamos cargamos al VBO
            // necesitamos un puntero de memoria donde estan los datos de los vertices
            fixed (float* buf = verticesArray)
                //target: Especifica el tipo de buffer. Para un VBO, usamos: 
                // size: tamaño de bytes que ocupara el dato que queremos cargar al VBO
                // data: puntero señalando los datos a cargar en la GPU
                // usage: indica como se usaran los datos para que OpenGL optimize la memoria para ese uso especifico
                gL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(verticesArray.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);


            // finalmente, definimos un EBO, elements buffer object, o IBO (index buffer object)
            // guardara indices que señalaran como conectar cada buffer para generar las figuras geometricas
            // asi hacemos poligonos!!

            // esta considerara las posiciones de los vertices ubicadas en el VBO
            // asi q debemos tener un buffer de estos bindeado
            ebo = gL.GenBuffer();
            gL.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);

            fixed (uint* buf = indicesArray)
                gL.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indicesArray.Length * sizeof(uint)), buf, BufferUsageARB.StaticRead);

            const uint aPos = 0;
            gL.EnableVertexAttribArray(0); // habilitamos el atributo indicando cual atributo del
                                           // del shader de vertices es, en este caso aPos que es el atributo 0
                                           // 3 le indica la cantidad de componentes que tiene el atributo y que leera del VBO
                                           // luego el tipo de valor que son los componentes
                                           // decimos si los valores estan normalizados o no, en este caso no lo estan, se toman tal como estan
                                           // luego el tamaño de un vertice singular
            gL.VertexAttribPointer(aPos, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);
            // ahora, finalmente, tenemos que usar el VAO para configurar los shaders, es decir
            // pasar los parametros


            const uint aColor = 1; // El layout(location = 1) en el shader

            // Generar y bindear VBO de colores, ahora con esto configuraremos el parametro 1 del shader con el VAO
            vboVertexColors = gL.GenBuffer();
            gL.BindBuffer(BufferTargetARB.ArrayBuffer, vboVertexColors);

            // cargamos en el VBO el array de los colores
            fixed (float* buf = coloresArray)
                gL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(coloresArray.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);

            // Habilitar el atributo de color y configurarlo
            gL.EnableVertexAttribArray(aColor);
            // aqui, tenemos 4 componentes para un vertice
            gL.VertexAttribPointer(aColor, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), (void*)0);





            // AHORA DESBINDEAMOS TODOS LOS BUFFERS PARA NO MODIFICARLOS AL PEPE
            gL.BindVertexArray(0); // desbindear el VAO actual
            gL.BindBuffer(BufferTargetARB.ArrayBuffer, 0); // desbindear el VBO actual
            gL.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0); // desbindear el EBO actual

        }

        public unsafe void Dibujar(GL gL, PrimitiveType type, uint program, int ubicacionDeMatrizTraslacion,float anguloRotacion, float desX, float desY)
        {
            // ahora, vamos a tomar un VAO, el VAO que configuramos antes tiene guardado
            // todas las configuraciones para el programa de los shaders
            gL.BindVertexArray(vao);

            // usamos el programa de los shaders que hicimos, el vao usara los parametros que le dijimos
            gL.UseProgram(program);
            this.desX = desX;
            this.desY = desY;

            float coseno = MathF.Cos((float)anguloRotacion);
            float seno = MathF.Sin((float)anguloRotacion);

            // openGl toma la columna mayor para las transformaciones
            // osea, las transformaciones las aplicamos en la ultima fila

            float[] translationMatrix = {
                coseno * size, -seno * size, 0.0f, 0.0f,
                seno * size, coseno * size, 0f, 0.0f,
                0.0f, 0f, 1.0f, 0.0f,
                desX, desY, 0.0f, 1.0f
            };
            gL.UniformMatrix4(ubicacionDeMatrizTraslacion, false, translationMatrix);

            gL.DrawElements(type, (uint)indicesArray.Length, DrawElementsType.UnsignedInt, (void*)0);
        }
    }


    public class Rayo
    {
        public static uint vao;
        public static uint vboVertexPositions;
        public static uint vboVertexColors;

        public static uint ebo;

        public float[] verticesArray = [ 0.0f , 0f, 0,
                                         0.1f,  0f, 0,];
        public uint[] indicesArray = [0, 1];

        public float[] coloresArray = [1.0f , 1.0f, 1.0f,1.0f,
                                       1.5f, 1.0f, 1.0f,1.0f];

        public float size = 15f;

        public float desX = 0.0f;
        public float desY = 0.0f;

        public float[] Position = {0.0f, 0.0f};

        public unsafe void Load(GL gL)
        {
            vao = gL.GenVertexArray();

            gL.BindVertexArray(vao);
            vboVertexPositions = gL.GenBuffer();
            gL.BindBuffer(BufferTargetARB.ArrayBuffer, vboVertexPositions); 
            fixed (float* buf = verticesArray)     
            gL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(verticesArray.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);

            ebo = gL.GenBuffer();
            gL.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);

            fixed (uint* buf = indicesArray)
                gL.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indicesArray.Length * sizeof(uint)), buf, BufferUsageARB.StaticRead);

            const uint aPos = 0;
            gL.EnableVertexAttribArray(0); 
            gL.VertexAttribPointer(aPos, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);


            const uint aColor = 1; 
            vboVertexColors = gL.GenBuffer();
            gL.BindBuffer(BufferTargetARB.ArrayBuffer, vboVertexColors);

            
            fixed (float* buf = coloresArray)
                gL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(coloresArray.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);


            gL.EnableVertexAttribArray(aColor);

            gL.VertexAttribPointer(aColor, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), (void*)0);

            gL.BindVertexArray(0); 
            gL.BindBuffer(BufferTargetARB.ArrayBuffer, 0); 
            gL.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0); 

        }

        public unsafe void Dibujar(GL gL, PrimitiveType type, uint program, int ubicacionDeMatrizTraslacion,float anguloRotacion, float desX, float desY, Figura origin)
        {
            // ahora, vamos a tomar un VAO, el VAO que configuramos antes tiene guardado
            // todas las configuraciones para el programa de los shaders
            gL.BindVertexArray(vao);

            // usamos el programa de los shaders que hicimos, el vao usara los parametros que le dijimos
            gL.UseProgram(program);
            this.desX = desX;
            this.desY = desY;

            float coseno = MathF.Cos((float)anguloRotacion);
            float seno = MathF.Sin((float)anguloRotacion);

            // openGl toma la columna mayor para las transformaciones
            // osea, las transformaciones las aplicamos en la ultima fila

            float[] translationMatrix = {
                coseno * size, -seno * size, 0.0f, 0.0f,
                seno * size, coseno * size, 0f, 0.0f,
                0.0f, 0f, 1.0f, 0.0f,
                origin.desX, origin.desY, 0.0f, 1.0f
            };
            gL.UniformMatrix4(ubicacionDeMatrizTraslacion, false, translationMatrix);

            gL.DrawElements(type, (uint)indicesArray.Length, DrawElementsType.UnsignedInt, (void*)0);


        }
    }


     public class Muro
    {
        public static uint vao;
        public static uint vboVertexPositions;
        public static uint vboVertexColors;

        public static uint ebo;

        public float[] verticesArray = [ 1f , 1f, 0,
                                         1f,  -1f, 0,
                                         -1f,  -1f, 0,
                                         -1f,  1f, 0];
        public uint[] indicesArray = [0, 1, 3, 
                                      1, 2, 3 ];

        public float[] coloresArray = [1.0f , 0.0f, 0.0f,1.0f,
                                       1.0f , 0.0f, 0.0f,1.0f,
                                       1.0f , 0.0f, 0.0f,1.0f,
                                       1.0f , 0.0f, 0.0f,1.0f,];

        public float size = 0.125f;

        public float desX = 0.0f;
        public float desY = 0.0f;

        public float[] Position = [0.0f, 0.0f];

        public unsafe void Load(GL gL)
        {
            vao = gL.GenVertexArray();

            gL.BindVertexArray(vao);
            vboVertexPositions = gL.GenBuffer();
            gL.BindBuffer(BufferTargetARB.ArrayBuffer, vboVertexPositions); 
            fixed (float* buf = verticesArray)     
            gL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(verticesArray.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);

            ebo = gL.GenBuffer();
            gL.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);

            fixed (uint* buf = indicesArray)
                gL.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indicesArray.Length * sizeof(uint)), buf, BufferUsageARB.StaticRead);

            const uint aPos = 0;
            gL.EnableVertexAttribArray(0); 
            gL.VertexAttribPointer(aPos, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);


            const uint aColor = 1; 
            vboVertexColors = gL.GenBuffer();
            gL.BindBuffer(BufferTargetARB.ArrayBuffer, vboVertexColors);

            
            fixed (float* buf = coloresArray)
                gL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(coloresArray.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);


            gL.EnableVertexAttribArray(aColor);

            gL.VertexAttribPointer(aColor, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), (void*)0);

            gL.BindVertexArray(0); 
            gL.BindBuffer(BufferTargetARB.ArrayBuffer, 0); 
            gL.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0); 

        }

        public unsafe void Dibujar(GL gL, PrimitiveType type, uint program, int ubicacionDeMatrizTraslacion,float anguloRotacion, float desX, float desY)
        {
            // ahora, vamos a tomar un VAO, el VAO que configuramos antes tiene guardado
            // todas las configuraciones para el programa de los shaders
            gL.BindVertexArray(vao);

            // usamos el programa de los shaders que hicimos, el vao usara los parametros que le dijimos
            gL.UseProgram(program);
            this.desX = desX;
            this.desY = desY;

            float coseno = MathF.Cos((float)anguloRotacion);
            float seno = MathF.Sin((float)anguloRotacion);

            // openGl toma la columna mayor para las transformaciones
            // osea, las transformaciones las aplicamos en la ultima fila

            float[] translationMatrix = {
                coseno * size, -seno * size, 0.0f, 0.0f,
                seno * size, coseno * size, 0f, 0.0f,
                0.0f, 0f, 1.0f, 0.0f,
                desX, desY, 0.0f, 1.0f
            };
            gL.UniformMatrix4(ubicacionDeMatrizTraslacion, false, translationMatrix);

            gL.DrawElements(type, (uint)indicesArray.Length, DrawElementsType.UnsignedInt, (void*)0);
        }
    }

    public class Barra
    {
        public static uint vao;
        public static uint vboVertexPositions;
        public static uint vboVertexColors;

        public static uint ebo;

        public float[] verticesArray = [ 1f , 1f, 0,
                                         1f,  -1f, 0,
                                         -1f,  -1f, 0,
                                         -1f,  1f, 0];
        public uint[] indicesArray = [0, 1, 3, 
                                      1, 2, 3 ];

        public float[] coloresArray = [1.0f , 0.0f, 0.0f,1.0f,
                                       1.0f , 0.0f, 0.0f,1.0f,
                                       1.0f , 0.0f, 0.0f,1.0f,
                                       1.0f , 0.0f, 0.0f,1.0f,];

        public float size = 1f;
        public float xsize = 0.02f;

        public float desX = 0.0f;
        public float desY = 0.0f;

        public float[] Position = [0.0f, 0.0f];

        public unsafe void Load(GL gL)
        {
            vao = gL.GenVertexArray();

            gL.BindVertexArray(vao);
            vboVertexPositions = gL.GenBuffer();
            gL.BindBuffer(BufferTargetARB.ArrayBuffer, vboVertexPositions); 
            fixed (float* buf = verticesArray)     
            gL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(verticesArray.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);

            ebo = gL.GenBuffer();
            gL.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);

            fixed (uint* buf = indicesArray)
                gL.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indicesArray.Length * sizeof(uint)), buf, BufferUsageARB.StaticRead);

            const uint aPos = 0;
            gL.EnableVertexAttribArray(0); 
            gL.VertexAttribPointer(aPos, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);


            const uint aColor = 1; 
            vboVertexColors = gL.GenBuffer();
            gL.BindBuffer(BufferTargetARB.ArrayBuffer, vboVertexColors);

            
            fixed (float* buf = coloresArray)
                gL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(coloresArray.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);


            gL.EnableVertexAttribArray(aColor);

            gL.VertexAttribPointer(aColor, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), (void*)0);

            gL.BindVertexArray(0); 
            gL.BindBuffer(BufferTargetARB.ArrayBuffer, 0); 
            gL.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0); 

        }

        public unsafe void Dibujar(GL gL, PrimitiveType type, uint program, int ubicacionDeMatrizTraslacion,float anguloRotacion, float desX, float desY)
        {
            // ahora, vamos a tomar un VAO, el VAO que configuramos antes tiene guardado
            // todas las configuraciones para el programa de los shaders
            gL.BindVertexArray(vao);

            // usamos el programa de los shaders que hicimos, el vao usara los parametros que le dijimos
            gL.UseProgram(program);
            this.desX = desX;
            this.desY = desY;

            float coseno = MathF.Cos((float)anguloRotacion);
            float seno = MathF.Sin((float)anguloRotacion);

            // openGl toma la columna mayor para las transformaciones
            // osea, las transformaciones las aplicamos en la ultima fila

            float[] translationMatrix = {
                coseno * xsize, -seno * xsize, 0.0f, 0.0f,
                seno * size, coseno * size, 0f, 0.0f,
                0.0f, 0f, 1.0f, 0.0f,
                desX, desY, 0.0f, 1.0f
            };
            gL.UniformMatrix4(ubicacionDeMatrizTraslacion, false, translationMatrix);

            gL.DrawElements(type, (uint)indicesArray.Length, DrawElementsType.UnsignedInt, (void*)0);
        }
    }
}