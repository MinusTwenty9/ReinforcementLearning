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
using ReinforcementLearning.QLearning.FunctionApproximation;

namespace PacMan
{
    public class GameMain : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Map map;
        int c_elp = 0;
        bool key_down = false;
        bool step_by_step = false;
        int speed = 100;

        public GameMain()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 896;
            graphics.PreferredBackBufferHeight = 512;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Contents.Load_Contents(Content);

            map = new Map();
            map.scale = 1f; 

            Vector2 map_size = Vector2.Zero;
            EntityType[,] grid = MapLoader.Load_Map(ref map_size);
            map.Create_Custome_Map(grid,map_size);

            //map.Create_Map(MapType.Border_Empty,new Vector2(28,16));
            //map.Set_Grid(new Vector2(12,12),EntityType.Ghost);
        }
        
        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();

            map.Update();

            if (!step_by_step)
            {
                c_elp += gameTime.ElapsedGameTime.Milliseconds;
                if (c_elp >= speed)
                {
                    c_elp = 0;
                    update_map();
                }
            }

            if (key_down == false && (ks.GetPressedKeys().Length > 0 && (ks.GetPressedKeys()[0]!=Keys.None || ks.GetPressedKeys().Length > 1)))
            {
                key_down = true;

                if (ks.IsKeyDown(Keys.R))
                    for (int i = 0; i < map.pacmans.Count; i++)
                        map.pacmans[i].rand_action = (map.pacmans[i].rand_action > 0 ? 0 : 0.8);
                if (ks.IsKeyDown(Keys.S))
                    step_by_step = ! step_by_step;
                if (ks.IsKeyDown(Keys.Enter) && step_by_step)
                {
                    update_map();
                }
                if (ks.IsKeyDown(Keys.Tab))
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        update_map();
                    }
                }
                if (ks.IsKeyDown(Keys.F1))
                    speed = (speed == 100 ? 250 : 100);
            }
            else if (key_down == true && (ks.GetPressedKeys().Length == 0 || (ks.GetPressedKeys().Length == 1 && ks.GetPressedKeys()[0] == Keys.None)))
                key_down = false;
            Keys[] keys = ks.GetPressedKeys();

            base.Update(gameTime);
        }

        private void update_map()
        {
            map.Pacman_Update();
            map.Ghost_Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            map.Draw(spriteBatch);

            string text = "";
            for (int i = 0; i < map.pacmans.Count; i++)
                text += map.pacmans[i].info_text;
                spriteBatch.DrawString(Contents.text, text, new Vector2(512, 16), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}
