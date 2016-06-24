using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacMan
{
    public class Ghost : CEntity
    {
        

        public Ghost(Vector2 grid_pos, Map map)
            : base(grid_pos * 32, new Vector2(32, 32), EntityType.Ghost, map)
        { }

        public override void Draw(SpriteBatch sb, float scale = 1.0f)
        {
            sb.Draw(Contents.tex_ghost, vector_2_rec(pos,size,scale),Color.White);
        }

        public override void  Update()
        {
            //Change dir
            if (Wall(dir) || !Wall(dir + 1) || !Wall(dir - 1))
            {
                int[] dirs = get_dir_list();
                if (dirs.Length == 0)
                    dir = (dir + 2) % 4;
                else
                    dir = dirs[rand.Next(dirs.Length)];
            }

            int[] grid_pos = this.grid_pos;
            if (dir == 0) grid_pos[0]++;
            else if (dir == 1) grid_pos[1]++;
            else if (dir == 2) grid_pos[0]--;
            else if (dir == 3) grid_pos[1]--;

            Set_Grid_Pos(grid_pos);
        }

        // Returns all possible dirs to go in
        private int[] get_dir_list()
        {
            List<int> dirs = new List<int>();
            for (int i = 0; i < 4; i++)
                if (!Wall(i) && (i+2)%4 != dir)
                    dirs.Add(i);
            return dirs.ToArray();
        }
    }
}
