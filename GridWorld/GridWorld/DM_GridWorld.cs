using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReinforcementLearning.QLearning;

namespace GridWorld
{
    public  class DM_GridWorld : DecisionMaker
    {
        public DM_GridWorld(double rand, QFunction q_func, bool learn)
            : base(rand, q_func, learn)
        { }

        public override int Calculate_Action()
        {
            // Calculate Perfect Q to go to
            double max = double.MinValue;
            int index = 0;
            List<int> max_i = new List<int>();

            for (int i = 0; i < c_state.actions.Length; i++)
                if (c_state.actions[i].Q > max)
                {
                    max = c_state.actions[i].Q;
                    index = i;

                    max_i.Clear();
                    max_i.Add(i);
                }
                else if (c_state.actions[i].Q == max) max_i.Add(i);

            if (max_i.Count > 1) index = max_i[random.Next(max_i.Count)];

            // Consider randomness
            if (random.NextDouble() < rand)
            {
                //index += (int)((random.Next(2) - 0.5) * 2);
                //if (index < 0) index = c_state.actions.Length - 1;

                //return index % c_state.actions.Length;
                return random.Next(c_state.actions.Length);
            }
            return index;
        }

        public override void Take_Action(int action_i)
        {
            base.Take_Action(action_i);
        } 
    }
}
