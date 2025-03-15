// clase para la implementacion del mapa del raycaster, es decir, los pre calculos antes de la visualizacion 
// del juego

using Figuras;
using Silk.NET.OpenGL;

namespace Mapas
{

    public class Mapa
    {


        public int[][] map = [[1,1,1,1,1,1,1,1],
                              [1,1,0,1,0,0,0,1],
                              [1,0,0,0,0,1,0,1],
                              [1,0,0,0,0,0,0,1],
                              [1,0,0,0,0,0,0,1],
                              [1,0,1,0,0,1,1,1],
                              [1,0,1,0,0,0,0,1],
                              [1,1,1,1,1,1,1,1]];

        public List<float[]> longitudes_rayos = new List<float[]>();

        public float fov = 75f;

        public List<float[]> map_loaded = new List<float[]>();

        public float desX = 0.0f;
        public float desY = 0.0f;

        public float anguloRotacion = 0;

        public float presicion; 

        public int distancia;

        public void Load(Muro muro)
        {
            float[] inicio = [-1f, 1f];
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    if (map[i][j] == 1)
                    {
                        float x = inicio[0] + muro.size * 2 * j + muro.size;
                        float y = inicio[1] + muro.size * 2 * -i - muro.size;
                        map_loaded.Add([x, y]);
                    }

                }
            }
        }


        public void CalcularColisiones() {
            double d = 0;
            longitudes_rayos.Clear();
            for (int i = 0; i < fov; i++)
            {
                float longitudRayo = 0;
                // proceso de alargamiento radial :b re avanzado era
                for (int j = 0; j < distancia; j++)
                {
                    float posicionX = longitudRayo * 0.1f * (float)Math.Cos(anguloRotacion + (float)((fov - 1) * 0.0174533) + (float)d) + desX;
                    float posicionY = longitudRayo * 0.1f * (float)Math.Sin(anguloRotacion + (float)((fov - 1) * 0.0174533) + (float)d) - desY;
                    //Console.WriteLine($"X: {posicionX} : Y: {-posicionY}");

                    var colision = map_loaded.Find(p => p[0] - 0.125f < posicionX && posicionX < p[0] + 0.125f && p[1] + 0.125f > -posicionY && -posicionY > p[1] - 0.125f);

                    if (colision != null)
                    {
                        //Console.WriteLine($"colision en {colision[0]}:{colision[1]}");

                        j = distancia;
                    }

                    longitudRayo = longitudRayo + presicion;
                }
                longitudes_rayos.Add([longitudRayo, anguloRotacion + (float)((fov - 1) * 0.0174533) + (float)d]);
                d += 0.0174533;
            }
        }


        public void DibujarCalculos(GL gL, Rayo rayo, uint program, int ubicacionDeMatrizTraslacion, Figura origin)
        {

            foreach (float[] l in longitudes_rayos.ToArray()) {
                rayo.size = l[0];
                rayo.Dibujar(gL, PrimitiveType.Lines, program, ubicacionDeMatrizTraslacion, l[1], 0, 0, origin);
            }
        }

    }
}