using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Data", menuName = "DialogueData")]
public class DialogueData : ScriptableObject
{
	[TextArea(8, 8)]
	public List<string> dialogueOption;
}
