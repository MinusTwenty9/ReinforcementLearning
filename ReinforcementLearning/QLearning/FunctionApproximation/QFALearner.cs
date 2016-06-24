using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReinforcementLearning.QLearning.FunctionApproximation
{
    public class QFALearner
    {

        public int action_count;
        public double gamma;
        public double alpha = 0.01;
        public double[] weights;
        public int feature_count;
        public F[] f;
        Random rand = new Random();


        public QFALearner(int action_count,F[] f, double gamma)
        {
            this.action_count = action_count;
            this.gamma = gamma;
            this.f = f;
            this.feature_count = f.Length;

            weights = new double[feature_count];    // 0 initialization
            //for (int i = 0; i < weights.Length; i++)
            //    weights[i] = rand.NextDouble();
        }

        public int Choose_Action()
        {
            return max_a_index();
        }
        
        // To Train QN
        public double Q(double[] s, double r, bool learn)
        {
            double q = Qf(s);
            double q_prime = max_a();
            double d = ((r + gamma * q_prime) - q);

            if (learn)
            {
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] += alpha * d * s[i];
                }
                alpha *= 0.999;
            }
            return d;
        }

        // After infinit steps Qn(s,a) = Q(s,a)
        public double Qf(double[] state)
        {
            double q = 0;

            for (int i = 0; i < feature_count; i++)
                q += weights[i] * state[i];

            return q;
        }

        // Returns Q value of optimal action
        public double max_a()
        {
            double max_q = double.MinValue;

            for (int i = 0; i < action_count; i++)
            {
                double[] state = Get_S(i);
                double qn = Qf(state);

                if (qn > max_q) max_q = qn;
            }

            return max_q;
        }

        // Returns Optimal action index to take
        private int max_a_index()
        {
            double max_q = double.MinValue;
            int max_i = 0;

            for (int i = 0; i < action_count; i++)
            {
                double[] state = Get_S(i);
                double qn = Qf(state);

                if (qn > max_q)
                {
                    max_q = qn;
                    max_i = i;
                }
            }

            return max_i;
        }

        public double[] Get_S(int action)
        { 
            double[] s = new double[f.Length];
            for (int i = 0; i < f.Length; i++)
                s[i] = f[i](action);
            return s;
        }

    }
    //public delegate double F(int a);
}
