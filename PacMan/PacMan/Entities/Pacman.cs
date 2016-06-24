using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReinforcementLearning.QLearning.FunctionApproximation;

namespace PacMan
{
    public class Pacman : CEntity
    {
        private double[] s;
        private int a;
        private QLearner qlearner;
        private double gamma = 0.1;
        public double rand_action;
        private int[] start_pos;
        private int episode = 0;

        public Pacman(Vector2 grid_pos, Map map, double rand_action = 0.0)
            : base(grid_pos * 32, new Vector2(32, 32), EntityType.Pacman, map)
        {
            this.rand_action = rand_action;
            this.start_pos = this.grid_pos;
            q_init();
        }

        public override void Draw(SpriteBatch sb, float scale = 1.0f)
        {
            sb.Draw(Contents.tex_pacman, vector_2_rec(pos, size, scale), Color.White);
        }

        public override void Update()
        {
            // Initialize s
            if (s == null) 
                s = new double[state_features.Count];

            int[] actions = get_dir_list();     // Possible actions to take
            double r = Reward();                // Reward for last move

            if (pacman_dead()) return;

            qlearner.Q(s, actions, r, true);    // Learn
            
            if (rand.NextDouble() < rand_action) a = actions[rand.Next(actions.Length)];
            else 
                a = qlearner.Choose_Action(actions);    // Choose next action
            
            s = Get_StateA_Features(a);             // s = s'

            info_text = "Pacman:\n";
            info_text += "r=" + r+"\n";
            info_text += "a=" + a + "\n";
            info_text += "alpha=" + qlearner.alpha + "\n";
            info_text += "episode=" + episode + "\n";
            for (int i = 0; i < qlearner.weights.Length; i++)
                info_text += "w"+i+"="+qlearner.weights[i]+"\n";

            for (int i = 0; i < 4; i++)
            {
                double[] f = Get_StateA_Features(i);
                info_text += "Action_"+i+":\n";
                for (int y = 0; y < f.Length; y++)
                    info_text += "f" + y + "=" + f[y]+"\n";
            }
            
                // Update
                Set_Grid_Pos(a_to_grid_pos(a));

                pacman_dead();
        }

        private bool pacman_dead()
        {
            // Grid reset
            if (pacman_on_ghost() || map.cheese_count==0)
            {
                if (map.cheese_count == 0)
                { }
                qlearner.Q(s, new int[0], Reward(), true);
                Set_Grid_Pos(start_pos);
                map.Grid_Reset();
                s = null;
                episode++;

                return true;
            }
            return false;
        }

        private double Reward()
        {
            double r = 0;
            EntityType grid_entity = map.grid[grid_pos[1],grid_pos[0]].entity_type;
            bool ghost = pacman_on_ghost();

            if (grid_entity == EntityType.Cheese) map.cheese_count--;
            map.Set_Grid(new Vector2(grid_pos[0],grid_pos[1]),EntityType.Empty);

            if (ghost) return -500;                                         // Ghost
            else if (map.cheese_count == 0) return 200;                     // All Cheese gone
            else if (grid_entity == EntityType.Cheese) return 0.1;          // Cheese
            else if (grid_entity == EntityType.Empty) return -0.1;          // Empty
            else return 0;
        }

        // Returns all possible dirs to go in
        private int[] get_dir_list(int[] grid_pos = null)
        {
            List<int> dirs = new List<int>();
            for (int i = 0; i < 4; i++)
                if (!Wall(i,grid_pos))
                    dirs.Add(i);
            return dirs.ToArray();
        }

        private bool pacman_on_ghost()
        {
            for (int i = 0; i < map.ghosts.Count; i++)
                if (map.ghosts[i].grid_pos[0] == grid_pos[0] &&
                    map.ghosts[i].grid_pos[1] == grid_pos[1])
                    return true;
            return false;
        }

        // Returns a gridpos depending on a
        private int[] a_to_grid_pos(int a, int[] _grid_pos = null)
        {
            int[] grid_pos = (_grid_pos == null ? this.grid_pos : (int[])_grid_pos.Clone());
            if (a == 0) grid_pos[0]++;
            else if (a == 1) grid_pos[1]++;
            else if (a == 2) grid_pos[0]--;
            else if (a == 3) grid_pos[1]--;
            return grid_pos;
        }

        #region Features

        private bool all_ghosts = false;

        private List<F> state_features;

        private void q_init()
        {
            state_features = new List<F>();

            // Features
            state_features.Add(f_tunnle);
            if (!all_ghosts)
                state_features.Add(f_ghost);
            state_features.Add(f_cheese);
            state_features.Add(f_cheese_2);

            if (all_ghosts)
                for (int i = 0; i < map.ghosts.Count; i++)
                    state_features.Add(null);

                    qlearner = new QLearner(4, state_features.Count, gamma, Get_StateA_Features);
        }

        // Returns the feature representations of the current state
        private double[] Get_StateA_Features(int action)
        {
            double[] features = new double[state_features.Count];

            int offset = 0;
            for (int i = 0; i < features.Length; i++)
                if (state_features[i] != null)
                    features[i] = state_features[i](action);
                else
                {
                    offset = i;
                    break;
                }

            if (all_ghosts)
            {
                for (int i = 0; i < map.ghosts.Count; i++)
                    features[i+offset] = f_all_ghosts(a,i);
            }


            return features;
        }

        private double f_tunnle(int a)
        {
            int[] grid_pos = a_to_grid_pos(a);          // Updated grid_pos
            int[] actions = get_dir_list(grid_pos);     // Action list from updated grid_pos

            if (actions.Length <=2)
                if (actions.Length==1 ||(actions[0] + 2) % 4 == actions[1]) 
                    return 0;
            return 0.1;
        }

        private double f_ghost(int a)
        {
            int[] grid_pos = a_to_grid_pos(a);          // Updated grid_pos
            double closest_ghost = path_finder(grid_pos, EntityType.Ghost);
            //int[] actions = get_dir_list(grid_pos);     // Action list from updated grid_pos

            //double n_ghost = double.MaxValue;
            //for (int i = 0; i < map.ghosts.Count; i++)
            //{
            //    double dist = Math.Abs(map.ghosts[i].grid_pos[0] - grid_pos[0]) + Math.Abs(map.ghosts[i].grid_pos[1] - grid_pos[1]);
            //    if (dist < n_ghost)
            //        n_ghost = dist;
            //}

            return 1 / (closest_ghost + 0.00001);
        }

        private double f_cheese(int a)
        {
            int[] grid_pos = a_to_grid_pos(a);          // Updated grid_pos
            double closest_cheese = path_finder(grid_pos, EntityType.Cheese);
            //int[] actions = get_dir_list(grid_pos);     // Action list from updated grid_pos

            //double n_cheese = double.MaxValue;
            //for (int y = 0; y < map.map_size.Y; y++)
            //    for (int x = 0; x < map.map_size.X; x++)
            //    {
            //        if (map.grid[y, x].entity_type != EntityType.Cheese) continue;

            //        double dist = Math.Abs(x - grid_pos[0]) + Math.Abs(y - grid_pos[1]);
            //        if (dist < n_cheese)
            //            n_cheese = dist;
            //    }

            return 1/(closest_cheese + 0.00001);
        }

        private double f_cheese_2(int a)
        {
            int[] grid_pos = a_to_grid_pos(a);          // Updated grid_pos
            double closest_cheese = path_finder(grid_pos, EntityType.Cheese);
            //int[] actions = get_dir_list(grid_pos);     // Action list from updated grid_pos

            //double n_cheese = double.MaxValue;
            //for (int y = 0; y < map.map_size.Y; y++)
            //    for (int x = 0; x < map.map_size.X; x++)
            //    {
            //        if (map.grid[y, x].entity_type != EntityType.Cheese) continue;

            //        double dist = Math.Abs(x - grid_pos[0]) + Math.Abs(y - grid_pos[1]);
            //        if (dist < n_cheese)
            //            n_cheese = dist;
            //    }

            return 1 / Math.Pow(closest_cheese + 0.00001, 2);
        }

        private double f_all_ghosts(int a, int i)
        {
            int[] grid_pos = a_to_grid_pos(a);          // Updated grid_pos
            double closest_ghost = path_finder(grid_pos, EntityType.Ghost, i);

            return 1 / (closest_ghost + 0.00001);
        }
        
        #endregion

        #region PathFinder
        int pf_fields;
        
        private double path_finder(int[] grid_pos, EntityType entity, int ghost_index=-1)
        {
            pf_fields = 0;
            List<int[]> visited = new List<int[]>();
            List<int[]> pos = new List<int[]>() { grid_pos };
            List<int[]> n_pos = new List<int[]>();
            double dist = 1;

            if (entity == EntityType.Ghost)
            {
                // Check for Ghosts on same grid_pos
                if (ghost_index != -1)
                {
                    if (map.ghosts[ghost_index].grid_pos[0] == grid_pos[0] && map.ghosts[ghost_index].grid_pos[1] == grid_pos[1])
                        return dist;
                }
                else
                    for (int j = 0; j < map.ghosts.Count; j++)
                        if (map.ghosts[j].grid_pos[0] == grid_pos[0] && map.ghosts[j].grid_pos[1] == grid_pos[1])
                            return dist;
            }
            else if (entity == EntityType.Cheese)
            {
                // Check for Cheese
                // Return if one is found for it must be the closest
                if (map.grid[grid_pos[1], grid_pos[0]].entity_type == EntityType.Cheese)
                    return dist;
            }

            while (pos.Count > 0)
            {
                // For all poses
                for (int i = 0; i < pos.Count; i++)
                {
                    // Get all possible actions 
                    int[] n_pos_a = get_dir_list(pos[i]);
                    // But only add the new grid poses if they arent already visited
                    for (int y = 0; y < n_pos_a.Length; y++)
                    {
                        pf_fields++;
                        int[] n_grid_pos = a_to_grid_pos(n_pos_a[y], pos[i]);
                        // Check for visited
                        if (visited.Where(f => f[0] == n_grid_pos[0] && f[1] == n_grid_pos[1]).ToList().Count() == 0)
                        {
                            n_pos.Add(n_grid_pos);
                            visited.Add(n_grid_pos);
                        }

                        if (entity == EntityType.Ghost)
                        {
                            // Check for Ghosts
                            // Return if one is found for it must be the closest
                            if (ghost_index != -1)
                            {
                                if (map.ghosts[ghost_index].grid_pos[0] == n_grid_pos[0] && map.ghosts[ghost_index].grid_pos[1] == n_grid_pos[1])
                                    return dist;
                            }
                            else
                                for (int j = 0; j < map.ghosts.Count; j++)
                                    if (map.ghosts[j].grid_pos[0] == n_grid_pos[0] && map.ghosts[j].grid_pos[1] == n_grid_pos[1])
                                        return dist;
                        }
                        else if (entity == EntityType.Cheese)
                        {
                            // Check for Cheese
                            // Return if one is found for it must be the closest
                            if (map.grid[n_grid_pos[1], n_grid_pos[0]].entity_type == EntityType.Cheese)
                                return dist;
                        }
                    }
                }

                pos.Clear();
                pos = clone_list(n_pos);
                n_pos.Clear();
                dist++;
            }
            return dist;
        }

        private List<T> clone_list<T>(List<T> list)
        {
            List<T> n_list = new List<T>();
            for (int i = 0; i < list.Count; i++)
                n_list.Add(list[i]);
            return n_list;
        }

        #endregion
    }
}
