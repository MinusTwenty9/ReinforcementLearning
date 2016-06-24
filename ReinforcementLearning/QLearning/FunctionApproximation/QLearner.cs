using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReinforcementLearning.QLearning.FunctionApproximation
{
    public class QLearner
    {
        public double gamma;
        public double alpha = 0.8;
        public double[] weights;
        public int feature_count;
        public int action_count;
        public Get_StateA_Features get_statea_features;
        Random rand;

        public QLearner(int action_count, int feature_count, double gamma, Get_StateA_Features get_statea_features)
        {
            this.action_count = action_count;
            this.gamma = gamma;
            this.feature_count = feature_count;
            this.get_statea_features = get_statea_features;
            this.rand = new Random();

            weights = new double[feature_count];    // 0 initialization
            //for (int i = 0; i < weights.Length; i++)
            //    weights[i] = rand.NextDouble();
        }

        public int Choose_Action(int[] actions)
        {
            return max_a_index(actions);
        }
        
        // To Train QN
        public double Q(double[] s, int[] a_primes, double r, bool learn)
        {
            double q = Qf(s);
            double q_prime = (a_primes.Length == 0 ? 0 : max_a(a_primes));
            double d = ((r + gamma * q_prime) - q);

            if (learn)
            {
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] += alpha * d * s[i];
                }
                alpha *= 0.99;
            }
            return d;
        }

        // After infinit steps Qn(s,a) = Q(s,a)
        public double Qf(double[] features)
        {
            double q = 0;

            for (int i = 0; i < feature_count; i++)
                q += weights[i] * features[i];

            return q;
        }

        // Returns Q value of optimal action
        public double max_a(int[] actions)
        {
            double max_q = double.MinValue;

            for (int i = 0; i < actions.Length; i++)
            {
                double[] features = get_statea_features(actions[i]);
                double qn = Qf(features);

                if (qn > max_q) max_q = qn;
            }

            return max_q;
        }

        // Returns Optimal action index to take
        private int max_a_index(int[] actions)
        {
            double max_q = double.MinValue;
            int max_i = 0;

            for (int i = 0; i < actions.Length; i++)
            {
                double[] features = get_statea_features(actions[i]);
                double qn = Qf(features);

                if (qn > max_q)
                {
                    max_q = qn;
                    max_i = actions[i];
                }
            }

            return max_i;
        }
    }
    public delegate double F(int a);
    public delegate double[] Get_StateA_Features(int action);
}
