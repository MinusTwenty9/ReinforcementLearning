using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuronDotNet.Core.Backpropagation;
using NeuronDotNet.Core;

namespace ReinforcementLearning.QLearning.FunctionApproximation
{
    public class QFALearnerNN
    {
        public BackpropagationNetwork network;
        public double nn_learning_rate = 0.1;
        private TrainingSet training_set;

        public int action_count;
        public double gamma;
        public double alpha = 0.5;
        public double range = 1;

        
        //public QFALearner(string nn_path, int action_count, double gamma)
        //{
        //    this.nn = new BackPropagationNetwork(nn_path);
        //    this.action_count = action_count;
        //    this.gamma = gamma;
        //}

        public QFALearnerNN(int[] feature_dim_lengths, int[] hidden_layers, int action_count, double gamma)
        { 
            this.action_count = action_count;
            this.gamma = gamma;

            int[] layer_sizes = new int[2+hidden_layers.Length];
            int input_length = 1;   // 1 for action
            int output_length = 1;  // 1 so it fits q-learning scheme

            for (int i = 0; i < feature_dim_lengths.Length; i++)
                input_length += feature_dim_lengths[i];

            for (int i = 0; i < hidden_layers.Length; i++)
                layer_sizes[i + 1] = hidden_layers[i];

            layer_sizes[0] = input_length;
            layer_sizes[layer_sizes.Length - 1] = output_length;

            create_nn(layer_sizes);
        }

        private void create_nn(int[] layer_sizes)
        {
            List<ActivationLayer> layer = new List<ActivationLayer>();
            layer.Add(new LinearLayer(layer_sizes[0]));

            for (int i = 1; i < layer_sizes.Length; i++)
            {
                layer.Add(new SigmoidLayer(layer_sizes[i]));
                new BackpropagationConnector(layer[i - 1], layer[i]);
            }

            network = new BackpropagationNetwork(layer[0], layer[layer.Count-1]);
            network.SetLearningRate(nn_learning_rate);

            training_set = new TrainingSet(layer_sizes[0],1);
        }

        public int Choose_Action(List<double[]> features)
        {
            return max_a_index(features);
        }
        
        // To Train QN
        public double Q(List<double[]> features_s, List<double[]> features_s_p, int action, double r, bool learn)
        {
            double[] s = features_action_2_state(features_s, action);
            double q = Qn(s);
            double qn = max_a(features_s_p);
            q += alpha * ((r + gamma * qn)-q);

            if (learn)
            {
                //training_set.Clear();
                training_set.Add(new TrainingSample(s, new double[]{scale_desired(q)}));
                //network.Learn(training_set,1);
                //if (network.initialize == true) network.initialize = false;
            }

            return q;
        }

        // After infinit steps Qn(s,a) = Q(s,a)
        public double Qn(double[] state)
        {
            return scale_output(network.Run(state)[0]); 
        }

        // Returns Q value of optimal action
        public double max_a(List<double[]> features)
        {
            double max_q = double.MinValue;

            for (int i = 0; i < action_count; i++)
            {
                double[] state = features_action_2_state(features, i);
                double qn = Qn(state);

                if (qn > max_q) max_q = qn;
            }

            return max_q;
        }

        // Returns Optimal action index to take
        private int max_a_index(List<double[]> features)
        {
            double max_q = double.MinValue;
            int max_i = 0;

            for (int i = 0; i < action_count; i++)
            {
                double[] state = features_action_2_state(features, i);
                double qn = Qn(state);

                if (qn > max_q)
                {
                    max_q = qn;
                    max_i = i;
                }
            }

            return max_i;
        }

        public double[] features_action_2_state(List<double[]> features, int action)
        {
            double[] state = new double[network.InputLayer.NeuronCount];
            int index = 1;
            state[0] = (double)(action+1.0) / (double)action_count; 

            for (int i = 0; i < features.Count; i++)
                for (int y = 0; y < features[i].Length; y++)
                {
                    state[index] = features[i][y];
                    index++;
                }

            return state;
        }

        private double scale_desired(double d)
        {
            return (d + range) / (range * 2);
        }
        private double scale_output(double o)
        {
            return o * range * 2 - range;
        }

        public void Train_Network()
        {
            network.Learn(training_set,5);
            training_set.Clear();
        }
    }
}
