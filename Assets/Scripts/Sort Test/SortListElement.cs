using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SortListElement : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI indexText;
    [SerializeField] TMP_InputField fitnessInput;
    public NeuralNetwork net;
	public int initialIndex;

	private void Start()
	{
		net = new NeuralNetwork(new int[] { 1, 3, 1 });
		initialIndex = int.Parse(indexText.text);
	}

	private void Update()
	{
		fitnessInput.text = net.GetFitness().ToString();
	}

	public void OnValueChange(string s)
	{
		net.SetFitness(float.Parse(s));
	}
}
