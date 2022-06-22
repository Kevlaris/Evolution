using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
	public NetworkPattern defaultPattern;
	public UnityEngine.Object creaturePrefab;
	public Transform hex;

	[Header("Simulation Settings")]
	[Min(1)] public int population = 25;
	[SerializeField] int generation = 0;
	[SerializeField] List<NeuralNetwork> nets = new List<NeuralNetwork>();
	[SerializeField] List<Boomerang> boomerangs = new List<Boomerang>();
	bool isTraining = false;

	[Header("Timer Settings")]
	[SerializeField] TMPro.TextMeshProUGUI timerText;
	[SerializeField] TMPro.TextMeshProUGUI generationText;
	float currentTime;
	[SerializeField] int timeBetweenGenerations = 15;

	private void Update()
	{
		hex.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

		currentTime += Time.deltaTime;
		timerText.text = TimeSpan.FromSeconds(currentTime).ToString(@"mm\:ss\.f");

		if (!isTraining)
		{
			if (generation == 0)
			{
				currentTime = 0;
				InstantiateCreatureNetworks(defaultPattern);
			}
			else
			{
				nets.Sort(Compare);
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
			generationText.text = "Generation " + generation;
			currentTime = 0;

			isTraining = true;
			InstanceCreatures();
		}
		else
		{
			if (currentTime >= timeBetweenGenerations)
			{
				isTraining = false;
			}
			else if (Input.GetKeyDown(KeyCode.Space))
			{
				isTraining = false;
			}
		}
		
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

	/// <summary>
	/// Compare and sort two networks based on fitness
	/// </summary>
	/// <param name="other">Network to be compared to</param>
	/// <returns>1 if original network is superior, 0 if equal, or -1 if inferior to other network</returns>
	public int Compare(NeuralNetwork a, NeuralNetwork b)
	{
		if (b == null) return 1;

		if (a.GetFitness() > b.GetFitness())
			return -1;
		else if (a.GetFitness() < b.GetFitness())
			return 1;
		else
			return 0;
	}
}