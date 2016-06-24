using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReinforcementLearning.QLearning
{
    public abstract class DecisionMaker 
    {
        public double rand;
        public State c_state;
        public bool learn;

        public Random random;
        public QFunction q_func;

        public DecisionMaker(double rand, QFunction q_func, bool learn)
        {
            this.rand = rand;
            this.q_func = q_func;
            this.learn = learn;
            this.random = new Random();
        }

        public void Set_C_State(double start_state_id)
        {
            this.c_state = q_func.StateID_2_State(start_state_id);
        }

        // Returns the Aciton index of the current state
        public abstract int Calculate_Action();

        // [Base.Take_Action last]
        public virtual void Take_Action(int action_i)
        {
            if (action_i < 0 || action_i >= c_state.actions.Length)
                throw new ArgumentException("Action index to take does not exist.");

            Action a = c_state.actions[action_i];
            State s_prime = q_func.StateID_2_State(a.state_prime_id);

            if (learn)
            {
                    a.Q = q_func.Q(a, c_state);
            }

            c_state = s_prime;
        }
    }
}
