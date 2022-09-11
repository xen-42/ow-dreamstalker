using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dreamstalker.Components;

public class DialogueRandomizer : MonoBehaviour
{
	private CharacterDialogueTree _dialogue;
	public void Awake()
	{
		_dialogue = gameObject.GetComponent<CharacterDialogueTree>();
		_dialogue.OnStartConversation += OnStartConversation;
	}

	public void OnDestroy()
	{
		if (_dialogue != null)
		{
			_dialogue.OnStartConversation -= OnStartConversation;
		}
	}

	public void OnStartConversation()
	{
		_dialogue._characterName = "";
		foreach (var node in _dialogue._mapDialogueNodes)
		{
			node.Value.Name = "";

			var dialogueKeys = TextTranslation.s_theTable.m_table.theTable.Keys.ToArray();

			int random;
			do
			{
				random = Random.Range(0, dialogueKeys.Length - 1);
			}
			// Just dropping anything with ":" because that's probably nomai text, like "COLEUS: hi im a nomai" etc 
			while (dialogueKeys[random].Contains(":"));

			Main.Log($"Saying random text {dialogueKeys[random]}");

			node.Value.DisplayTextData._listTextBlocks = new List<DialogueText.TextBlock>()
			{
				new DialogueText.TextBlock(new List<string>() { dialogueKeys[random] }, string.Empty)
			};
		}
	}
}
