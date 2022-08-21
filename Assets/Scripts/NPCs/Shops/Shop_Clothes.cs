using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_Clothes : Shop
{
    void Update()
    {
		if (shopInteracting)
		{
			if (Input.GetAxis("Horizontal") < 0 && activePanel == 0)
			{
				PressPlusKey();
			}
			else if (Input.GetAxis("Horizontal") > 0 && activePanel == 0)
			{
				PressMinusKey();
			}
			if (Input.GetAxis("Vertical") > 0 && activePanel == 1)
			{
				PressPlusKey();
			}
			else if (Input.GetAxis("Vertical") < 0 && activePanel == 1)
			{
				PressMinusKey();
			}
			else if (Input.GetButtonDown("Submit") && !pressTime)
			{
				switch (activePanel)
				{
					case 0:
						switch (options)
						{
							case 0:
								break;
							case 1:
								StartCoroutine(ShowSellList());
								break;
							case 2:
								shopInteracting = false;
								StartCoroutine(HideMainMenu());
								break;
							default:
								break;
						}
						break;
					case 1:
						if (buttonList[1][options].tag == "Sell_Panel")
						{
							Destroy(playerItems[options].gameObject);
							StartCoroutine(QuickUpdateList());
						}
						else
						{
							StartCoroutine(HideSellList());
						}
						break;
					case 2:
						break;
					default:
						break;
				}				
			}
		}		
	}
}
