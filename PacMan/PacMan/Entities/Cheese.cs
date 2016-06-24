using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacMan
{
    public class Cheese : Entity
    {
        public Cheese(Vector2 grid_pos)
            : base(grid_pos*32, new Vector2(32, 32),EntityType.Cheese)
        { }

        public override void  Update()
        {
        }

        public override void Draw(SpriteBatch sb, float scale)
        {
            sb.Draw(Contents.tex_cheese, vector_2_rec(pos,size, scale),Color.White);
        }
    }
}
