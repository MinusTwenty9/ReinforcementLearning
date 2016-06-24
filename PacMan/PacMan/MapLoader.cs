using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using ZBitmap;

namespace PacMan
{
    public static class MapLoader
    {
        public static EntityType[,] Load_Map(ref Microsoft.Xna.Framework.Vector2 map_size)
        {
            if (!File.Exists("./map.txt")) return null;
            string map_path = File.ReadAllText("./map.txt");
            if (!File.Exists(map_path)) return null;

            ImageH img = new ImageH(map_path);
            img.Load_Parallel();

            map_size = new Microsoft.Xna.Framework.Vector2(img.bmp.Width, img.bmp.Height);
            EntityType[,] grid = new EntityType[img.bmp.Height, img.bmp.Width];

            for (int y = 0; y < map_size.Y; y++)
                for (int x = 0; x < map_size.X; x++)
                {
                    EntityType type = EntityType.Empty ;
                    int index = (int)((y * map_size.X)+x)*3;

                    Color c = img.bmp.GetPixel(x, y);//Color.FromArgb(img.p_bmp.rgb[index], img.p_bmp.rgb[index + 1], img.p_bmp.rgb[index+2]);

                    if (comp_color(c,Color.White)) type = EntityType.Cheese;
                    else if (comp_color(c, Color.Black)) type = EntityType.Wall;
                    else if (comp_color(c, Color.Blue)) type = EntityType.Ghost;
                    else if (comp_color(c, Color.Yellow)) type = EntityType.Pacman;
                    else if (comp_color(c, Color.Red)) type = EntityType.Ghost_Killer;
                    else type = EntityType.Empty;

                    grid[y, x] = type;
                }

            img.Dispose();
            return grid;
        }

        private static bool comp_color(Color c1, Color c2)
        {
            if (c1.R == c2.R && c1.G == c2.G && c1.B == c2.B) 
                return true;
            return false;
        }
    }
}
