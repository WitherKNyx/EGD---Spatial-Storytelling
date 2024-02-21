using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime;

public class DialogueManager : MonoBehaviour
{
	#region Static Variables
	[HideInInspector]
	public static DialogueManager instance;

	/// <summary>
	/// This variable is used to block interactions while dialogue is occuring
	/// </summary>
	[HideInInspector]
	public static bool inDialogue = false;
	#endregion

	#region References
	[SerializeField]
	private GameObject _dialogueBox, _dialogueChoices;

	[SerializeField]
	private Image _dialoguePortrait, _continueIndicator;

	[SerializeField]
	private TextMeshProUGUI _nameTextBox, _dialogueTextBox;

	[SerializeField]
	private TextMeshProUGUI[] _dialogueChoiceTextBoxes;

	[SerializeField]
	private AudioSource _talkingSound, _clickSound;
	#endregion

	[SerializeField, Min(0f), Tooltip("Time interval between each character appearing in the textbox.")]
	private float _timeInterval;

	[SerializeField]
	Dialogue test;

	private Coroutine _currentLine;

	private string _text;
	private int _choice = -1;

	#region System Functions
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			SceneManager.sceneLoaded += OnSceneChange;
		}
		else
			Destroy(gameObject);
	}

	private void Start()
	{
		StartCoroutine(PlayDialogue(test));
	}

	private void Update()
	{
		if (GameManager.Instance.IsPaused) return;
		if (Input.GetMouseButtonDown(0) && _currentLine != null)
		{
			StopCoroutine(_currentLine);
			_currentLine = null;
		}
	}
	#endregion

	public void SetChoice(int i) => _choice = i;

	private void OnSceneChange(Scene scene, LoadSceneMode mode)
	{
		string currentScene = scene.name;
		Dialogue openingDialogue = null;

		
		if (openingDialogue != null)
		{
			StartCoroutine(PlayDialogue(openingDialogue));
		}
	}

	public void StartNewDialogue(Dialogue dialogue)
	{
		StopAllCoroutines();
		_currentLine = null;
		inDialogue = false;
		StartCoroutine(PlayDialogue(dialogue));
	}

	/// <summary>
	/// Function to play a give Dialogue object
	/// </summary>
	/// <param name="dialogue"></param>
	public IEnumerator PlayDialogue(Dialogue dialogue)
	{
		Debug.Log(dialogue.name);
		inDialogue = true;
		_dialogueBox.SetActive(true);
		string filepath = string.Format("Dialogue/{0}", dialogue.fileName);
		// Split dialogue into lines for parsing
		
		string[] lines = Resources.Load<TextAsset>(filepath).text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < lines.Length; i++)
		{
			// Tag beginning a line indicates special behavior
			if (lines[i][0] == '<')
			{
				int start, end;

				start = lines[i].IndexOf("<end>");
				if (start != -1)
				{
					break;
				}

				// Speaker tag indicates a different character
				start = lines[i].IndexOf("speaker=\"");
				if (start != -1)
				{
					start += 9;
					end = lines[i].IndexOf('"', start);
					_nameTextBox.text = lines[i][start..end];
				}

				// Sprite tag indicates a different sprite
				start = lines[i].IndexOf("sprite=\"");
				if (start != -1)
				{
					start += 8;
					end = lines[i].IndexOf('"', start);
					_dialoguePortrait.sprite = Resources.Load<Sprite>(
						string.Format("Sprites/{0}_{1}", _nameTextBox.text, lines[i][start..end])
					);
				}

				
				start = lines[i].IndexOf("jump=\"");
				if (start != -1) {
					start+=6;
					end = lines[i].IndexOf('"', start);
					string jumpLabel = lines[i][start..end];
					string label = "<label=\"" + jumpLabel + "\">";
					for (int j = 0; j < lines.Length; j++)
					{
						start = lines[j].IndexOf(label);
						if (start != -1)
						{
							i = j;
							_choice = -1;
							_clickSound.Play();
							break;
						}
					}
				}

				start = lines[i].IndexOf("event=\"");
				if (start != -1)
				{
					start += 7;
					end = lines[i].IndexOf('"', start);
					string funcEvent = lines[i][start..end];
					string arg = "";
					start = lines[i].IndexOf("arg=\"");
					if (start != -1)
					{
						start += 5;
						end = lines[i].IndexOf('"', start);
						arg = lines[i][start..end];
					}
					if (funcEvent == "LoadScene")
					{
						inDialogue = false;
						GameManager.LoadScene(arg);
					}
				}

				start = lines[i].IndexOf("<choice>");
				if (start != -1)
				{
					_dialogueTextBox.text = "";
					++i;
					// Obtain choices and jump Labels
					List<string> labels = new();
					List<string> choices = new();
					while (lines[i].IndexOf("</choice>") != 0)
					{
						start = lines[i].IndexOf("<") + 1;
						end = lines[i].IndexOf(">");
						labels.Add(lines[i][start..end]);
						choices.Add(lines[i][(end + 1)..]);
						++i;
					}

					for (int j = 0; j < Mathf.Min(choices.Count, _dialogueChoiceTextBoxes.Length); j++)
					{
						_dialogueChoiceTextBoxes[j].transform.parent.gameObject.SetActive(true);
						_dialogueChoiceTextBoxes[j].text = choices[j];
					}
					for (int j = choices.Count; j < _dialogueChoiceTextBoxes.Length; j++)
						_dialogueChoiceTextBoxes[j].transform.parent.gameObject.SetActive(false);
					_dialogueChoices.SetActive(true);
					yield return new WaitUntil(() => _choice != -1);
					_dialogueChoices.SetActive(false);
					string label = "<label=\"" + labels[_choice] + "\">";
					for (int j = 0; j < lines.Length; j++)
					{
						start = lines[j].IndexOf(label);
						if (start != -1)
						{
							i = j;
							_choice = -1;
							_clickSound.Play();
							break;
						}
					}
				}

				continue;
			}

			// Wait until the current line is processed before moving on
			_currentLine = StartCoroutine(ProcessLine(lines[i]));
			yield return new WaitWhile(() => _currentLine != null);
			_continueIndicator.enabled = true;
			// This yield is here to prevent accidentally reading the same mouse
			// press to skip dialogue as the mouse press to advance to the next dialogue
			yield return null;

			// Ensure the dialogue box contains all text, then wait for user input
			_text = lines[i];

			// Convert any escape characters necessary
			_text = _text.Replace("\\<", "<");
			_text = _text.Replace("\\n", "\n");
			_text = _text.Replace("\\t", "\t");
			_text = _text.Replace("\\\\", "\\");
			_dialogueTextBox.text = _text;
			while (true)
			{
				if (Input.GetMouseButtonDown(0))
					break;
				yield return null;
			}
			_clickSound.Play();
			_continueIndicator.enabled = false;
		}

		_dialogueBox.SetActive(false);
		inDialogue = false;

		if (dialogue.nextDialogue != null)
		{
			yield return new WaitForSeconds(1.5f);
			StartCoroutine(PlayDialogue(dialogue.nextDialogue));
		}
	}

	public IEnumerator ProcessLine(string line)
	{
		// change to use _text and update to _dialogueTextbox.text in update
		yield return new WaitForSeconds(_timeInterval);
		_dialogueTextBox.text = line[0].ToString();
		for (int j = 1; j < line.Length; j++)
		{
			// If the next word would be too long for the textbox, add a newline
			if (line[j - 1] == ' ')
			{
				int nextSpace = line.IndexOf(' ', j);
				nextSpace = nextSpace == -1 ? line.Length : nextSpace;
				string nextWord = line[j..nextSpace];
				string currText = _dialogueTextBox.text;
				_dialogueTextBox.text += nextWord;
				float modifiedTextWidth = _dialogueTextBox.preferredWidth;
				float textBoxWidth = _dialogueTextBox.rectTransform.rect.width;
				if (modifiedTextWidth > textBoxWidth) currText += '\n';
				_dialogueTextBox.text = currText;
			}

			// Assign extra pauses to special characters
			int multiplier = 1;
			if (char.IsWhiteSpace(line[j - 1]))
				multiplier = 2;
			else if (char.IsPunctuation(line[j - 1]) && line[j -1 ] != '\'' && line[j - 1] != '"')
				multiplier = 4;
			else
				_talkingSound.Play();

			yield return new WaitForSeconds(multiplier * _timeInterval);
			char letter = line[j];
			if (letter == '\\')
			{
				switch (line[j + 1])
				{
					case '<':
						letter = '<'; break;
					case 'n':
						letter = '\n'; break;
					case 't':
						letter = '\t'; break;
					case '\\':
						letter = '\\'; break;
				}
				_dialogueTextBox.text += letter;
				++j;
			} 
			else if (line[j] == '<')
			{
				string block = "";
				if (line[j + 1] == 'i' || line[j + 1] == 'b')
				{
					block = string.Format("<{0}>", line[j + 1]);
					j += 2;
				} else if (line[j + 1] == '/' && (line[j + 2] == 'i' || line[j + 2] == 'b'))
				{
					block = string.Format("</{0}>", line[j + 2]);
					j += 3;
				}
				_dialogueTextBox.text += block;
			} else
			{
				_dialogueTextBox.text += line[j];
			}
		}
		_currentLine = null;
	}
}
