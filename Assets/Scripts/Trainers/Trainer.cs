using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class Trainer : MonoBehaviour
{
	protected enum SimulationTypeEnum
	{
		DEFAULT,
		SAVE,
		LOAD,
		SAVE_AND_LOAD
	}

	protected abstract NeuralNetwork Net { get; set; }
	protected abstract NetworkPattern Pattern { get; set; }
	public abstract int NumberOfTests { get; set; }
	protected abstract SimulationTypeEnum SimulationType { get; set; }
	protected abstract string Directory { get; set; }
	protected abstract string FileName { get; set; }
	protected abstract string Path { get; set; }

	protected virtual void Start()
	{
		PreTraining();
		Training();
		PostTraining();
	}

	protected virtual void PreTraining()
	{
		switch (SimulationType)
		{
			case SimulationTypeEnum.LOAD:
				Load(Path);
				break;
			case SimulationTypeEnum.SAVE_AND_LOAD:
				Load(Path);
				break;
			default:
				Net = new NeuralNetwork(Pattern);
				break;
		}
	}

	protected abstract void Training();

	protected virtual void PostTraining()
	{
		switch (SimulationType)
		{
			case SimulationTypeEnum.SAVE:
				Debug.Log("SAVED: " + Net.SaveNetwork(Directory, FileName + "#" + NumberOfTests));
				break;
			case SimulationTypeEnum.SAVE_AND_LOAD:
				DateTime now = DateTime.Now.ToLocalTime();
				Debug.Log("SAVED: " + Net.SaveNetwork(Directory, FileName + "#" + NumberOfTests));
				break;
			default:
				break;
		}
	}

	protected virtual float Train(float[][] inputs, float[][] expected)
	{
		int num;
		if (inputs.Length == 0 || expected.Length == 0)
		{
			Debug.LogError("No parameters, returning -1", this);
			return -1;
		}
		else if (inputs.Length < expected.Length)
		{
			Debug.LogWarning("More expected values than inputs, excess will be ignored", this);
			num = inputs.Length;
		}
		else if (inputs.Length > expected.Length)
		{
			Debug.LogWarning("More inputs than expected values, excess will be ignored", this);
			num = expected.Length;
		}
		else
		{
			num = inputs.Length;
		}

		for (int i = 0; i < num; i++)
		{
			//do the training
		}

		return 0;
	}

	protected virtual void Load(string path)
	{
		if (System.IO.Directory.Exists(path))
		{
			string[] files = System.IO.Directory.GetFiles(path);
			if (files.Length < 1)
			{
				SimulationType = SimulationTypeEnum.DEFAULT;
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

			string file = files[latestFileIndex];

			if (!Net.LoadNetwork(file))
			{
				SimulationType = SimulationTypeEnum.DEFAULT;
				return;
			}

			Debug.Log("LOADED: " + file);
		}
	}
}