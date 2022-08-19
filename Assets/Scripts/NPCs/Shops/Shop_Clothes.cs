using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_Clothes : Shop
{
    void Update()
    {
		if (Input.GetAxis("Horizontal") < 0)
		{
			PressLeftKey();
		}
		else if (Input.GetAxis("Horizontal") > 0)
		{
			PressRightKey();
		}
		else if (Input.GetButtonDown("Submit") && !pressTime)
		{
			switch (options)
			{
				case 0:
					break;
				case 1:
					break;
				case 2:
					StartCoroutine(HideMainMenu());
					break;
				default:
					break;
			}
		}
	}
}
