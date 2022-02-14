using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class XORTrainer : MonoBehaviour
{
	public enum SimulationType
	{
		DEFAULT,
		SAVE,
		LOAD,
		SAVE_AND_LOAD
	}

	NeuralNetwork net;
	[SerializeField] int numberOfTests = 5000;
	public SimulationType simulationType = SimulationType.DEFAULT;
	[SerializeField] string directory = "/Data/Networks/XOR/";
	[SerializeField] string fileName = "Trained XOR Network";
	string path;
	string file;

	void Start()
	{
		path = Application.dataPath.TrimEnd("/".ToCharArray()) + directory;
		net = new NeuralNetwork(new int[] { 3, 25, 25, 1 });

		//   XOR test
		// 0 0 0	=> 0
		// 0 0 1	=> 1
		// 0 1 0	=> 1
		// 0 1 1	=> 0
		// 1 0 0	=> 1
		// 1 0 1	=> 0
		// 1 1 0	=> 0
		// 1 1 1	=> 1

		switch (simulationType)
		{
			case SimulationType.LOAD:
				Load(path);
				break;
			case SimulationType.SAVE_AND_LOAD:
				Load(path);
				break;
			default:
				break;
		}

		for (int i = 0; i < numberOfTests; i++)
		{
			net.FeedForward(new float[] { 0, 0, 0 });
			net.BackProp(new float[] { 0 });

			net.FeedForward(new float[] { 0, 0, 1 });
			net.BackProp(new float[] { 1 });

			net.FeedForward(new float[] { 0, 1, 0 });
			net.BackProp(new float[] { 1 });

			net.FeedForward(new float[] { 0, 1, 1 });
			net.BackProp(new float[] { 0 });

			net.FeedForward(new float[] { 1, 0, 0 });
			net.BackProp(new float[] { 1 });

			net.FeedForward(new float[] { 1, 0, 1 });
			net.BackProp(new float[] { 0 });

			net.FeedForward(new float[] { 1, 1, 0 });
			net.BackProp(new float[] { 0 });

			net.FeedForward(new float[] { 1, 1, 1 });
			net.BackProp(new float[] { 1 });
		}

		Debug.Log("0 => " + net.FeedForward(new float[] { 0, 0, 0 })[0]);
		Debug.Log("1 => " + net.FeedForward(new float[] { 0, 0, 1 })[0]);
		Debug.Log("1 => " + net.FeedForward(new float[] { 0, 1, 0 })[0]);
		Debug.Log("0 => " + net.FeedForward(new float[] { 0, 1, 1 })[0]);
		Debug.Log("1 => " + net.FeedForward(new float[] { 1, 0, 0 })[0]);
		Debug.Log("0 => " + net.FeedForward(new float[] { 1, 0, 1 })[0]);
		Debug.Log("0 => " + net.FeedForward(new float[] { 1, 1, 0 })[0]);
		Debug.Log("1 => " + net.FeedForward(new float[] { 1, 1, 1 })[0]);

		switch (simulationType)
		{
			case SimulationType.SAVE:
				Debug.Log("SAVED: " + net.SaveNetwork(directory, fileName + "#" + numberOfTests));
				break;
			case SimulationType.SAVE_AND_LOAD:
				DateTime now = DateTime.Now.ToLocalTime();
				Debug.Log("SAVED: " + net.SaveNetwork(directory, fileName + "#" + numberOfTests));
				break;
			case SimulationType.DEFAULT:
				break;
		}
	}

	void Load(string path)
	{
		if (Directory.Exists(path))
		{
			string[] files = Directory.GetFiles(path);
			if (files.Length < 1)
			{
				simulationType = SimulationType.DEFAULT;
				return;
			}
			int latestFileIndex = -1;
			for (int i = 1; i < files.Length; i++)
			{
				if (!files[i].EndsWith(".net"))
				{
					continue;
				}
				else if (latestFileIndex == -1)
				{
					latestFileIndex = i;
				}
				else if (File.GetLastWriteTimeUtc(files[i]) > File.GetLastWriteTimeUtc(files[latestFileIndex]))
				{
					latestFileIndex = i;
				}
			}

			file = files[latestFileIndex];

			if (!net.LoadNetwork(file))
			{
				simulationType = SimulationType.DEFAULT;
				return;
			}

			Debug.Log("LOADED: " + file);
		}
	}
}