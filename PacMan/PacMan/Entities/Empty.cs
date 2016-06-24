using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacMan
{
    public class Empty: Entity
    {
        public Empty(Vector2 pos)
            : base(pos, new Vector2(32, 32),EntityType.Empty)
        { }

        public override void  Update()
        {
        }

        public override void Draw(SpriteBatch sb, float scale)
        {
        }
    }
}
