using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NOTTrainer : Trainer
{
	NeuralNetwork net;
	NetworkPattern pattern;
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
	protected override NeuralNetwork Net
	{
		get { return net; }
		set { net = value; }
	}
	protected override NetworkPattern Pattern
	{
		get { return pattern; }
		set { pattern = value; }
	}
	protected override SimulationTypeEnum SimulationType
	{
		get { return simulationType; }
		set { simulationType = value; }
	}
	protected override string Directory
	{
		get { return directory; }
		set { directory = value; }
	}
	protected override string FileName
	{
		get { return fileName; }
		set { fileName = value; }
	}
	protected override string Path
	{
		get { return path; }
		set { path = value; }
	}

	protected override void Training()
	{
		throw new System.NotImplementedException();
	}
}