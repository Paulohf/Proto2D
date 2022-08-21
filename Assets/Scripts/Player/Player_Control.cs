using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Control : MonoBehaviour
{
    public GameObject clothes;

    public float speed; //Player Speed
    public Animator[] anim; //Array of Animator, to sync the animations of the Character and Clothes
    Rigidbody2D rb;

    bool[] blockInt = new bool[4]; //Array of Block to prevent the script to send unecessary commands
    bool blockIdle; //Same as above, but for Idle
    public static bool interacting; //if the player is talking/checking/interacting with a NPC or Event, this is to disable player movement
    bool canInteract; //if the player is in a trigger that he can intereact (If all criteria met)

    Actor_Trigger at;

    public enum Direction //Where the Player is facing, to check if he's in the same direction of a NPC
    {
        Up,
        Down,
        Left,
        Right
    };
    [HideInInspector] public Direction direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //Get Rigidbody from player
        anim = GetComponentsInChildren<Animator>(); //Get clothes Animator from player
    }

	private void FixedUpdate()
	{
        if (!interacting)
            rb.velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed; //Character movement
    }

	void Update()
    {        
        //This is for animation, when pressed W, A, S, D or Arrows Keys, this script will change Direction variable and send animation var only one time, "else" is Idle
        if (Input.GetAxis("Horizontal") < 0)
            Movement(4);

        else if(Input.GetAxis("Horizontal") > 0)
            Movement(3);

        else if (Input.GetAxis("Vertical") < 0)
            Movement(2);

        else if (Input.GetAxis("Vertical") > 0)
            Movement(1);

        else
            if (!blockIdle)// If is the first time in idle before any player movement
            {
                rb.velocity = Vector2.zero; //Reset movemnt
                foreach (Animator a in anim) //Set all animator (Clothes and Player) to idle (In that case, Zero is Idle)
                    a.SetInteger("Direction", 0);

                for (int i = 0; i < blockInt.Length; i++) //And enable all movement to send a int parameter to Animator again
                    blockInt[i] = false;

                blockIdle = true;//Block the Idle, preventing all the loops to trigger again
            }

		if (Input.GetButtonDown("Submit") && canInteract && !interacting)
		{
            at.StartActorBehavior();
            interacting = true;
		}

    }

    void Movement(int pDirection)// Here is the samething in idle
    {
		if (!interacting)
		{
            blockIdle = false;//Enable Idle to send int parameter

            if (!blockInt[pDirection - 1])//check if is the first time after a movement or idle
            {
                foreach (Animator a in anim)//send all animator the direction animation
                    a.SetInteger("Direction", pDirection);

                for (int i = 0; i < blockInt.Length; i++)//block the loop of the movement and enable all the another
                {
                    if (i == (pDirection - 1)) blockInt[i] = true;
                    else blockInt[i] = false;
                }

                switch (pDirection)
                {
                    case 1:
                        direction = Direction.Up;
                        break;
                    case 2:
                        direction = Direction.Down;
                        break;
                    case 3:
                        direction = Direction.Right;
                        break;
                    case 4:
                        direction = Direction.Left;
                        break;
                    default:
                        break;
                }
            }
        }		
	}

    private void OnTriggerStay2D(Collider2D actor)
    {
        at = actor.GetComponent<Actor_Trigger>();

		if (direction == at.direction)
		{
            canInteract = true;
        }
		else
		{
            canInteract = false;
        }
    }
}
