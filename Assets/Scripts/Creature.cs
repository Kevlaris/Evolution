using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ICreature;

public interface ICreature
{
	public NeuralNetwork Net { get; set; }
	Color Color { get; }
	Texture Texture { get; }

	public enum ReproductionType
	{
		DIVISION,
		ASEXUAL,
		SEXUAL
	}
	public enum DietType
	{
		HERBIVORE,
		CARNIVORE
	}

	abstract void Move();
	abstract float[] GetOutputs();
	public abstract void Reproduce();
	public abstract void Die();
}

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Creature : MonoBehaviour, ICreature
{
	Rigidbody2D rb;
	bool initialised = false;

	NeuralNetwork net;
	Color color;
	Texture texture;
	DietType diet;
	ReproductionType reproduction;

	int receptors;
	int fov;
	float viewingDistance;
	float speed;
	float rotationSpeed;
	float angle;

	public NeuralNetwork Net { get => net; set => net = value; }
	public Color Color { get => color; }
	public Texture Texture { get => texture; }
	public int Receptors { get => receptors; }
	public int Fov { get => fov; }
	public float ViewingDistance { get => viewingDistance; }
	public float Speed { get => speed; }
	public float RotationSpeed { get => rotationSpeed; }
	public float Angle { get => angle; }
	public DietType Diet { get => diet; }
	public ReproductionType Reproduction { get => reproduction; }

	public virtual void Init()
	{
		rb = gameObject.GetComponent<Rigidbody2D>();
		initialised = true;
	}

	protected float[] GetAngles(int fov, int receptors)
	{
		if (receptors < 0)
		{
			Debug.LogError("Receptor count less than 1");
			return null;
		}
		else
		{
			float[] angles = new float[receptors];
			for (int i = 0; i < angles.Length; i++)
			{
				angles[i] = fov / (receptors - 1) * (i);
			}
			return angles;
		}
	}

	public float[] GetOutputs()
	{
		if (initialised)
		{
			float[] inputs = new float[receptors];
			float[] angles = GetAngles(fov, receptors);
			for (int i = 0; i < receptors; i++)
			{
				RaycastHit2D hit = Physics2D.Raycast(transform.position, (Vector3)(Quaternion.AngleAxis(angles[i], transform.forward) * transform.up), viewingDistance, 8);
				if (hit)
				{
					ICreature creature = hit.transform.GetComponent<ICreature>();
					Color.RGBToHSV(creature.Color, out float hue, out float saturation, out float value);
					inputs[i] = hue * saturation / value * hit.distance;
				}
			}
			return net.FeedForward(inputs);
		} else
		{
			return null;
		}
	}

	public virtual void Move()
	{
		angle = transform.eulerAngles.z % 360f;
		if (angle < 0f) angle += 360f;
		else if (angle > 360f) angle -= 360f;

		float[] outputs = GetOutputs();

		rb.velocity = speed * transform.right * outputs[0];
		rb.angularVelocity = rotationSpeed * outputs[1];
	}

	public virtual void Reproduce()
	{
		ICreature child;
		switch (Reproduction)
		{
			case ReproductionType.DIVISION:
				for (int i = 0; i < 2; i++)
				{
					child = Instantiate(gameObject, transform.position, transform.rotation, transform.parent).GetComponent<ICreature>();
					child.Net = new NeuralNetwork(Net);
					child.Net.Mutate();
					child.Net.SetFitness(0);
				}
				Destroy(gameObject);
				break;
			case ReproductionType.ASEXUAL:
				child = Instantiate(gameObject, transform.position, transform.rotation, transform.parent).GetComponent<ICreature>();
				child.Net = new NeuralNetwork(Net);
				child.Net.Mutate();
				child.Net.SetFitness(0);
				break;
			case ReproductionType.SEXUAL:
				break;
			default:
				break;
		}
	}

	public virtual void Die()
	{
		//death animation or fx
		Destroy(gameObject);
	}

	void OnDrawGizmosSelected()
	{
		for (int i = 0; i < receptors; i++)
		{
			float[] angles = GetAngles(fov, receptors);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, (Vector3)(Quaternion.AngleAxis(angles[i], transform.forward) * transform.up) * viewingDistance);
		}
	}
}

[CreateAssetMenu]
public class CreatureData : ScriptableObject
{
	public string creatureName;
	public Color color;
	public Texture texture;
	public ReproductionType reproductionType;
	public DietType dietType;
	public NetworkPattern networkPattern;
}