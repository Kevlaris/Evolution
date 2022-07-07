using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

[Serializable]
public class NeuralNetwork
{
	public int[] layer; //number of neurons in layers
	Layer[] layers; //layers
	float[][] neurons; //values of neurons in layers - 2D matrix
	float[][][] weights; //weights leading to neurons in layers - 3D matrix
	[SerializeField] public float fitness; //fitness of the network

	private BinaryFormatter formatter;
	private System.Random rnd = new System.Random((int)(Time.frameCount / Time.realtimeSinceStartup * DateTime.Now.Second));

	#region Constructors

	public NeuralNetwork(int[] layer)
	{
		this.layer = new int[layer.Length];
		for (int i = 0; i < layer.Length; i++)
		{
			this.layer[i] = layer[i];
		}

		//generate matrixes
		InitializeNeurons();
		InitializeWeights();

		layers = new Layer[this.layer.Length - 1];
		for (int i = 0; i < layers.Length; i++)
		{
			layers[i] = new Layer(layer[i], layer[i + 1]);
		}
		formatter = new BinaryFormatter();
	}

	/// <summary>
	/// Creates a network based on a <see cref="NetworkPattern">Network Pattern</see>
	/// </summary>
	public NeuralNetwork(NetworkPattern pattern)
	{
		this.layer = new int[pattern.numberOfNeuronsInLayers.Length];
		for (int i = 0; i < layer.Length; i++)
		{
			this.layer[i] = pattern.numberOfNeuronsInLayers[i];
		}

		//generate matrixes
		InitializeNeurons();
		InitializeWeights();

		layers = new Layer[this.layer.Length - 1];
		for (int i = 0; i < layers.Length; i++)
		{
			layers[i] = new Layer(layer[i], layer[i + 1]);
		}
		formatter = new BinaryFormatter();
	}

	/// <summary>
	/// Deep copy constructor
	/// </summary>
	/// <param name="copyNetwork">Network to deep copy</param>
	public NeuralNetwork(NeuralNetwork copyNetwork)
	{
		this.layer = new int[copyNetwork.layer.Length];
		for (int i = 0; i < copyNetwork.layer.Length; i++)
		{
			this.layer[i] = copyNetwork.layer[i];
		}

		InitializeNeurons();
		InitializeWeights();
		CopyWeights(copyNetwork.weights);

		layers = new Layer[this.layer.Length - 1];
		for (int i = 0; i < layers.Length; i++)
		{
			layers[i] = new Layer(layer[i], layer[i + 1]);
		}
		formatter = new BinaryFormatter();
	}

	#endregion


	public class Layer
	{
		int numberOfInputs;
		int numberOfOutputs;

		public float[] outputs;
		public float[] inputs;
		public float[,] weights;
		public float[,] weightsDelta;
		public float[] gamma;
		public float[] error;

		float learningRate = 0.05f;

		public Layer(int numberOfInputs, int numberOfOutputs)
		{
			this.numberOfInputs = numberOfInputs;
			this.numberOfOutputs = numberOfOutputs;

			outputs = new float[numberOfOutputs];
			inputs = new float[numberOfInputs];
			weights = new float[numberOfOutputs, numberOfInputs];
			weightsDelta = new float[numberOfOutputs, numberOfInputs];
			gamma = new float[numberOfOutputs];
			error = new float[numberOfOutputs];

			InitializeWeights();
		}

		public void InitializeWeights()
		{
			for (int i = 0; i < numberOfOutputs; i++)
			{
				for (int j = 0; j < numberOfInputs; j++)
				{
					weights[i, j] = (float)UnityEngine.Random.Range(-0.5f, 0.5f);
				}
			}
		}

		public float[] FeedForward(float[] inputs)
		{
			this.inputs = inputs;

			for (int i = 0; i < numberOfOutputs; i++)
			{
				outputs[i] = 0;

				for (int j = 0; j < inputs.Length; j++)
					outputs[i] += inputs[j] * weights[i, j];

				outputs[i] = (float)Math.Tanh(outputs[i]);
			}

			return outputs;
		}

		/// <summary>
		/// Returns the tanh derivative of given value
		/// </summary>
		public float TanHDer(float value)
		{
			return 1 - (value * value);
		}

		public void BackPropOutput(float[] expected)
		{
			for (int i = 0; i < numberOfOutputs; i++)
				error[i] = outputs[i] - expected[i];

			for (int i = 0; i < numberOfOutputs; i++)
				gamma[i] = error[i] * TanHDer(outputs[i]);

			for (int i = 0; i < numberOfOutputs; i++)
			{
				for (int j = 0; j < numberOfInputs; j++)
				{
					weightsDelta[i, j] = gamma[i] * inputs[j];
				}
			}
		}

		public void BackPropHidden(float[] gammaForward, float[,] weightsForward)
		{
			for (int i = 0; i < numberOfOutputs; i++)
			{
				gamma[i] = 0;

				for (int j = 0; j < gammaForward.Length; j++)
				{
					gamma[i] += gammaForward[j] * weightsForward[j, 1];
				}

				gamma[i] *= TanHDer(outputs[i]);
			}
			for (int i = 0; i < numberOfOutputs; i++)
			{
				for (int j = 0; j < numberOfInputs; j++)
				{
					weightsDelta[i, j] = gamma[i] * inputs[j];
				}
			}
		}

		public void UpdateWeights()
		{
			for (int i = 0; i < numberOfOutputs; i++)
			{
				for (int j = 0; j < numberOfInputs; j++)
				{
					weights[i, j] -= weightsDelta[i, j] * learningRate;
				}
			}
		}
	}


	/// <summary>
	/// Copy weight values to current network
	/// </summary>
	/// <param name="copyWeights">Weights matrix to copy from</param>
	void CopyWeights(float[][][] copyWeights)
	{
		for (int i = 0; i < weights.Length; i++)
		{
			for (int j = 0; j < weights[i].Length; j++)
			{
				for (int k = 0; k < weights[i][j].Length; k++)
				{
					weights[i][j][k] = copyWeights[i][j][k];
				}
			}
		}
	}

	/// <summary>
	/// Create neuron matrix
	/// </summary>
	void InitializeNeurons()
	{
		List<float[]> neuronsList = new List<float[]>(); //temporary list for neurons

		for (int i = 0; i < layer.Length; i++)
		{
			neuronsList.Add(new float[layer[i]]);
		}

		neurons = neuronsList.ToArray();
	}

	/// <summary>
	/// Create weight matrix
	/// </summary>
	void InitializeWeights()
	{
		List<float[][]> weightsList = new List<float[][]>();

		for (int i = 1; i < layer.Length; i++) //cycle through layers
		{
			List<float[]> layerWeightList = new List<float[]>(); //temporary list for weights

			int neuronsInPreviousLayer = layer[i - 1];

			for (int j = 0; j < neurons[i].Length; j++) //cycle through neurons
			{
				float[] neuronWeights = new float[neuronsInPreviousLayer];

				for (int k = 0; k < neuronsInPreviousLayer; k++) //cycle through previous neurons
				{
					neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f); //assign random weight
				}
				layerWeightList.Add(neuronWeights);
			}

			weightsList.Add(layerWeightList.ToArray());
		}

		weights = weightsList.ToArray();
	}

	/// <summary>
	/// Feed forward given input in the network
	/// </summary>
	/// <param name="inputs">Values of the input layer</param>
	/// <returns>Values of the network's output layer</returns>
	public float[] FeedForward(float[] inputs)
	{
		#region Old Code
		/*
		for (int i = 0; i < inputs.Length; i++) //assign input values to the network
		{
			neurons[0][i] = inputs[i];
		}

		for (int i = 1; i < layer.Length; i++) //cycle through layers
		{
			for (int j = 0; j < neurons[i].Length; j++) //cycle through neurons
			{
				float value = 0.25f; //bias

				for (int k = 0; k < neurons[i - 1].Length; k++) //cycle through weights
				{
					value += weights[i - 1][j][k] * neurons[i - 1][k]; //multiply value by weight
				}

				neurons[i][j] = (float)System.Math.Tanh(value);
			}
		}

		return neurons[neurons.Length - 1]; //return output layer
		*/
		#endregion

		layers[0].FeedForward(inputs);
		for (int i = 1; i < layers.Length; i++)
		{
			layers[i].FeedForward(layers[i - 1].outputs);
		}

		return layers[layers.Length - 1].outputs;
	}

	/// <summary>
	/// Back-propagates the expected value
	/// </summary>
	public void BackProp(float[] expected)
	{
		for (int i = layers.Length-1; i >=0; i--)
		{
			if (i == layers.Length - 1)
			{
				layers[i].BackPropOutput(expected);
			}
			else
			{
				layers[i].BackPropHidden(layers[i+1].gamma, layers[i+1].weights);
			}
		}

		for (int i = 0; i < layers.Length; i++)
		{
			layers[i].UpdateWeights();
		}
	}

	#region Reproduction

	/// <summary>
	/// Mutate the current network. Has a 0.8% chance to do something.
	/// </summary>
	public void Mutate()
	{
		for (int i = 0; i < weights.Length; i++)
		{
			for (int j = 0; j < weights[i].Length; j++)
			{
				for (int k = 0; k < weights[i][j].Length; k++)
				{
					float weight = weights[i][j][k];

					float randomNumber = (float)rnd.NextDouble();

					if (randomNumber <= 0.002f)
					{
						//flip sign of weight
						weight *= -1;
					}
					else if (randomNumber <= 0.004f)
					{
						//pick random between -1 and 1
						weight = (float)rnd.NextDouble()-0.5f;
					}
					else if (randomNumber <= 0.006f)
					{
						//randomly increase
						float factor = (float)rnd.NextDouble() + 1f;
						weight *= factor;
					}
					else if (randomNumber <= 0.008f)
					{
						//randomly decrease
						float factor = (float)rnd.NextDouble();
						weight *= factor;
					}

					weights[i][j][k] = weight;
				}
			}
		}
	}
	/// <summary>
	/// Mutate single weight
	/// </summary>
	public float Mutate(float weight)
	{
		float randomNumber = (float)rnd.NextDouble();

		if (randomNumber <= 0.002f)
		{
			//flip sign of weight
			weight *= -1;
		}
		else if (randomNumber <= 0.004f)
		{
			//pick random between -1 and 1
			weight = (float)rnd.NextDouble() - 0.5f;
		}
		else if (randomNumber <= 0.006f)
		{
			//randomly increase
			float factor = (float)rnd.NextDouble() + 1f;
			weight *= factor;
		}
		else if (randomNumber <= 0.008f)
		{
			//randomly decrease
			float factor = (float)rnd.NextDouble();
			weight *= factor;
		}

		return weight;
	}

	/// <summary>
	/// Mix network's weights with other network's
	/// </summary>
	/// <returns>New weight matrix</returns>
	public float[][][] MixGenes(NeuralNetwork other)
	{
		float[][][] newWeights = weights;
		for (int i = 0; i < newWeights.Length; i++)
		{
			for (int j = 0; j < newWeights[i].Length; j++)
			{
				for (int k = 0; k < newWeights[i][j].Length; k++)
				{
					float randomNumber = (float)rnd.NextDouble();
					if (randomNumber < 0.01f)   // copy mate's gene - 1%
					{
						newWeights[i][j][k] = other.weights[i][j][k];
					}
					else if (randomNumber < 0.525f) // copy and mutate mate's gene - 49.25%
					{
						newWeights[i][j][k] = Mutate(other.weights[i][j][k]);
					}
					else if (randomNumber < 0.995f) // leave and mutate original gene - 49.25%
					{
						newWeights[i][j][k] = Mutate(weights[i][j][k]);
					}
					// leave gene as it was - 0.5%
				}
			}
		}
		return newWeights;
	}
	/// <summary>
	/// Mix network's weights with weight matrix
	/// </summary>
	/// <returns>New weight matrix</returns>
	public float[][][] MixGenes(float[][][] otherWeights)
	{
		float[][][] newWeights = weights;
		for (int i = 0; i < newWeights.Length; i++)
		{
			for (int j = 0; j < newWeights[i].Length; j++)
			{
				for (int k = 0; k < newWeights[i][j].Length; k++)
				{
					float randomNumber = (float)rnd.NextDouble();
					if (randomNumber < 0.01f)   // copy mate's gene - 1%
					{
						newWeights[i][j][k] = otherWeights[i][j][k];
					}
					else if (randomNumber < 0.525f) // copy and mutate mate's gene - 49.25%
					{
						newWeights[i][j][k] = Mutate(otherWeights[i][j][k]);
					}
					else if (randomNumber < 0.995f) // leave and mutate original gene - 49.25%
					{
						newWeights[i][j][k] = Mutate(weights[i][j][k]);
					}
					// leave gene as it was - 0.5%
				}
			}
		}
		return newWeights;
	}

	/// <summary>
	/// Simulate asexual reproduction
	/// </summary>
	/// <returns>Child <see cref="NeuralNetwork"/></returns>
	public NeuralNetwork Reproduce()
	{
		NeuralNetwork child = new NeuralNetwork(this);
		child.Mutate();
		return child;
	}
	/// <summary>
	/// Simulate sexual reproduction
	/// </summary>
	/// <param name="other">Mate <see cref="NetworkPattern"/></param>
	/// <param name="doubleMutation">Whether to execute mutation on both the weights and the network</param>
	/// <returns>Child <see cref="NeuralNetwork"/></returns>
	public NeuralNetwork Reproduce(NeuralNetwork other, bool doubleMutation = false)
	{
		NeuralNetwork child = new NeuralNetwork(this);
		child.CopyWeights(MixGenes(other));
		if (doubleMutation)
		{
			child.Mutate();
		}
		return child;
	}

	#endregion

	#region Fitness

	public void AddFitness(float fit)
	{
		fitness += fit;
	}
	public void SetFitness(float fit)
	{
		fitness = fit;
	}
	public float GetFitness()
	{
		return fitness;
	}

	/// <summary>
	/// Compare and sort two <see cref="NeuralNetwork">NeuralNetworks</see> based on fitness
	/// </summary>
	public static int Compare(NeuralNetwork a, NeuralNetwork b)
	{
		if (b == null) return 1;
		else if (a == null) return -1;

		if (a.GetFitness() > b.GetFitness())
			return -1;
		else if (a.GetFitness() < b.GetFitness())
			return 1;
		else
			return 0;
	}

	#endregion

	#region Save & Load

	/// <summary>
			/// Saves network to directory
			/// </summary>
			/// <returns>Path to file</returns>
			/// <param name="directory">Directory for the network to be saved to</param>
			/// <param name="filename">Name of the saved file</param>
			/// <example>SaveNetwork("/Networks/","My Network")</example>
	public string SaveNetwork(string directory = "/Data/Networks/", string filename = "New Network")
	{
		DateTime now = DateTime.Now.ToLocalTime();
		string path = Application.dataPath + directory + filename + "-" + now.Year + now.Month + now.Day + "_" + now.Hour + now.Minute + now.Second + ".net";

		if (!Directory.Exists(Application.dataPath + directory))
			Directory.CreateDirectory(Application.dataPath + directory);

		FileStream stream = File.Open(path, FileMode.OpenOrCreate);
		formatter.Serialize(stream, weights);
		stream.Close();

		return path;
	}
	/// <summary>
	/// Save network to path
	/// </summary>
	/// <returns>Path to file</returns>
	public string SaveNetwork(string path)
	{
		FileStream stream = File.Open(path, FileMode.OpenOrCreate);
		formatter.Serialize(stream, weights);
		stream.Close();
		return path;
	}

	public bool LoadNetwork(string path)
	{
		if (File.Exists(path))
		{
			FileStream stream = File.OpenRead(path);
			float[][][] newWeights = (float[][][])formatter.Deserialize(stream);
			stream.Close();
			weights = newWeights;
			return true;
		}
		return false;
	}

	#endregion

	/// <summary>
	/// Selects an n number of networks with the best fitness out of the array
	/// </summary>
	public static NeuralNetwork[] SelectBest(NeuralNetwork[] nets, int n)
	{
		if (n < 2)
		{
			Debug.LogError("Index out of range");
			return null;
		}

		List<NeuralNetwork> netsList = nets.ToList();
		netsList.Sort(Compare);
		NeuralNetwork[] bestNets = new NeuralNetwork[n];
		for (int i = 0; i < n; i++)
		{
			bestNets[i] = netsList[i];
		}
		return bestNets;
	}
}