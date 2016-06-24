using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReinforcementLearning.QLearning
{
    public class State
    {
        public Action[] actions;
        public double V;
        public double r;
        public double k;
        public double state_id;
        public double max_a
        {
            get {
                double max = double.MinValue;
                int index = 0;

                for (int i = 0; i < actions.Length; i++)
                    if (actions[i].Q > max)
                    {
                        max = actions[i].Q + (k / actions[i].n_s);
                        index = i;
                    }

                actions[index].n_s++;
                return max;
            }
        }

        public State(Action[] state_actions, double V, double r, double state_id, double k)
        {
            this.actions = state_actions;
            this.V = V;
            this.r = r;
            this.k = k;
            this.state_id = state_id;
        }
    }
}
