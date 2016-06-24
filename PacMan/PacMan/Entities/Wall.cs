using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacMan
{
    public class Wall : Entity
    {
        public Wall(Vector2 grid_pos)
            : base(grid_pos*32, new Vector2(32, 32),EntityType.Wall)
        { }

        public override void  Update()
        {
        }

        public override void Draw(SpriteBatch sb, float scale)
        {
            sb.Draw(Contents.tex_wall, vector_2_rec(pos,size, scale),Color.White);
        }
    }
}
