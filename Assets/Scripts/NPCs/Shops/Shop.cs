using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
	GameObject player;
	Player_Control pcScript;
	public SpriteRenderer[] playerItems;

	[HideInInspector] public bool shopInteracting;

	public Canvas maincanvas;
	Canvas mc;
	public Shop_Canvas scScript;
	bool waitMain;
	bool waitSub;

	[HideInInspector] public bool pressTime;
	float pressTimeDelay = 0.25f;

	float fadeTime = 0.1f;

	[HideInInspector] public int options = 0;
	[HideInInspector] public int activePanel = 0;

	List<Button> playerItemsList = new List<Button>();
	List<Button> mainMenu = new List<Button>();

	public List<List<Button>> buttonList = new List<List<Button>>();

	Color highlightColor = new Color32(0, 103, 255, 255);
	Color normalColor = new Color32(180, 210, 255, 255);

	public void ActorBehavior()
	{
		options = 0;
		activePanel = 0;

		mc = Instantiate(maincanvas);
		mc.GetComponentsInChildren(mainMenu);
		scScript = mc.GetComponent<Shop_Canvas>();

		buttonList.Add(mainMenu);

		StartCoroutine(ShowMainMenu());		
	}

	public IEnumerator ShowMainMenu()
	{
		StartCoroutine(FadeIn(fadeTime, scScript.welcomePanel));
		waitMain = true;
		yield return new WaitUntil(() => !waitMain);

		StartCoroutine(ShowButtons());

		HighlightButton(buttonList[0]);

		yield return null;
	}

	public IEnumerator HideMainMenu()
	{
		StartCoroutine(FadeOut(fadeTime, scScript.exitButton));
		waitMain = true;
		yield return new WaitUntil(() => !waitMain);

		StartCoroutine(FadeOut(fadeTime, scScript.sellButton));
		waitMain = true;
		yield return new WaitUntil(() => !waitMain);

		StartCoroutine(FadeOut(fadeTime, scScript.buyButton));
		waitMain = true;
		yield return new WaitUntil(() => !waitMain);

		StartCoroutine(FadeOut(fadeTime, scScript.welcomePanel));
		waitMain = true;
		yield return new WaitUntil(() => !waitMain);

		Destroy(mc.gameObject);
		options = 0;
		activePanel = 0;

		Player_Control.interacting = false;

		yield return null;
	}

	public IEnumerator ShowButtons()
	{
		StartCoroutine(FadeIn(fadeTime, scScript.buyButton));
		waitMain = true;
		yield return new WaitUntil(() => !waitMain);

		StartCoroutine(FadeIn(fadeTime, scScript.sellButton));
		waitMain = true;
		yield return new WaitUntil(() => !waitMain);

		StartCoroutine(FadeIn(fadeTime, scScript.exitButton));

		yield return null;
	}

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
	}

	public IEnumerator ShowSellList()
	{
		int count = 0;

		activePanel = 1;
		options = 0;

		RefreshList();

		StartCoroutine(HideButtons());
		waitSub = true;
		yield return new WaitUntil(() => !waitSub);

		StartCoroutine(FadeIn(fadeTime, scScript.sellingPanel));
		waitSub = true;
		yield return new WaitUntil(() => !waitSub);

		while (count < playerItemsList.Count)
		{
			StartCoroutine(FadeIn(fadeTime, buttonList[1][count].gameObject));
			waitSub = true;
			yield return new WaitUntil(() => !waitSub);

			count++;
		}

		yield return null;
	}

	public IEnumerator QuickUpdateList()
	{
		int count = 0;

		buttonList.RemoveAt(buttonList.Count - 1);		

		RefreshList();

		StartCoroutine(FadeIn(fadeTime, scScript.sellingPanel));
		waitSub = true;
		yield return new WaitUntil(() => !waitSub);

		while (count < playerItemsList.Count)
		{
			StartCoroutine(FadeIn(fadeTime, buttonList[1][count].gameObject));
			waitSub = true;
			yield return new WaitUntil(() => !waitSub);

			count++;
		}		

		yield return null;
	}

	public IEnumerator HideSellList()
	{
		int count = 0;

		while (count < playerItemsList.Count)
		{
			StartCoroutine(FadeOut(fadeTime, buttonList[1][count].gameObject));
			waitSub = true;
			yield return new WaitUntil(() => !waitSub);

			count++;
		}

		StartCoroutine(FadeOut(fadeTime, scScript.sellingPanel));
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
	}

	void RefreshList()
	{
		playerItemsList.Clear();

		foreach (Transform child in scScript.sellingPanel.gameObject.transform)
		{
			Destroy(child.gameObject);
		}

		Vector2 panelZeroPos = new Vector2(100 , -100);

		float offset = 0.5f;
		float padding = 2;

		player = GameObject.FindWithTag("Player");
		pcScript = player.GetComponent<Player_Control>();
		playerItems = pcScript.clothes.GetComponentsInChildren<SpriteRenderer>();

		foreach (SpriteRenderer go in playerItems)
		{
			GameObject sellButton = Instantiate(scScript.sellButtonTemplate);
			RectTransform rtSellButton = sellButton.GetComponent<RectTransform>();
			sellButton.transform.SetParent(scScript.sellingPanel.transform);
			Image imageButton = sellButton.transform.GetChild(0).GetComponent<Image>();
			imageButton.sprite = go.sprite;
			imageButton.color = go.color;
			rtSellButton.anchoredPosition = new Vector3(0, 0, 0);

			playerItemsList.Add(sellButton.GetComponent<Button>());
		}

		GameObject exitButton = Instantiate(scScript.buttonTemplate);
		RectTransform rtExitButton = exitButton.GetComponent<RectTransform>();
		exitButton.transform.SetParent(scScript.sellingPanel.transform);
		exitButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Exit";

		playerItemsList.Add(exitButton.GetComponent<Button>());
		
		RectTransform panelTransorm = scScript.sellingPanel.GetComponent<RectTransform>();

		float rectY = scScript.sellButtonTemplate.GetComponent<RectTransform>().rect.width;

		float sizeX = rtExitButton.rect.width + (padding * 5);
		float sizeY = (rectY * (playerItemsList.Count - 1)) + ((playerItemsList.Count + 1)) + rtExitButton.rect.height;

		float posX = (sizeX * offset) + (panelZeroPos.x / 2);
		float posY = (sizeY * -offset) + (panelZeroPos.y / 2);

		Vector2 panelSize = new Vector2(sizeX, sizeY);
		Vector2 panelPos = new Vector2(posX, posY);

		panelTransorm.anchoredPosition = panelPos;
		panelTransorm.sizeDelta = panelSize;

		for (int i = 0; i < playerItemsList.Count; i++)
		{
			playerItemsList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0,(padding) + (sizeY / 2) - ((i + 1) * (sizeY / (playerItemsList.Count + 1))));
		}

		buttonList.Add(playerItemsList);
		HighlightButton(buttonList[1]);
	}

	public IEnumerator FadeIn(float time, GameObject gameObject)
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
	}

	public IEnumerator PressTime()
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

	public void HighlightButton(List<Button> HighLightbuttons)
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

	public void PressPlusKey()
	{
		if (!pressTime)
		{
			StartCoroutine(PressTime());
			options--;
			if (options < 0)
			{
				options = buttonList[activePanel].Count - 1;
			}
			HighlightButton(buttonList[activePanel]);
		}
	}

	public void PressMinusKey()
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
