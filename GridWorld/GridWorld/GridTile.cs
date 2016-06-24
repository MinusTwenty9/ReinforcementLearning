
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReinforcementLearning.QLearning;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GridWorld
{
    public class GridTile
    {
        public State state;
        public Vector2 pos = Vector2.Zero;
        public Vector2 size = new Vector2(128,128);
        public GridTileType gtt;
        private Vector2 world_size;

        private double k;

        public double TILE_Q = 0.0;
        public double TILE_R = 0;

        public GridTile(GridTileType gtt, Vector2 pos, Vector2 world_size, double k)
        {
            this.gtt = gtt;
            this.k = k;
            this.pos = pos;
            this.world_size = world_size;

            if (gtt == GridTileType.Tile) tile();
            else if (gtt == GridTileType.Fin) fin();
            else if (gtt == GridTileType.Death) death();
            else wall();
        }

        private void tile()
        {
            Action[] actions = new Action[4];

            Vector2[] prime_poses = new Vector2[] 
            {
                new Vector2(pos.X-1,pos.Y), new Vector2(pos.X,pos.Y-1),
                new Vector2(pos.X+1,pos.Y),new Vector2(pos.X,pos.Y+1)
            };

            for (int i = 0; i < prime_poses.Length; i++)
                if (prime_poses[i].X < 0 || prime_poses[i].X >= world_size.X ||
                    prime_poses[i].Y < 0 || prime_poses[i].Y >= world_size.Y)
                    actions[i] = new Action(pos_prime_2_state_id(pos), TILE_Q);
                else
                // Normal TIle (consider Wall)
                {
                    if (prime_poses[i].X == 1 && prime_poses[i].Y == 1) actions[i] = new Action(pos_prime_2_state_id(pos), TILE_Q);
                    else actions[i] = new Action(pos_prime_2_state_id(prime_poses[i]), TILE_Q);

                }
            state = new State(actions, 0.0,TILE_R, pos_prime_2_state_id(pos),k); 
        }

        private void fin()
        {
            state = new State(new Action[] { new Action(pos_prime_2_state_id(new Vector2(0, world_size.Y - 1)), TILE_Q) }, 0, 1.0, pos_prime_2_state_id(pos), k);
        }

        private void death()
        {
            state = new State(new Action[] { new Action(pos_prime_2_state_id(new Vector2(0, world_size.Y - 1)), TILE_Q) }, 0, -1.0, pos_prime_2_state_id(pos), k);
        }

        private void wall()
        {
            state = new State(new Action[0], 0, 0.0, pos_prime_2_state_id(pos), k);
        }

        private double pos_prime_2_state_id(Vector2 pos_prime)
        {
            return pos_prime.Y * world_size.X + pos_prime.X;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Game1.tex_grid_tile, new Rectangle((int)(pos.X * size.X), (int)(pos.Y * size.Y), (int)(size.X), (int)(size.Y)),Color.White);

            if (state.actions.Length == 0)
                return;

            // Left Label
            float q = (float)state.actions[0].Q;
            Color c = new Color(new Vector3((q< 0 ? q*(-1) : 0.0f),(q > 0 ? q : 0f),0));

            sb.Draw(Game1.tex_triangle, new Rectangle((int)(pos.X * size.X + size.Y*0.5f), (int)(pos.Y * size.Y + size.Y * 0.5f), 
                (int)size.X, (int)size.Y), new Rectangle(0,0,Game1.tex_triangle.Width, Game1.tex_triangle.Height),
                c, (float)(MathHelper.Pi*2*0.25),new Vector2(64,64),SpriteEffects.None,1);

            sb.DrawString(Game1.text, state.actions[0].Q.ToString("G2"), new Vector2(pos.X * size.X + 4, pos.Y * size.Y + size.Y * 0.5f), Color.White,0,Vector2.Zero,1,SpriteEffects.None,0);

            if (state.actions.Length <= 1) return;

            // Up Label
            q = (float)state.actions[1].Q;
            c = new Color(new Vector3((q< 0 ? q*(-1) : 0.0f),(q > 0 ? q : 0f),0));
            sb.Draw(Game1.tex_triangle, new Rectangle((int)(pos.X * size.X +size.Y * 0.5f), (int)(pos.Y * size.Y + size.Y * 0.5f),
                (int)size.X, (int)size.Y), new Rectangle(0, 0, Game1.tex_triangle.Width, Game1.tex_triangle.Height),
                c, (float)(MathHelper.Pi * 2 * 0.5), new Vector2(64,64), SpriteEffects.None, 1);

            string label = state.actions[1].Q.ToString("G2");
            Vector2 ls = Game1.text.MeasureString(label);
            sb.DrawString(Game1.text, label, new Vector2(pos.X * size.X + (size.X / 2 - ls.X / 2), pos.Y * size.Y + 4), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            // Right label
            q = (float)state.actions[2].Q;
            c = new Color(new Vector3((q < 0 ? q * (-1) : 0.0f), (q > 0 ? q : 0f), 0));
            sb.Draw(Game1.tex_triangle, new Rectangle((int)(pos.X * size.X + size.Y * 0.5f), (int)(pos.Y * size.Y + size.Y * 0.5f),
                (int)size.X, (int)size.Y), new Rectangle(0, 0, Game1.tex_triangle.Width, Game1.tex_triangle.Height),
                c, (float)(MathHelper.Pi * 2 * 0.25), new Vector2(64, 64), SpriteEffects.FlipVertically, 1);

            label = state.actions[2].Q.ToString("G2");
            ls = Game1.text.MeasureString(label);
            sb.DrawString(Game1.text, label, new Vector2(pos.X * size.X + (size.X - ls.X - 4), pos.Y * size.Y+ size.Y * 0.5f), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            // Down Label
            q = (float)state.actions[3].Q;
            c = new Color(new Vector3((q < 0 ? q * (-1) : 0.0f), (q > 0 ? q : 0f), 0));
            sb.Draw(Game1.tex_triangle, new Rectangle((int)(pos.X * size.X + size.Y * 0.5f), (int)(pos.Y * size.Y + size.Y * 0.5f),
                (int)size.X, (int)size.Y), new Rectangle(0, 0, Game1.tex_triangle.Width, Game1.tex_triangle.Height),
                c, (float)(MathHelper.Pi * 2 * 0.5), new Vector2(64, 64), SpriteEffects.FlipVertically, 1);

                        
            label = state.actions[3].Q.ToString("G2");
            ls = Game1.text.MeasureString(label);
            sb.DrawString(Game1.text, label, new Vector2(pos.X * size.X + (size.X / 2- ls.X / 2), pos.Y * size.Y + (size.Y - ls.Y - 4)), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
    }

    public enum GridTileType
    {
        Tile,
        Wall,
        Fin,
        Death
    }
}
