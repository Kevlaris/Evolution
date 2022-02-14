using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
	public NetworkPattern defaultPattern;
	public Object creaturePrefab;
	public Transform hex;

	[Header("Simulation Settings")]
	[Min(1)] public int population = 25;
	[SerializeField] int generation = 0;
	[SerializeField] List<NeuralNetwork> nets = new List<NeuralNetwork>();
	List<Boomerang> boomerangs = new List<Boomerang>();
	bool isTraining = false;

	private void Update()
	{
		hex.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if (!isTraining)
		{
			if (generation == 0)
			{
				InstantiateCreatureNetworks(defaultPattern);
			}
			else
			{
				nets.Sort();
				for (int i = 0; i < population / 2; i++)
				{
					nets[i] = new NeuralNetwork(nets[i + (population / 2)]);
					nets[i].Mutate();

					nets[i + (population / 2)] = new NeuralNetwork(nets[i + (population / 2)]);
				}

				for (int i = 0; i < nets.Count; i++)
				{
					nets[i].SetFitness(0f);
				}
			}

			generation++;

			isTraining = true;
			Invoke("Timer", 15f);
			InstanceCreatures();
		}
		else if (Input.GetMouseButtonDown(0))
		{
			//isTraining = false;
		}
	}

	void Timer()
	{
		isTraining = false;
	}

	/// <summary>
	/// Creates new creatures if not enough, deletes some if more, and resets their position.
	/// </summary>
	void InstanceCreatures()
	{
		if (boomerangs.Count < population)
		{
			for (int i = 0; i < population; i++)
			{
				Boomerang boomerang = ((GameObject)Instantiate(creaturePrefab, transform)).GetComponent<Boomerang>();
				boomerang.Init(nets[i], hex);
				boomerangs.Add(boomerang);
			}
		}
		else if (boomerangs.Count > population)
		{
			for (int i = population; i < boomerangs.Count; i++)
			{
				boomerangs.RemoveAt(i);
			}
		}
		for (int i = 0; i < boomerangs.Count; i++)
		{
			boomerangs[i].transform.position = Vector2.zero;
			boomerangs[i].net.SetFitness(0);
		}
	}

	void InstantiateCreatureNetworks(NetworkPattern pattern)
	{
		for (int i = 0; i < population; i++)
		{
			nets.Add(new NeuralNetwork(pattern));
		}
	}
}