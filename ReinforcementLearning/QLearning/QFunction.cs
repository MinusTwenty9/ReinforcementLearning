using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReinforcementLearning.QLearning
{
    public class QFunction
    {
        public double gamma;
        public double k;
        public List<State> states;

        public QFunction(double gamma, double k)
        {
            this.k = k;
            this.gamma = gamma;
            this.states = new List<State>();
        }

        public double Q(Action a, State s)
        {
            State s_prime = StateID_2_State(a.state_prime_id);
            if (s_prime == null)
                throw new ArgumentException("max a Q(s', a') does not exist!");

            double sample = s.r + gamma * /**/(s.state_id == 3 || s.state_id == 7 ? 0 : /**/s_prime.max_a);
            double Q = (1 - a.alpha) * a.Q + a.alpha * sample;

            a.alpha *= 0.99;
            return Q;
        }

        public void Add_State(State state)
        {
            this.states.Add(state);
        }

        public State StateID_2_State(double state_id)
        {
            List<State> id_states = states.Where(s => s.state_id == state_id).ToList();

            if (id_states.Count != 1)
                throw new ArgumentException("Start State does not exist.");

            return id_states[0];
        }

    }
}
