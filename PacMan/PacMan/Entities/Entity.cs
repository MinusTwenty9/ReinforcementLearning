using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacMan
{
    public abstract class Entity
    {
        public Vector2 pos;
        public Vector2 size;
        public bool empty;
        public EntityType entity_type;

        public Entity(Vector2 pos, Vector2 size,EntityType entity_type, bool empty = false)
        {
            this.pos = pos;
            this.size = size;
            this.empty = empty;
            this.entity_type = entity_type;
        }
        
        public abstract void Update();
        public abstract void Draw(SpriteBatch sb, float scale = 1.0f);

        public void Set_Grid_Pos(int[] grid_pos)
        {
            pos = new Vector2(grid_pos[0]*size.X, grid_pos[1] * size.Y);
        }

        public Rectangle vector_2_rec(Vector2 pos, Vector2 size, float scale = 1.0f)
        {
            return new Rectangle((int)(pos.X * scale), (int)(pos.Y * scale), (int)(size.X * scale), (int)(size.Y * scale));
        }
    }
}
