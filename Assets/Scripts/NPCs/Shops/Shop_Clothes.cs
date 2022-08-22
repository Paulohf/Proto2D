using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop_Clothes : Shop //Shop_Clothes inherit all the function of the Shop, to it's more easy to create all kinds of shops, since Sell and Exit is already done
{
	List<GameObject> buyClothesList = new List<GameObject>(); //The list of itens that the NPC can sell
	void Update()
    {
		if (shopInteracting)
		{
			if (Input.GetAxis("Horizontal") < 0)//Navigate system
			{
				PressLeftKey();
			}
			else if (Input.GetAxis("Horizontal") > 0)//Navigate system
			{
				PressRightKey();
			}
			else if (Input.GetButtonDown("Submit") && !pressTime)//In case o the player select a option
			{
				switch (activePanel)//Check the panel that is active
				{
					case 0://In the panel 0 (Main menu)..
						switch (options)//There is 3 buttons...
						{
							case 0:	//Buy...
								BuyList();
								StartCoroutine(ShowShopList(1));
								break;
							case 1: //Sell...
								SellList();
								StartCoroutine(ShowShopList(1));
								break;
							case 2: //Exit...
								shopInteracting = false;
								StartCoroutine(HideMainMenu());
								break;
							default:
								break;
						}
						break;
					case 1://This is a rushed part that I haven't finished yet, so it's ugly
						if (buttonList[1][options].tag == "Sell_Panel")//if clicking in a sell item...
						{
							StartCoroutine(QuickUpdateList());
						}
						else if (buttonList[1][options].tag == "Buy_Panel")//if clicking in a buy item...
						{
							UpdateClothes();
						}
						else
						{
							StartCoroutine(HideShopList()); //Else hide the shop and back to the main menu
						}
						break;
					default:
						break;
				}				
			}
		}		
	}
	
	void UpdateClothes()//This is also rushed...
	{
		foreach (SpriteRenderer item in playerItems)//Just check the buy item and add to the player "Inventory"
		{
			if (buyClothesList[options].tag == item.tag)
			{
				Destroy(item.gameObject);
				GameObject newClothe = Instantiate(buyClothesList[options]);
				newClothe.transform.SetParent(pcScript.clothes.transform);
				newClothe.transform.localPosition = Vector2.zero;
				newClothe.transform.localScale = new Vector2(1, 1);
				playerItems[options] = newClothe.GetComponent<SpriteRenderer>();
			}
		}
	}

	void BuyList()//Create the buy list and is almost identicall to the SellList in Shop.cs
	{
		List<GameObject> shirtsList = new List<GameObject>(); //List of Shirts
		List<GameObject> legsList = new List<GameObject>(); //List of Legs
		List<GameObject> bootsList = new List<GameObject>();//List of Boots

		firstPanel = Instantiate(scScript.shopPanel);//Instantiate the background panel
		firstPanel.transform.SetParent(scScript.transform);//Set parent to the main canvas

		buyClothesList.Clear();//clear the buying list (will be null in the first time)
		itemList.Clear();//Clear the buttons

		Vector2 panelZeroPos = new Vector2(100, -100);//This is the position of the panel when there's no item, this is for refence, so the panel will always in the same location with diferent sizes

		float offset = 0.5f; //Offset value, how much the panel need to move for each pixel size
		float padding = 10; //A padding to have a space between the items

		player = GameObject.FindWithTag("Player");//Fingind the player
		pcScript = player.GetComponent<Player_Control>();//Get player script
		playerItems = pcScript.clothes.GetComponentsInChildren<SpriteRenderer>();//Get player items

		List<List<GameObject>> buyList = new List<List<GameObject>>();//A list of list, to create rows, first row will be the shirts, second will be the legs and the third will be the boots
		int highestX = 0;//Var to check which row has more item, to define the panel size

		shirtsList.AddRange(Resources.LoadAll("Clothes/Shirts").Cast<GameObject>().ToArray()); //Get all the shirts in the Resources/Clothes/Shirts
		legsList.AddRange(Resources.LoadAll("Clothes/Legs").Cast<GameObject>().ToArray()); //Get all the legs in the Resources/Clothes/legs
		bootsList.AddRange(Resources.LoadAll("Clothes/Boots").Cast<GameObject>().ToArray()); //Get all the boots in the Resources/Clothes/Boots

		buyList.Add(shirtsList);//
		buyList.Add(legsList);  //
		buyList.Add(bootsList); //Add all the item in the list

		for (int i = 0; i < buyList.Count; i++)//Check the highest list
		{
			if (buyList[i].Count > highestX) highestX = buyList[i].Count;
		}

		RectTransform panelTransorm = firstPanel.GetComponent<RectTransform>();//Get the RectTransform of the panel

		float rectX = scScript.itemButtonTemplate.GetComponent<RectTransform>().rect.width;//Get the width of the buttons
		float rectY = scScript.itemButtonTemplate.GetComponent<RectTransform>().rect.height;//Get the height of the buttons

		float sizeX = (rectX * highestX) + (padding * 2); //calculate the X size of the Panel
		float sizeY = (rectY * (buyList.Count + 1)) + (padding * 2); //calculate the Y size of the Panel

		float posX = (sizeX * offset) + (panelZeroPos.x / 2); //calculate the X position of the Panel
		float posY = (sizeY * -offset) + (panelZeroPos.y / 2); //calculate the Y position of the Panel

		Vector2 panelSize = new Vector2(sizeX, sizeY);
		Vector2 panelPos = new Vector2(posX, posY);

		panelTransorm.anchoredPosition = panelPos;
		panelTransorm.sizeDelta = panelSize;

		for (int i = 0; i < buyList.Count; i++) //creating the button for each item
		{
			for (int j = 0; j < buyList[i].Count; j++)
			{
				GameObject buyButton = Instantiate(scScript.itemButtonTemplate);//Instantiate the button
				RectTransform rtBuyButton = buyButton.GetComponent<RectTransform>();///Get the REctTransform
				buyButton.transform.SetParent(firstPanel.transform);//Set parent to the panel
				Image imageButton = buyButton.transform.GetChild(0).GetComponent<Image>();//Get the image of the buttons
				buyButton.tag = "Buy_Panel";//Add tag

				float x = padding + (sizeX / -2) + ((j + 1) * (sizeX / (buyList[i].Count + 1)));//Calculating the X position of the button
				float y = padding + (sizeY / 2) - ((i + 1) * (sizeY / (buyList.Count + 1)));//Calculating the Y position of the button

				rtBuyButton.anchoredPosition = new Vector2(x, y); // set the position

				imageButton.sprite = buyList[i][j].GetComponent<SpriteRenderer>().sprite;//Get the sprite of the item

				itemList.Add(buyButton.GetComponent<Button>());//Add to the button list
				buyClothesList.Add(buyList[i][j]);//Add to the item list
			}
		}

		GameObject exitButton = Instantiate(scScript.buttonTemplate);//Same process, but for the Exit button
		RectTransform rtExitButton = exitButton.GetComponent<RectTransform>();
		exitButton.transform.SetParent(firstPanel.transform);
		exitButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Exit";

		float ExitY = padding + (sizeY / 2) - ((buyList.Count + 1) * (sizeY / (buyList.Count + 1)));

		rtExitButton.anchoredPosition = new Vector2(0, ExitY);

		itemList.Add(exitButton.GetComponent<Button>());

		buttonList.Add(itemList);
		HighlightButton(buttonList[1]);
	}
}
