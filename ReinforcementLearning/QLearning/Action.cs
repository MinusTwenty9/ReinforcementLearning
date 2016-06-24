using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReinforcementLearning.QLearning
{
    public class Action
    {
        public double state_prime_id;
        public double Q;
        public long n_s;
        //public double alpha
        //{
        //    get { /*return 1.0 / (double)n_s;*/ return 0.5; }
        //}
        public double alpha = 0.5;

        public Action(double state_prime_id, double Q)
        {
            this.state_prime_id = state_prime_id;
            this.Q = Q;
            n_s = 1;
        }
    }
}
