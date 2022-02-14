using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NOTTrainer : Trainer
{
	NeuralNetwork net;
	int numberOfTests;
	SimulationTypeEnum simulationType;
	string directory;
	string fileName;
	string path;
	string file;

	public override int NumberOfTests
	{
		get { return numberOfTests; }
		set { numberOfTests = value; }
	}
	protected override NeuralNetwork Net { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
	protected override SimulationTypeEnum SimulationType { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
	protected override string Directory { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
	protected override string FileName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
	protected override string Path { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

	protected override void Train()
	{
		throw new System.NotImplementedException();
	}
}