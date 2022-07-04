using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
	public NetworkPattern defaultPattern;
	public UnityEngine.Object creaturePrefab;
	public Transform hex;

	System.Random rnd = new System.Random();

	[Header("Simulation Settings")]
	[Min(1)] public int population = 25;
	[SerializeField] protected int generation = 0;
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
			NextGeneration();
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

	void NextGeneration()
	{
		isTraining = false;
		if (generation == 0)
		{
			InitiateCreatures();
		}
		else
		{
			boomerangs.Sort(CompareBoomerangs);
			List<NeuralNetwork> goodNets = new List<NeuralNetwork>();
			for (int i = 0; i < boomerangs.Count / 2; i++)
			{
				goodNets.Add(boomerangs[i].net);
			}
			for (int i = (boomerangs.Count/2)-1; i < boomerangs.Count/2; i++)
			{
				boomerangs[i].net = new NeuralNetwork(goodNets[rnd.Next(goodNets.Count)]);
				boomerangs[i].net.Mutate();
			}
			for (int i = 0; i < boomerangs.Count; i++)
			{
				boomerangs[i].transform.position = Vector3.zero;
				boomerangs[i].net.SetFitness(0);
				boomerangs[i].name = "Boomerang " + i;
			}
		}

		generation++;
		generationText.text = "Generation " + generation;
		currentTime = 0;

		isTraining = true;
	}
	
	void InitiateCreatures()
	{
		for (int i = 0; i < population; i++)
		{
			Boomerang boomerang = ((GameObject)Instantiate(creaturePrefab, transform)).GetComponent<Boomerang>();
			boomerang.Init(new NeuralNetwork(defaultPattern), hex);
			boomerang.name = "Boomerang " + i;
			boomerangs.Add(boomerang);
		}
	}

	/// <summary>
	/// Compare and sort two <see cref="NeuralNetwork">NeuralNetworks</see> based on fitness
	/// </summary>
	public int CompareNetworks(NeuralNetwork a, NeuralNetwork b)
	{
		if (b == null) return 1;

		if (a.GetFitness() > b.GetFitness())
			return -1;
		else if (a.GetFitness() < b.GetFitness())
			return 1;
		else
			return 0;
	}
	/// <summary>
	/// Compare and sort two <see cref="Boomerang">Boomerangs</see> based on fitness
	/// </summary>
	public int CompareBoomerangs(Boomerang a, Boomerang b)
	{
		if (b == null) return 1;

		if (a.net.GetFitness() > b.net.GetFitness())
			return -1;
		else if (a.net.GetFitness() < b.net.GetFitness())
			return 1;
		else
			return 0;
	}
}