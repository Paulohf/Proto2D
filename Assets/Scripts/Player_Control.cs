using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Control : MonoBehaviour
{
    public float speed;
    public Animator[] anim;
    Rigidbody2D rb;

    bool[] blockInt = new bool[4];
    bool blockIdle = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentsInChildren<Animator>();
    }

	private void FixedUpdate()
	{
        rb.velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
    }

	void Update()
    {        
        if (Input.GetAxis("Horizontal") < 0)
		{
            PlayAnimation(4);
        }
        else if(Input.GetAxis("Horizontal") > 0)
        {
            PlayAnimation(3);
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            PlayAnimation(2);
        }
        else if (Input.GetAxis("Vertical") > 0)
        {
            PlayAnimation(1);
        }
        else
        {
            if (!blockIdle)
            {
                rb.velocity = Vector2.zero;
                foreach (Animator a in anim)
                    a.SetInteger("Direction", 0);

                for (int i = 0; i < blockInt.Length; i++)
                    blockInt[i] = false;

                blockIdle = true;
            }
        }

    }

    void PlayAnimation(int direction)
    {
		blockIdle = false;
		if (!blockInt[direction - 1])
		{
			foreach (Animator a in anim)
				a.SetInteger("Direction", direction);

			for (int i = 0; i < blockInt.Length; i++)
			{
				if (i == (direction - 1)) blockInt[i] = true;
				else blockInt[i] = false;
			}
		}
	}
}
