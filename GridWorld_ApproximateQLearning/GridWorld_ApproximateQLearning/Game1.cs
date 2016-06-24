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
using ReinforcementLearning.QLearning.Visualizer;

namespace GridWorld_ApproximateQLearning
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Texture2D tex_grid_tile;
        public static Texture2D tex_pixel;
        public static SpriteFont text;
        
        QFALearner qlearner;
        QFALearnerNN q_learner_nn;
        bool learn = true;
        //List<double[]> feature_s;
        double[] state;
        Random rand = new Random();
        double epsilon = 0.8;
        int episode = 0;

        Vector2 grid_tile_size = new Vector2(64,64);
        Vector2 agent_pos; 
        Vector2 grid_size;
        int[,] grid;

        F[] f;
        double difference = 0;

        //Graph graph;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            f = new F[]{
                ((int a) =>
                {
                    return (double)wall_a(a);
                }),
                ((int a) =>
                {
                    return 1.0-(double)wall_a(a);
                }),
                // X
                ((int a) =>
                {
                    Vector2 agent_pos = grid_a(a);
                    return Math.Abs((agent_pos.X- (grid_size.X/2)) / (grid_size.X/2));
                }),
                // Y
                ((int a) =>
                {
                    Vector2 agent_pos = grid_a(a);
                    return Math.Abs((agent_pos.Y- (grid_size.Y/2)) / (grid_size.Y/2));
                }),
                // X
                ((int a) =>
                {
                    Vector2 agent_pos = grid_a(a);
                    return 1-Math.Abs((agent_pos.X- (grid_size.X/2)) / (grid_size.X/2));
                }),
                // Y
                ((int a) =>
                {
                    Vector2 agent_pos = grid_a(a);
                    return 1-Math.Abs((agent_pos.Y- (grid_size.Y/2)) / (grid_size.Y/2));
                }), 
                ((int a) =>
                {
                    Vector2 agent_pos = grid_a(a);
                    return (agent_pos.X / grid_size.X) + 0.1;
                }),
                ((int a) =>
                {
                    Vector2 agent_pos = grid_a(a);
                    return (agent_pos.Y / grid_size.Y) + 0.1;
                }),
                ((int a) =>
                {

                    Vector2 agent_pos = grid_a(a);
                    return (1 - (agent_pos.X / grid_size.X)) + 0.1;
                }),
                ((int a) =>
                {
                    Vector2 agent_pos = grid_a(a);
                    return (1 - (agent_pos.Y / grid_size.Y)) + 0.1;
                })
            };

            #region
            /*
            f[0] = ((int a) =>
            {
                Vector2 agent_pos = grid_a(a);
                return (agent_pos.X / grid_size.X) + 0.1;
            });

            f[1] = ((int a) =>
            {
                Vector2 agent_pos = grid_a(a);
                return (agent_pos.Y / grid_size.Y) + 0.1;
            });
            f[2] = ((int a) =>
            {

                Vector2 agent_pos = grid_a(a);
                return (1 - (agent_pos.X / grid_size.X)) + 0.1;
            });
            f[3] = ((int a) =>
            {
                Vector2 agent_pos = grid_a(a);
                return (1 - (agent_pos.Y / grid_size.Y)) + 0.1;
            });
            f[4] = ((int a) =>
            {
                Vector2 agent_pos = grid_a(a);
                if (agent_pos.X > 0 && grid[(int)agent_pos.Y, (int)agent_pos.X - 1] == 2) return 1.0;
                //else if (agent_pos.X + 1 < grid_size.X && grid[(int)agent_pos.Y, (int)agent_pos.X + 1] == 2) return 1.0;
                else return 0.1;

            });
            f[5] = ((int a) =>
            {
                Vector2 agent_pos = grid_a(a);
                if (agent_pos.Y > 0 && grid[(int)agent_pos.Y - 1, (int)agent_pos.X] == 2) return 1.0;
                //else if (agent_pos.Y + 1 < grid_size.Y && grid[(int)agent_pos.Y+1, (int)agent_pos.X ] == 2) return 1.0;
                else return 0.1;

            });
            f[6] = ((int a) =>
            {
                Vector2 agent_pos = grid_a(a);
                if (agent_pos.X + 1 < grid_size.X && grid[(int)agent_pos.Y, (int)agent_pos.X + 1] == 2) return 1.0;
                else return 0.1;

            });
            f[7] = ((int a) =>
            {
                Vector2 agent_pos = grid_a(a);
                if (agent_pos.Y + 1 < grid_size.Y && grid[(int)agent_pos.Y + 1, (int)agent_pos.X] == 2) return 1.0;
                else return 0.1;

            });*/
            //f[8] = ((int a) =>
            //{
            //    Vector2 agent_pos = grid_a(a);
            //    if (agent_pos.X > 0 && grid[(int)agent_pos.Y, (int)agent_pos.X - 1] == 2) return 1.0;
            //    //else if (agent_pos.X + 1 < grid_size.X && grid[(int)agent_pos.Y, (int)agent_pos.X + 1] == 2) return 1.0;
            //    else return 0.1;

            //});
            //f[9] = ((int a) =>
            //{
            //    Vector2 agent_pos = grid_a(a);
            //    if (agent_pos.Y > 0 && grid[(int)agent_pos.Y - 1, (int)agent_pos.X] == 2) return 1.0;
            //    //else if (agent_pos.Y + 1 < grid_size.Y && grid[(int)agent_pos.Y+1, (int)agent_pos.X ] == 2) return 1.0;
            //    else return 0.1;

            //});
            //f[10] = ((int a) =>
            //{
            //    Vector2 agent_pos = grid_a(a);
            //    if (agent_pos.X + 1 < grid_size.X && grid[(int)agent_pos.Y, (int)agent_pos.X + 1] == 2) return 1.0;
            //    else return 0.1;

            //});
            //f[11] = ((int a) =>
            //{
            //    Vector2 agent_pos = grid_a(a);
            //    if (agent_pos.Y + 1 < grid_size.Y && grid[(int)agent_pos.Y + 1, (int)agent_pos.X] == 2) return 1.0;
            //    else return 0.1;

            //});
            //f[4] = ((int a) =>
            //{

            //    Vector2 agent_pos = grid_a(a);
            //    return (1 - (agent_pos.X / grid_size.X)) + 0.1;
            //});
            //f[5] = ((int a) =>
            //{
            //    Vector2 agent_pos = grid_a(a);
            //    return (1 - (agent_pos.Y / grid_size.Y)) + 0.1;
            //});

            #endregion

            #region
            /*
            //f[0] = ((int a) =>
            //{
            //    int ga = grid_a(a, 0);
            //    if (ga == 0) return 0.1;
            //    else return 0.25;
            //});
            //f[1] = ((int a) =>
            //{
            //    int ga = grid_a(a, 1);
            //    if (ga == 0) return 0.1;
            //    else return 0.25;
            //});
            //f[2] = ((int a) =>
            //{
            //    int ga = grid_a(a, 2);
            //    if (ga == 0) return 0.1;
            //    else return 0.25;
            //});
            //f[3] = ((int a) =>
            //{
            //    int ga = grid_a(a, 3);
            //    if (ga == 0) return 0.1;
            //    else return 0.25;
            //});

            //f[0] = ((int a) =>
            //{
            //    double x = (a == 0 || a == 2 ? (a == 0 ? agent_pos.X + 1 : agent_pos.X - 1) : agent_pos.X);
            //    double dist = 1 - ((grid_size.X - agent_pos.X + x) / grid_size.X);
            //    return dist;
            //});
            //f[1] = ((int a) =>
            //{
            //    double y = (a == 1 || a == 3 ? (a == 1 ? agent_pos.Y + 1 : agent_pos.Y - 1) : agent_pos.Y);
            //    double dist = 1 - ((grid_size.Y - agent_pos.Y) / grid_size.Y + y);
            //    return dist;
            //});
            //f[2] = ((int a) =>
            //{
            //    double x = (a == 0 || a == 2 ? (a == 0 ? agent_pos.X + 1 : agent_pos.X - 1) : agent_pos.X);
            //    double dist = 1 - ((agent_pos.X + x) / grid_size.X);
            //    return dist;
            //});
            //f[3] = ((int a) =>
            //{
            //    double y = (a == 1 || a == 3 ? (a == 1 ? agent_pos.Y + 1 : agent_pos.Y - 1) : agent_pos.Y);
            //    double dist = 1 - ((agent_pos.Y) / grid_size.Y + y);
            //    return dist;
            //});
            */
            #endregion


            qlearner = new QFALearner(4,f,0.999);
            //graph = new Graph();
            //graph.Show();

            //q_learner = new QFALearnerNN(new int[]{1,1,1,1}, new int[]{8,3},4, 0.999);
            Reset();

            //while (episode <= 100)
            //    UpdateQ();
            //learn = false;
            //epsilon = 0.0;
        }

        public void Reset()
        {
            agent_pos = new Vector2(0, 2);
            grid_size = new Vector2(5, 5);     
            // 0 = Free Field
            // 1 = Goal
            // 2 = Wall
            //-1 = Death

            grid = new int[,]       
            {
                { 0, 0, 0, 0, 0 },
                { 0, 2, 2, 2, 0 },     
                { 0, 2, 1, 0, 0 },    
                { 0, 2, 2, 2, 0 },    
                { 0, 0, 0, 0, 1 },     
            };                      

            //feature_s = get_s_feature();
            //if (episode % 1000 == 0)
            //    qlearner.Train_Network();
            episode++;

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            tex_grid_tile = Content.Load<Texture2D>("GridTileBorder");
            tex_pixel = Content.Load<Texture2D>("pixel");
            text = Content.Load<SpriteFont>("text");
        }

        protected override void UnloadContent()
        {
        }

        int c_elapsed = 0;
        bool key_down = false;
        bool keyboard_control = false;

        protected override void Update(GameTime gameTime)
        {
            if (keyboard_control == false)
            {
                c_elapsed += gameTime.ElapsedGameTime.Milliseconds;
                if (c_elapsed >= 50)
                {
                    c_elapsed = 0;

                    //UpdateQ
                    UpdateQ();
                }
            }

            KeyboardState ks = Keyboard.GetState();
            if (key_down == false && ks.GetPressedKeys().Length > 0)
            {
                key_down = true;

                if (ks.IsKeyDown(Keys.R))
                    epsilon = (epsilon == 0 ? 0.8 : 0);
                if (ks.IsKeyDown(Keys.L))
                    learn = !learn;
                if (ks.IsKeyDown(Keys.NumPad2))
                {
                    int c_ep = episode;
                    while (episode < c_ep + 20)
                        UpdateQ();
                }
                if (ks.IsKeyDown(Keys.NumPad9))
                {
                    int c_ep = episode;
                    while (episode < c_ep + 100)
                        UpdateQ();
                }
                if (ks.IsKeyDown(Keys.Right)) UpdateQ(true, 0);
                else if (ks.IsKeyDown(Keys.Down)) UpdateQ(true, 1);
                else if (ks.IsKeyDown(Keys.Left)) UpdateQ(true, 2);
                else if (ks.IsKeyDown(Keys.Up)) UpdateQ(true, 3);
                if (ks.IsKeyDown(Keys.K)) keyboard_control = !keyboard_control;
            }
            else if (key_down == true && ks.GetPressedKeys().Length == 0)
                key_down = false;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            for (int y = 0; y < grid_size.Y; y++)
                for (int x = 0; x < grid_size.X; x++)
                {
                    Color c = Color.White;
                    if (grid[y, x] == 2) c = new Color(Vector4.Zero);
                    else if (grid[y, x] == -1) c = Color.Red;
                    else if (grid[y, x] == 1) c = Color.Green;

                    spriteBatch.Draw(tex_grid_tile, new Rectangle((int)(x * (int)grid_tile_size.X) + 16, (int)(y * (int)grid_tile_size.Y) + 16, (int)grid_tile_size.X, (int)grid_tile_size.Y), c);
                }

            spriteBatch.Draw(tex_pixel, new Rectangle(((int)agent_pos.X * (int)grid_tile_size.X) - 8 + 16 + (int)grid_tile_size.X/2 , ((int)agent_pos.Y * (int)grid_tile_size.Y) - 8 + 16 + (int)grid_tile_size.Y/2, 16, 16), Color.Red);

            spriteBatch.Draw(tex_pixel, new Rectangle(((int)agent_pos.X * (int)grid_tile_size.X) - 8 + 16 + (int)grid_tile_size.X / 2 + 16, ((int)agent_pos.Y * (int)grid_tile_size.Y) - 8 + 16 + (int)grid_tile_size.Y / 2, 16, 16), q_2_color(qlearner.Qf(qlearner.Get_S(0))));
            spriteBatch.Draw(tex_pixel, new Rectangle(((int)agent_pos.X * (int)grid_tile_size.X) - 8 + 16 + (int)grid_tile_size.X / 2, ((int)agent_pos.Y * (int)grid_tile_size.Y) - 8 + 16 + (int)grid_tile_size.Y / 2 + 16, 16, 16), q_2_color(qlearner.Qf(qlearner.Get_S(1))));
            spriteBatch.Draw(tex_pixel, new Rectangle(((int)agent_pos.X * (int)grid_tile_size.X) - 8 + 16 + (int)grid_tile_size.X / 2 - 16, ((int)agent_pos.Y * (int)grid_tile_size.Y) - 8 + 16 + (int)grid_tile_size.Y / 2, 16, 16), q_2_color(qlearner.Qf(qlearner.Get_S(2))));
            spriteBatch.Draw(tex_pixel, new Rectangle(((int)agent_pos.X * (int)grid_tile_size.X) - 8 + 16 + (int)grid_tile_size.X / 2, ((int)agent_pos.Y * (int)grid_tile_size.Y) - 8 + 16 + (int)grid_tile_size.Y / 2 - 16, 16, 16), q_2_color(qlearner.Qf(qlearner.Get_S(3))));
            
            spriteBatch.DrawString(text, "episode="+episode, new Vector2(4,4),Color.White);
            spriteBatch.DrawString(text, "difference=" + difference, new Vector2(4, grid_size.Y * grid_tile_size.Y + 20), Color.White);

            for (int i = 0; i < qlearner.f.Length; i++)
            {
                Color c = q_2_color(qlearner.weights[i]);
                spriteBatch.Draw(tex_pixel, new Rectangle(graphics.PreferredBackBufferWidth - 66, 20 + (28 * i), 64, 24), c);
                spriteBatch.DrawString(text, qlearner.weights[i].ToString("G2"), new Vector2(graphics.PreferredBackBufferWidth-66, 20 + (28*i)),negative(c));
            }

                spriteBatch.End();
            base.Draw(gameTime);
        }

        private List<double[]> get_s_feature()
        {
            //double h_block = 0;
            //double v_block = 0;
            //if (agent_pos.X - 1 < 0 || agent_pos.X + 1 >= grid_size.X) h_block = 1;
            //else if (grid[(int)agent_pos.Y, (int)agent_pos.X + 1] == 2 || grid[(int)agent_pos.Y, (int)agent_pos.X - 1] == 2) h_block = 1;

            //if (agent_pos.Y - 1 < 0 || agent_pos.Y + 1 >= grid_size.Y) v_block = 1;
            //else if (grid[(int)agent_pos.Y + 1, (int)agent_pos.X] == 2 || grid[(int)agent_pos.Y - 1, (int)agent_pos.X] == 2) v_block = 1;

            //return  new List<double[]>() 
            //{ 
            //    new double[] { h_block }, 
            //    new double[] { v_block },
            //    new double[] { (grid_size.X - agent_pos.X)/grid_size.X},
            //    new double[] { (grid_size.Y - agent_pos.Y)/grid_size.Y}
            //};
            return new List<double[]>() { 
                new double[]{agent_pos.X/grid_size.X},
                new double[]{agent_pos.Y/grid_size.Y}
            };
        }

        // Returns reward
        private double do_action(int action)
        {
            Vector2 n_pos = Vector2.Zero;
            double reward = -1;

            // Calculate new possible position
            if (action == 0) n_pos = new Vector2(agent_pos.X + 1, agent_pos.Y);         // Right
            else if (action == 1) n_pos = new Vector2(agent_pos.X, agent_pos.Y + 1);    // Down
            else if (action == 2) n_pos = new Vector2(agent_pos.X - 1, agent_pos.Y);    // Left
            else if (action == 3) n_pos = new Vector2(agent_pos.X, agent_pos.Y - 1);    // Up

            int x = (int)n_pos.X;
            int y = (int)n_pos.Y;

            // Out of bounds
            if (x < 0 || x >= grid_size.X || y < 0 || y >= grid_size.Y) n_pos = agent_pos;
            // Wall
            else if (grid[y, x] == 2) n_pos = agent_pos;
                        
            x = (int)n_pos.X;
            y = (int)n_pos.Y;

            agent_pos = n_pos;

            // Calculate reward 
            if (grid[y, x] == 0) reward = -0.1;
            else if (grid[y, x] == -1) reward = -1.0;
            else if (grid[y, x] == 1) reward =30;


            return reward;
        }

        private void UpdateQ(bool keyboard = false, int ak = 0)
        {
            int a;

            if (!keyboard)
            {
                if (rand.NextDouble() < epsilon) a = rand.Next(4);
                else a = qlearner.Choose_Action();//a = q_learner_nn.Choose_Action(get_s_feature());
            }
            else a = ak;

            state = qlearner.Get_S(a);
            double r = do_action(a);
            //List<double[]> feature_s_p = get_s_feature();
            //q_learner_nn.Q(feature_s, feature_s_p, a, r, learn);
            difference = qlearner.Q(state,r,learn);

            //graph.Add_Data(0,qlearner.weights[0]);
            //feature_s = feature_s_p;

            // Check for end of episode
            int grid_val = grid[(int)agent_pos.Y, (int)agent_pos.X];
            if (grid_val == 1) Reset();
            else if (grid_val == -1) Reset();
        }

        private Vector2 grid_a(int a)
        {
            Vector2 agent_pos = this.agent_pos;

            if (a == 0)
            {
                if (agent_pos.X == grid_size.X - 1) ;
                else if (grid[(int)agent_pos.Y, (int)agent_pos.X + 1] == 2) ;
                else agent_pos.X +=1;
            }
            else if (a == 2)
            {
                if (agent_pos.X == 0) ;
                else if (grid[(int)agent_pos.Y, (int)agent_pos.X - 1] == 2) ;
                else agent_pos.X-=1;
            }
            else if (a == 1)
            {
                if (agent_pos.Y == grid_size.Y - 1) ;
                else if (grid[(int)agent_pos.Y + 1, (int)agent_pos.X] == 2) ;
                else agent_pos.Y += 1;
            }
            else if (a == 3)
            {
                if (agent_pos.Y == 0) ;
                else if (grid[(int)agent_pos.Y - 1, (int)agent_pos.X] == 2) ;
                else agent_pos.Y -= 1;
            }
            return agent_pos;
        }

        private int wall_a(int a)
        {
            if (a == 0)
            {
                if (agent_pos.X == grid_size.X - 1) return 1;
                else if (grid[(int)agent_pos.Y, (int)agent_pos.X + 1] == 2) return 1;
                else return 0;
            }
            else if (a == 2)
            {
                if (agent_pos.X == 0) return 1;
                else if (grid[(int)agent_pos.Y, (int)agent_pos.X - 1] == 2) return 1;
                else return 0;
            }
            else if (a == 1)
            {
                if (agent_pos.Y == grid_size.Y - 1) return 1;
                else if (grid[(int)agent_pos.Y + 1, (int)agent_pos.X] == 2) return 1;
                else return 0;
            }
            else if (a == 3)
            {
                if (agent_pos.Y == 0) return 1;
                else if (grid[(int)agent_pos.Y - 1, (int)agent_pos.X] == 2) return 1;
                else return 0;
            }
            return 0;
        }

        private Color q_2_color(double q)
        {
            Vector3 cv = new Vector3(0, 0, 0);
            float qf = ((float)q +1)/2;

            if (q < 0) cv.X = qf;
            else if (q > 0) cv.Y = qf;
            return new Color(cv);
        }

        private Color negative(Color c)
        {
            return new Color(1.0f - c.R, 1.0f - c.G, 1.0f - c.B);
        }
    }
}
