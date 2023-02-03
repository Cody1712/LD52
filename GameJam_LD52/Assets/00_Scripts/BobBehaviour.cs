using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class BobBehaviour : MonoBehaviour
{
	[Header("Dialogue")]
    [SerializeField] DialogueData dialogueData;
    [SerializeField] GameObject dialogueCanvas;
    [SerializeField] GameObject dialogueCamera;
    [SerializeField] Transform lookAtTarget;
    [SerializeField] float heightOffset;
    [SerializeField] TextMeshProUGUI tmpDialogueText;
    [SerializeField] float dialogueSpeed;
    private string activeDialogue;

    [Header("Spawn Nest")]
    [SerializeField] GameObject nest;
    [SerializeField] GameObject puffParticleSystem;


    // Start is called before the first frame update
    void Start()
    {
        activeDialogue = dialogueData.dialogueOption[0];
        GameManager.Instance.allTheGoldCollected += AllGoldCollected;
    }

    // Update is called once per frame
    void Update()
    {
		
    }

    private void AllGoldCollected()
    {
        puffParticleSystem.SetActive(true);
        puffParticleSystem.GetComponent<ParticleSystem>().Play();
        nest.SetActive(true);
        this.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        GameManager.Instance.allTheGoldCollected -= AllGoldCollected;
    }

    public void StartInteraction(Vector3 playerPos)
	{
        Vector3 heightOffsetVector = new Vector3(0,heightOffset,0);
        lookAtTarget.position = ((this.transform.position + heightOffsetVector) + (playerPos + heightOffsetVector)) / 2;
        GameManager.Instance.gameState = GameManager.GameState.DIALOGUE;
        ObjectPooler.Instance.SpawnFromPool("Dialogue_Sound",Vector3.zero, null, Quaternion.identity);
        HandleDialogue();
    }

    public void EndInteraction()
	{
		
        // dialogue has ended
        GameManager.Instance.gameState = GameManager.GameState.RUNNING;
        tmpDialogueText.text = "";
        dialogueCanvas.SetActive(false);
        dialogueCamera.SetActive(false);
    }

    void HandleDialogue()
	{
        dialogueCanvas.SetActive(true);
        dialogueCamera.SetActive(true);
        tmpDialogueText.text = "";
        StartCoroutine(ReadDialogue());
	}

    IEnumerator ReadDialogue()
	{
        foreach(char dialogueLetter in activeDialogue.ToCharArray())
		{
            tmpDialogueText.text += dialogueLetter;
            yield return new WaitForSeconds(dialogueSpeed);
        }

        TakeGold();
    }

    void TakeGold()
	{
        GameManager.Instance.SpendGold();
	}
}
