using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Boomerang : MonoBehaviour
{
	bool initialized = false;

	[Header("Movement")]
	Rigidbody2D rb;
	[SerializeField] Transform hex;
	[SerializeField] float speed = 1f;
	[SerializeField] float rotationSpeed = 250f;
	[Range(0, 360)] public float angle = 0f;

	[Header("Artificial Intelligence")]
	[SerializeField] float fitness;
	[SerializeField] public NeuralNetwork net;
	float[] inputs;
	[SerializeField] float input;
	[SerializeField] float output;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (initialized)
		{
			Move();
			fitness = net.GetFitness();
		}
	}

	public void Init(NeuralNetwork net, Transform hex)
	{
		this.net = net;
		this.hex = hex;
		initialized = true;
	}

	public void Move()
	{
		float distance = Vector2.Distance(transform.position, hex.position);
		if (distance > 20f) distance = 20f;
		gameObject.GetComponent<SpriteRenderer>().color = new Color(distance / 20f, 1f - (distance / 20f), 1f - (distance / 20f));

		inputs = new float[1];

		angle = transform.eulerAngles.z % 360f;
		if (angle < 0f) angle += 360f;
		else if (angle > 360f) angle -= 360f;

		Vector2 deltaVector = (hex.transform.position - transform.position).normalized;

		Quaternion forwardRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		Vector2 forwardDirection = forwardRotation * new Vector2(distance, 0f);

		input = Vector2.SignedAngle(deltaVector, forwardDirection);
		inputs[0] = input;

		output = net.FeedForward(inputs)[0];

		rb.velocity = speed * transform.right;
		rb.angularVelocity = rotationSpeed * output;

		net.AddFitness((180f - Mathf.Abs(input)) / distance);

		if (transform.position.x > 18)
		{
			transform.SetPositionAndRotation(new Vector2(18, transform.position.y), transform.rotation);
		}
		if (transform.position.y > 10)
		{
			transform.SetPositionAndRotation(new Vector2(transform.position.x, 10), transform.rotation);
		}
		if (transform.position.x < -18)
		{
			transform.SetPositionAndRotation(new Vector2(-18, transform.position.y), transform.rotation);
		}
		if (transform.position.y < -10)
		{
			transform.SetPositionAndRotation(new Vector2(transform.position.x, -10), transform.rotation);
		}
	}
}
