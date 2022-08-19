using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
	public Canvas maincanvas;
	Canvas mc;
	bool wait;

	[HideInInspector] public bool pressTime;
	float pressTimeDelay = 0.25f;

	float fadeTime = 0.1f;

	[HideInInspector] public int options = 0;
	[HideInInspector] public int panel = 0;
	[HideInInspector] public Button[] buttons;
	Color highlightColor = new Color32(0, 103, 255, 255);
	Color normalColor = new Color32(180, 210, 255, 255);

	public void ActorBehavior()
	{
		mc = Instantiate(maincanvas);
		buttons = mc.GetComponentsInChildren<Button>();

		StartCoroutine(ShowMainMenu());		
	}

	public IEnumerator ShowMainMenu()
	{
		Shop_Canvas scScript = mc.GetComponent<Shop_Canvas>();

		StartCoroutine(FadeIn(fadeTime, scScript.welcomePanel));
		wait = true;
		yield return new WaitUntil(() => !wait);

		StartCoroutine(FadeIn(fadeTime, scScript.buyButton));
		wait = true;
		yield return new WaitUntil(() => !wait);

		StartCoroutine(FadeIn(fadeTime, scScript.sellButton));
		wait = true;
		yield return new WaitUntil(() => !wait);

		StartCoroutine(FadeIn(fadeTime, scScript.exitButton));

		HighlightButton();

		yield return null;
	}

	public IEnumerator HideMainMenu()
	{
		Shop_Canvas scScript = mc.GetComponent<Shop_Canvas>();

		StartCoroutine(Fadeout(fadeTime, scScript.exitButton));		
		wait = true;
		yield return new WaitUntil(() => !wait);

		StartCoroutine(Fadeout(fadeTime, scScript.sellButton));		
		wait = true;
		yield return new WaitUntil(() => !wait);

		StartCoroutine(Fadeout(fadeTime, scScript.buyButton));
		wait = true;
		yield return new WaitUntil(() => !wait);

		StartCoroutine(Fadeout(fadeTime, scScript.welcomePanel));
		wait = true;
		yield return new WaitUntil(() => !wait);

		Destroy(mc.gameObject);
		options = 0;
		panel = 0;

		Player_Control.interacting = false;

		yield return null;
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

		wait = false;

		yield return null;
	}

	public IEnumerator Fadeout(float time, GameObject gameObject)
	{
		CanvasGroup cv = gameObject.GetComponent<CanvasGroup>();
		float t = 0;

		while (t < time)
		{
			t += Time.deltaTime;
			cv.alpha = Mathf.Lerp(1, 0, (t / time));
			yield return null;
		}

		wait = false;

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

	public void HighlightButton()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			if (i == options)
			{
				buttons[i].gameObject.GetComponent<Image>().color = highlightColor;
			}
			else
			{
				buttons[i].gameObject.GetComponent<Image>().color = normalColor;
			}
		}
	}

	public void PressLeftKey()
	{
		if (!pressTime)
		{
			StartCoroutine(PressTime());
			options--;
			if (options < 0)
			{
				options = buttons.Length - 1;
			}
			HighlightButton();
		}
	}

	public void PressRightKey()
	{
		if (!pressTime)
		{
			StartCoroutine(PressTime());
			options++;
			if (options > buttons.Length - 1)
			{
				options = 0;
			}
			HighlightButton();
		}
	}
}
