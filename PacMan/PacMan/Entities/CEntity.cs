using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PacMan
{
    public abstract class CEntity : Entity
    {
        public int[] grid_pos
        {
            get { return new int[] { (int)(pos.X / size.X), (int)(pos.Y / size.Y) }; }
        }
        public Map map;
        public Random rand;
        public int dir;
        public string info_text;

        public CEntity(Vector2 pos, Vector2 size,EntityType entity_type, Map map)
            : base(pos, size, entity_type)
        {
            this.map = map;
            rand = new Random(Program.rand.Next(int.MaxValue));
            dir = 0;
            info_text = "-";
        }

        // 0 => Right; 1 => Down ...
        public bool Wall(int dir, int[] grid_pos = null)
        {
            if (grid_pos == null)
                grid_pos = this.grid_pos;

            //  dir + 4 so if dir -1 to -3 no err
            dir = (dir+4)% 4;

            if (dir == 0)
            {
                if (grid_pos[0] + 1 >= map.map_size.X) return true;
                else if (map.grid[grid_pos[1], grid_pos[0] + 1].entity_type == EntityType.Wall) return true;
                else return false;
            }
            else if (dir == 2)
            {
                if (grid_pos[0] - 1 < 0) return true;
                else if (map.grid[grid_pos[1], grid_pos[0] - 1].entity_type == EntityType.Wall) return true;
                else return false;
            }
            else if (dir == 1)
            {
                if (grid_pos[1] + 1 >= map.map_size.Y) return true;
                else if (map.grid[grid_pos[1] + 1, grid_pos[0]].entity_type == EntityType.Wall) return true;
                else return false;
            }
            else if (dir == 3)
            {
                if (grid_pos[1] - 1 < 0) return true;
                else if (map.grid[grid_pos[1] - 1, grid_pos[0]].entity_type == EntityType.Wall) return true;
                else return false;
            }
            return false;
        }
    }
}
