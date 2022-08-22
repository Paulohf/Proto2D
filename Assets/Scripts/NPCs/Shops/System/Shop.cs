using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
	[HideInInspector] public GameObject player; //Player GameObject
	[HideInInspector] public Player_Control pcScript; //Player Script
	[HideInInspector] public SpriteRenderer[] playerItems; //Player item in the "inventory"

	[HideInInspector] public bool shopInteracting; //Check if the player is interacting with the shop

	public Canvas maincanvas; //This is the main shop screen, here you will add what will appear to the player
	Canvas mc;
	[HideInInspector] public Shop_Canvas scScript; //The script of the Shop_canvas

	bool waitMain; //Those booleans are used to make the "show up" effect of the menu
	bool waitSub; //The waitSub is important to not block/unblock the other routine, this way 2 coroutine can run and wait at same time

	[HideInInspector] public bool pressTime; //bool to add delay the buttons
	float pressTimeDelay = 0.2f; //This one is the delay between one press of the button and another

	float fadeTime = 0.1f; //time to fade in/out

	[HideInInspector] public int options = 0; //this is the button showing at moment to player, 0 means first button, 1 the second and so on
	[HideInInspector] public int activePanel = 0; //this is the panel, 0 mean the main, that leads to another section of the shop (buy, sell or exit), in this case 1 can be any other option
												  //and 2 an "option of previous option", like 0> Main menu | 1> Buy Menu | 2> Confirmation Menu

	[HideInInspector] public GameObject firstPanel; //Panels to instantiate a background to item lists
	[HideInInspector] public GameObject secondPanel;

	[HideInInspector] public List<Button> itemList = new List<Button>(); //The list of items that will appear to the player, if selling, will be all items player, buying, all NPC items
	List<Button> mainMenu = new List<Button>(); //Main menu buttons (Buy/Sell/Exit)

	[HideInInspector] public List<List<Button>> buttonList = new List<List<Button>>(); //A list of button list, the button list hold all buttons, the list of list, hold all the buttons so far,
																					   //in this case List[0] will be the main menu, List[1] can be buy or sell buttons 

	Color highlightColor = new Color32(0, 103, 255, 255); //Color of the buttons to tell the player which button is selected
	Color normalColor = new Color32(180, 210, 255, 255); //And which is not

	public void ActorBehavior() 
	{
		options = 0; //first option that will start
		activePanel = 0; //This option need to be 0 at start

		mc = Instantiate(maincanvas); //Instantiate main canvas into a variable
		mc.GetComponentsInChildren(mainMenu); //Get Buttons of the main menu
		scScript = mc.GetComponent<Shop_Canvas>(); //Get Sthe script of the main menu

		buttonList.Add(mainMenu); //Add the buttons to the List of button lists

		StartCoroutine(ShowMainMenu());	//Show the menu to the player
	}

	public IEnumerator ShowMainMenu()
	{
		StartCoroutine(FadeIn(fadeTime, scScript.welcomePanel)); //Fade in the main menu
		waitMain = true; //Block the coroutine
		yield return new WaitUntil(() => !waitMain); //unblock when the fade in/out it's done

		StartCoroutine(ShowButtons()); //Fade in the buttons

		HighlightButton(buttonList[0]); //Highlight the button

		yield return null;
	} //Show the main menu

	public IEnumerator HideMainMenu() //Hide the main menu
	{
		StartCoroutine(FadeOut(fadeTime, scScript.exitButton)); //Fade out the buttons
		waitMain = true;//block the corroutine
		yield return new WaitUntil(() => !waitMain);//And unblock when the fade in/out it's done

		StartCoroutine(FadeOut(fadeTime, scScript.sellButton));
		waitMain = true;
		yield return new WaitUntil(() => !waitMain);

		StartCoroutine(FadeOut(fadeTime, scScript.buyButton));
		waitMain = true;
		yield return new WaitUntil(() => !waitMain);

		StartCoroutine(FadeOut(fadeTime, scScript.welcomePanel));
		waitMain = true;
		yield return new WaitUntil(() => !waitMain);

		Destroy(mc.gameObject); //Destroy shop canvas
		options = 0; //reset options
		activePanel = 0;

		Player_Control.interacting = false; //release the player

		yield return null;
	}

	public IEnumerator ShowButtons()
	{
		HighlightButton(buttonList[0]);

		StartCoroutine(FadeIn(fadeTime, scScript.buyButton));
		waitMain = true;
		yield return new WaitUntil(() => !waitMain);

		StartCoroutine(FadeIn(fadeTime, scScript.sellButton));
		waitMain = true;
		yield return new WaitUntil(() => !waitMain);

		StartCoroutine(FadeIn(fadeTime, scScript.exitButton));

		yield return null;
	} //Show only the buttons

	public IEnumerator HideButtons()
	{
		StartCoroutine(FadeOut(fadeTime, scScript.exitButton));
		waitMain = true;
		yield return new WaitUntil(() => !waitMain);

		StartCoroutine(FadeOut(fadeTime, scScript.sellButton));
		waitMain = true;
		yield return new WaitUntil(() => !waitMain);

		StartCoroutine(FadeOut(fadeTime, scScript.buyButton));

		waitSub = false;

		yield return null;
	} //Hide only the buttons

	public IEnumerator ShowShopList(int panel)
	{
		int count = 0;

		activePanel = panel;
		options = 0;

		HighlightButton(buttonList[1]);

		StartCoroutine(HideButtons());
		waitSub = true;
		yield return new WaitUntil(() => !waitSub);

		StartCoroutine(FadeIn(fadeTime, firstPanel));
		waitSub = true;
		yield return new WaitUntil(() => !waitSub);

		while (count < itemList.Count) //Showing all player's items
		{
			StartCoroutine(FadeIn(fadeTime, buttonList[1][count].gameObject));
			waitSub = true;
			yield return new WaitUntil(() => !waitSub);

			count++;
		}

		yield return null;
	}   // Show buy/sell list

	public IEnumerator HideShopList()
	{
		int count = 0;

		while (count < itemList.Count) //hide all player's items
		{
			StartCoroutine(FadeOut(fadeTime, buttonList[1][count].gameObject));
			waitSub = true;
			yield return new WaitUntil(() => !waitSub);

			count++;
		}

		StartCoroutine(FadeOut(fadeTime, firstPanel));
		waitSub = true;
		yield return new WaitUntil(() => !waitSub);

		StartCoroutine(ShowButtons());
		waitSub = true;
		yield return new WaitUntil(() => !waitSub);

		buttonList.RemoveAt(buttonList.Count - 1);

		options = 0;
		activePanel = 0;

		HighlightButton(buttonList[0]);

		yield return null;
	} //hide buy/sell list

	public IEnumerator QuickUpdateList() //quick update the sell item without the need of close and load the panel again
	{
		int count = 0;

		buttonList.RemoveAt(buttonList.Count - 1);

		Destroy(firstPanel); //destroy the old panel to ajust the new size 
		Destroy(playerItems[options].gameObject); //"Sell" player item

		SellList(); //recalculate the size of the panel and items quantity

		HighlightButton(buttonList[1]);

		StartCoroutine(FadeIn(fadeTime, firstPanel));
		waitSub = true;
		yield return new WaitUntil(() => !waitSub);

		while (count < itemList.Count)
		{
			StartCoroutine(FadeIn(fadeTime, buttonList[1][count].gameObject));
			waitSub = true;
			yield return new WaitUntil(() => !waitSub);

			count++;
		}

		yield return null;
	}

	public void SellList() // Calculate all the sell item
	{
		firstPanel = Instantiate(scScript.shopPanel); //Instantiate the background panel
		firstPanel.transform.SetParent(scScript.transform);//Set the parent with the main canvas

		itemList.Clear();//clear the item list

		Vector2 panelZeroPos = new Vector2(100 , -100); //This is the position of the panel when there's no item, this is for refence, so the panel will always in the same location with diferent sizes

		float offset = 0.5f; //Offset value, how much the panel need to move for each pixel size
		float padding = 10; //A padding to have a space between the items

		player = GameObject.FindWithTag("Player");//Fingind the player
		pcScript = player.GetComponent<Player_Control>();//Get player script
		playerItems = pcScript.clothes.GetComponentsInChildren<SpriteRenderer>();//Get player items

		foreach (SpriteRenderer go in playerItems) //creating the button for each item
		{
			GameObject sellButton = Instantiate(scScript.itemButtonTemplate);//Instantiate the button
			RectTransform rtSellButton = sellButton.GetComponent<RectTransform>();//Get the RectTransform of the button
			sellButton.transform.SetParent(firstPanel.transform);//Set parent with the background to be easy to allign the buttons
			Image imageButton = sellButton.transform.GetChild(0).GetComponent<Image>();//Get the image of the button
			imageButton.sprite = go.sprite;//Change the image of the button to the player item
			imageButton.color = go.color;//Change the color
			sellButton.tag = "Sell_Panel";//Add Tag to find it later
			rtSellButton.anchoredPosition = new Vector3(0, 0, 0);//Set the position to zero

			itemList.Add(sellButton.GetComponent<Button>());//add the button to the list
		}

		GameObject exitButton = Instantiate(scScript.buttonTemplate);//Same process, but for the Exit button
		RectTransform rtExitButton = exitButton.GetComponent<RectTransform>();
		exitButton.transform.SetParent(firstPanel.transform);
		exitButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Exit";

		itemList.Add(exitButton.GetComponent<Button>());
		
		RectTransform panelTransorm = firstPanel.GetComponent<RectTransform>();//Get the background panel RectTransform

		float rectY = scScript.itemButtonTemplate.GetComponent<RectTransform>().rect.height;//Get the height of the button

		float sizeX = rtExitButton.rect.width + (padding);//Calculating the X size of the Panel
		float sizeY = (rectY * (itemList.Count)) + (padding);//Y Size of the Panel

		float posX = (sizeX * offset) + (panelZeroPos.x / 2);//X Position of the Panel
		float posY = (sizeY * -offset) + (panelZeroPos.y / 2);//Y Position of the Panel

		Vector2 panelSize = new Vector2(sizeX, sizeY);//
		Vector2 panelPos = new Vector2(posX, posY);   //
													  //
		panelTransorm.anchoredPosition = panelPos;    //
		panelTransorm.sizeDelta = panelSize;          //Set the posistion and size

		for (int i = 0; i < itemList.Count; i++)//Calculating the Position of each button
		{
			itemList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0,padding + (sizeY / 2) - ((i + 1) * (sizeY / (itemList.Count))));
		}

		buttonList.Add(itemList);//Adding to the list
		HighlightButton(buttonList[1]);
	} 

	public IEnumerator FadeIn(float time, GameObject gameObject) //Fade in system
	{
		CanvasGroup cv = gameObject.GetComponent<CanvasGroup>();
		float t = 0;

		while (t < time)
		{
			t += Time.deltaTime;
			cv.alpha = Mathf.Lerp(0, 1, (t / time));
			yield return null;
		}

		waitMain = false;
		waitSub = false;

		yield return null;
	}

	public IEnumerator FadeOut(float time, GameObject gameObject)
	{
		CanvasGroup cv = gameObject.GetComponent<CanvasGroup>();
		float t = 0;

		while (t < time)
		{
			t += Time.deltaTime;
			cv.alpha = Mathf.Lerp(1, 0, (t / time));
			yield return null;
		}

		waitMain = false;
		waitSub = false;

		yield return null;
	} //Fade out system

	public IEnumerator PressTime() //The delay of the button system
	{
		float t = 0;

		pressTime = true;

		while (t < pressTimeDelay)
		{
			t += Time.deltaTime;
			yield return null;
		}

		pressTime = false;
		yield return null;
	}

	public void HighlightButton(List<Button> HighLightbuttons)//Highlight button system, button 0 is the first button in the list, 1 is the second and so on
	{
		for (int i = 0; i < HighLightbuttons.Count; i++)
		{
			if (i == options)
			{
				HighLightbuttons[i].gameObject.GetComponent<Image>().color = highlightColor;
			}
			else
			{
				HighLightbuttons[i].gameObject.GetComponent<Image>().color = normalColor;
			}
		}
	}

	public void PressLeftKey()//Navigation system
	{
		if (!pressTime)
		{
			StartCoroutine(PressTime());
			options--; //if pressed the Left key, will reduce the value "option"
			if (options < 0)
			{
				options = buttonList[activePanel].Count - 1; //if zero go to the max value
			}
			HighlightButton(buttonList[activePanel]);
		}
	}

	public void PressRightKey()//Navigation system
	{
		if (!pressTime)
		{
			StartCoroutine(PressTime());
			options++;
			if (options > buttonList[activePanel].Count - 1)
			{
				options = 0;
			}
			HighlightButton(buttonList[activePanel]);
		}
	}
}
