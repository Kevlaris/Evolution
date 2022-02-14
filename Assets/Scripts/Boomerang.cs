using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
	bool initialized = false;

	[Header("Movement")]
	Rigidbody2D rb;
	[SerializeField] Transform hex;
	[SerializeField] float speed = 1f;
	[SerializeField] float rotationSpeed = 250f;
	[Range(0, 360)] public float angle = 0f;	

	Vector2 prevPos;

	[Header("Artificial Intelligence")]
	public NeuralNetwork net;
	float[] inputs;
	[SerializeField] float input;
	[SerializeField] float output;
	[SerializeField] TMPro.TMP_Text fitnessText;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (initialized)
		{
			/* inputs[0] = hex.position.x;
			inputs[1] = hex.position.y;
			output = net.FeedForward(inputs)[0];

			Move(angle + output); */

			float distance = Vector2.Distance(transform.position, hex.position);
			if (distance > 20f) distance = 20f;
			gameObject.GetComponent<SpriteRenderer>().color = new Color(distance / 20f, 1f - (distance / 20f), 1f - (distance / 20f));

			//find out how much the boomerang has to turn to face the hex
			//pass value to neural net
			//set the output as angular velocity to the rigidbody

			inputs = new float[1];

			angle = transform.eulerAngles.z % 360f;
			if (angle < 0f) angle += 360f;
			else if (angle > 360f) angle -= 360f;

			Vector2 deltaVector = (hex.transform.position - transform.position).normalized;

			/*
			float rad = Mathf.Atan2(deltaVector.y, deltaVector.x);
			rad *= Mathf.Rad2Deg;

			rad = rad % 360;
			if (rad < 0)
			{
				rad = 360 + rad;
			}

			rad = 90f - rad;
			if (rad < 0f)
			{
				rad += 360f;
			}
			rad = 360 - rad;
			rad -= angle;
			if (rad < 0)
			{
				rad = 360 + rad;
			}
			if (rad >= 180f)
			{
				rad = 360 - rad;
				rad *= -1f;
			}
			rad *= Mathf.Rad2Deg;

			inputs.SetValue(input = rad / Mathf.PI, 0);
			*/

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

			fitnessText.text = net.GetFitness().ToString();
			fitnessText.GetComponentInParent<Transform>().rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
		}
	}

	public void Init(NeuralNetwork net, Transform hex)
	{
		this.net = net;
		this.hex = hex;
		initialized = true;
	}

	void Move()
	{
		prevPos = transform.position;

		transform.Rotate(0, 0, angle - transform.rotation.eulerAngles.z);
		transform.Translate(speed * Time.deltaTime, 0, 0);

		if (transform.position.x > 18 || transform.position.y > 10 || transform.position.x < -18 || transform.position.y < -10)
		{
			transform.position = prevPos;
		}
		if (angle > 360)
		{
			angle = 0;
		}
		else if (angle < 0)
		{
			angle = 360;
		}
	}
}
