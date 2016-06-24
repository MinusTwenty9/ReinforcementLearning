using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ReinforcementLearning.QLearning;

namespace GridWorld
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Texture2D tex_pixel;
        public static Texture2D tex_grid_tile;
        public static Texture2D tex_triangle;
        public static SpriteFont text;

        QFunction q_func;
        DecisionMaker dm;
        GridTile[,] grid;
        Vector2 grid_size;
        Vector2 agent_pos;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            q_func = new QFunction(0.999,0);
            dm = new DM_GridWorld(0.999,q_func, true);

            grid_size = new Vector2(4,3);
            grid = new GridTile[(int)grid_size.Y,(int)grid_size.X];

            for (int y = 0; y < grid_size.Y; y++)
                for (int x = 0; x < grid_size.X; x++)
                {
                    if (y == 0 && x == grid_size.X - 1) grid[y, x] = new GridTile(GridTileType.Fin, new Vector2(x, y), grid_size,q_func.k); // Fin
                    else if (y == 1 && x == grid_size.X - 1) grid[y, x] = new GridTile(GridTileType.Death, new Vector2(x, y), grid_size, q_func.k); // Death
                    else if (y == 1 && x == 1) grid[y, x] = new GridTile(GridTileType.Wall, new Vector2(x, y), grid_size, q_func.k); // Wall
                    else grid[y, x] = new GridTile(GridTileType.Tile, new Vector2(x, y), grid_size, q_func.k); // Tile

                    q_func.Add_State(grid[y, x].state);
                }

            dm.Set_C_State(grid[2,0].state.state_id);
            agent_pos = new Vector2(0,2);

            //for (int i = 0; i < 100; i++)
            //{
            //    int a_i = dm.Calculate_Action();
            //    dm.Take_Action(a_i);
            //}
            //dm.learn = false;
            //dm.rand = 0.0;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            tex_pixel = Content.Load<Texture2D>("pixel");
            tex_grid_tile = Content.Load<Texture2D>("GridTileBorder");
            tex_triangle = Content.Load<Texture2D>("Triangle");
            text = Content.Load<SpriteFont>("text");
        }

        protected override void UnloadContent()
        {
        }

        bool enter_down = false;
        int c_elapsed = 0;

        protected override void Update(GameTime gameTime)
        {
            c_elapsed += gameTime.ElapsedGameTime.Milliseconds;
            if (c_elapsed >= 10)
            {
                c_elapsed = 0;
                int a_i = dm.Calculate_Action();
                dm.Take_Action(a_i);

                bool done = false;
                for (int y = 0; y < grid_size.Y && !done; y++)
                    for (int x = 0; x < grid_size.X && !done; x++)
                        if (grid[y, x].state.state_id == dm.c_state.state_id)
                        {
                            agent_pos = grid[y, x].pos;
                            done = true;
                        }
            }

            KeyboardState ks = Keyboard.GetState();

            if (enter_down == false && ks.GetPressedKeys().Length > 0)
            {
                enter_down = true;
                int a_i = dm.Calculate_Action();

                if (ks.IsKeyDown(Keys.Left)) a_i = 0;
                else if (ks.IsKeyDown(Keys.Up)) a_i = 1;
                else if (ks.IsKeyDown(Keys.Right)) a_i = 2;
                else if (ks.IsKeyDown(Keys.Down)) a_i = 3;
                else goto BASE_UPDATE;

                dm.Take_Action(a_i);

                bool done = false;
                for (int y = 0; y < grid_size.Y && !done; y++)
                    for (int x = 0; x < grid_size.X && !done; x++)
                        if (grid[y, x].state.state_id == dm.c_state.state_id)
                        {
                            agent_pos = grid[y, x].pos;
                            done = true;
                        }
            }
            else if (enter_down && ks.GetPressedKeys().Length == 0)
                enter_down = false;

            BASE_UPDATE:
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.BackToFront,BlendState.AlphaBlend);

            for (int y = 0; y < grid_size.Y; y++)
                for (int x = 0; x < grid_size.X; x++)
                    grid[y, x].Draw(spriteBatch);

            spriteBatch.Draw(tex_pixel, new Rectangle((int)agent_pos.X * 128 + 60,(int)agent_pos.Y * 128 + 60,8,8),Color.Red);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
