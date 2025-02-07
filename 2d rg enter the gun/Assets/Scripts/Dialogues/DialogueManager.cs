﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
	public bool isDialogueRunning;
	//public Text nameText;
	public Text dialogueText;

	public Animator animator;

	[HideInInspector]
	public Queue<string> sentences;

	// Use this for initialization
	void Start()
	{
		sentences = new Queue<string>();
	}

	public void StartDialogue(Dialogue dialogue)
	{
		isDialogueRunning = true;

		animator.SetBool("IsOpen", true);

		//nameText.text = dialogue.name;

		sentences.Clear();

		foreach (string sentence in dialogue.sentences)
		{
			sentences.Enqueue(sentence);
		}

		DisplayNextSentence();
	}

	public void DisplayNextSentence()
	{
		if (sentences.Count == 0)
		{
			EndDialogue();
			return;
		}

		string sentence = sentences.Dequeue();
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
	}

	IEnumerator TypeSentence(string sentence)
	{
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return null;
		}
	}

	public void EndDialogue()
	{
		isDialogueRunning = false;
		animator.SetBool("IsOpen", false);
	}

}