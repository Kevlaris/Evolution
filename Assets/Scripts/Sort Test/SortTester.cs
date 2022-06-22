using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SortTester : MonoBehaviour
{
    [SerializeField] Object elementPrefab;
    [Range(1,6)] [SerializeField] int numberOfElements = 6;
	Transform listObj;

	List<SortListElement> elements = new List<SortListElement>();
	

	private void Start()
	{
		listObj = transform.GetChild(0);

		for (int i = 0; i < numberOfElements; i++)
		{
			elements.Add(((GameObject)Instantiate(elementPrefab, listObj)).GetComponent<SortListElement>());
			elements[i].indexText.text = (i + 1).ToString();
		}
	}

	public void OnValueChange()
	{

	}

	public void SortElements()
	{
		elements.Sort(Compare);
		foreach (var element in elements)
		{
			Debug.Log(element.initialIndex);
		}
	}

	/// <summary>
	/// Sorts two SortListElements based on their nets' fitness.
	/// </summary>
	public int Compare(SortListElement a, SortListElement b)
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