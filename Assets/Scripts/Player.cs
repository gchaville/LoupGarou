﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;


public class Player : NetworkBehaviour {
	
	[SerializeField]
	GameObject ChatBoxPrefab = null;

	[SerializeField]
	GameObject Capsule = null;

	[SerializeField]
	Camera PlayerCamera = null;

	[SerializeField]
	public Image cursor = null;

	[SerializeField]
	TextMesh gameTag = null;

    static int nextId = 0;

	[SyncVar]
    public int id;

    [SyncVar]
    public int vote;

    [SyncVar]
    public int prevVote = -1;

    [SyncVar]
    public string pseudo;

	GameManager gameManager;

    Quaternion targetRotation;
   	
    GameObject SelectButton;
	ChatBox CurrentChat;

    Player Pla;

	SkyManager skyManager;

    [SyncVar]
    public bool yourTurn = false;
    [SyncVar]
    public bool death = false;
    [SyncVar]
    public bool timer = false;

    void Start()
    {
        if (!isLocalPlayer)
        {
            PlayerCamera.gameObject.SetActive(false);
            Capsule.GetComponent<MouseLook>().enabled = false;
        }
    }

    public override void OnStartLocalPlayer() {
        PlayerCamera.enabled = true;
        PlayerCamera.tag = "MainCamera";
		cursor.color = Color.black;

        yourTurn = false;

		CmdSetup ();

		skyManager = GameObject.Find ("SkyManager").GetComponent<SkyManager> ();

        GameObject ChatB = Instantiate (ChatBoxPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
		ChatB.transform.SetParent (PlayerCamera.transform);
		CurrentChat = ChatB.GetComponent<ChatBox> ();
		CurrentChat.setPlayer (this);


		Cursor.lockState = CursorLockMode.Locked;
	}
		
	[Command]
	void CmdSetup() {
		id = nextId++;
		gameManager = GameManager.instance;

        RpcChangeColor(Color.red);

        int x = Random.Range (0, gameManager.nom.Count);
		pseudo = gameManager.nom [x];
		gameManager.nom.RemoveAt (x);

		gameManager.AddPlayer (gameObject);
	}

	[ClientRpc]
	public void RpcChangeFireColor(GameManager.TurnIssue issue) 
	{
		if (!isLocalPlayer)
			return;
		
		skyManager.FireCamp.ChangeColor (issue);
	}

	[ClientRpc]
	public void RpcSwitchTime() 
	{
		if (!isLocalPlayer)
			return;

		StartCoroutine(skyManager.SwitchTime ());
	}

	[ClientRpc]
	public void RpcUpdateChatBPlace(string place)
	{
		if (!isLocalPlayer)
			return;

		CurrentChat.WelcomeText.text = "Bienvenue à " + place;
	}

    [ClientRpc]
    public void RpcUpdateChatBRole(string role)
    {
        if (!isLocalPlayer)
            return;
		
		CurrentChat.RoleText.GetComponent<Text>().text = role;
    }

    [ClientRpc]
    public void RpcChangeColor(Color c)
    {
        if (!isLocalPlayer)
            return;
		
		ChangeColor (c);
	}

	[ClientRpc]
	public void RpcChangeCursorColor(Color c)
	{
		if (!isLocalPlayer)
			return;

		cursor.color = c;
	}

	void ChangeColor(Color c) {
		Capsule.GetComponent<MeshRenderer> ().material.color = c;
		Capsule.transform.Find("Arm").GetComponent<MeshRenderer>().material.color = c;
	}

    [ClientRpc]
    public void RpcUpdatePosition(Vector3 pos, Quaternion rot)
    {
        if (!isLocalPlayer)
            return;

        transform.position = pos;
        transform.rotation = rot;
    }

	[ClientRpc]
	public void RpcDie(NetworkInstanceId _netId)
	{
		if (isLocalPlayer && _netId == GetComponent<NetworkIdentity> ().netId)
			skyManager.bDead = true;

		if (_netId == GetComponent<NetworkIdentity> ().netId) {
			ChangeColor (Color.black);
			gameTag.gameObject.SetActive (false);
		}
	}

    [Command]
    public void CmdSendMsg(string Message)
    {
		gameManager.MessageToPlayers(Message, false);
    }
		
	[ClientRpc]
    public void RpcAddMsg(string Message)
    {
		if (!isLocalPlayer)
			return;

		CurrentChat.ChatUpdate(Message);
        CurrentChat.gameObject.transform.GetChild(0).GetChild(0).Find("Scrollbar Vertical").GetComponent<Scrollbar>().value = 0f;
    }

    void Update()
    {
		if (gameTag.gameObject.activeInHierarchy)
			gameTag.text = pseudo;

        if (!isLocalPlayer || death)
            return;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);

        if (Input.GetKeyDown(KeyCode.T) || (Input.GetKeyDown(KeyCode.Return) && CurrentChat.TextZone.text == ""))
            CurrentChat.GetComponentInChildren<InputField>().ActivateInputField();
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CurrentChat.gameObject.transform.GetChild(0).GetChild(0).Find("Scrollbar Vertical").GetComponent<Scrollbar>().value -= 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            CurrentChat.gameObject.transform.GetChild(0).GetChild(0).Find("Scrollbar Vertical").GetComponent<Scrollbar>().value += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CurrentChat.gameObject.transform.GetChild(0).GetChild(0).Find("Scrollbar Horizontal").GetComponent<Scrollbar>().value -= 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CurrentChat.gameObject.transform.GetChild(0).GetChild(0).Find("Scrollbar Horizontal").GetComponent<Scrollbar>().value += 0.1f;
        }

        //Raycast pour savoir si on a toucher un joueur bon joueur
        if (Input.GetMouseButtonDown(1))//&& yourTurn)
        {
            RaycastHit hit;

            //Debug.DrawRay(PlayerCamera.transform.position, PlayerCamera.transform.forward, Color.blue, 10f, false);

            //Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit) && yourTurn)
            {
                /*Destroy(SelectButton);
                SelectButton = null;*/
                Pla = hit.transform.gameObject.GetComponentInParent<Player>();
                if (Pla != null)
                {
                    if (Pla.id != id)
                    {
						cursor.color = Color.red;
                        /*SelectButton = Instantiate ((GameObject)Resources.Load ("PlayerSelect"), new Vector3 (0, 0, 0), Quaternion.identity);
						SelectButton.transform.SetParent (PlayerCamera.transform);
						SelectButton.GetComponentInChildren<Text> ().text = "Player " + Pla.pseudo;
						SelectButton.GetComponentInChildren<Button> ().onClick.AddListener (selectionPlayer);*/
                        CmdVote(Pla.id, prevVote);
                        prevVote = Pla.id;
                        CmdSetSelected(Pla.gameObject);
                    }
				}
            }
        }
    }

    [Command]
    public void CmdSetSelected(GameObject g) {
        GetComponent<BaseRole>().SetSelectedPlayer(g);
    }

    [Command]
    public void CmdVote(int id, int pV)
    {
        GameManager.instance.Vote(id, pV);
    }

    [Command]
    public void CmdMsg(string msg, bool pV) {
        GameManager.instance.MessageToPlayers(msg, pV);
    }


    void selectionPlayer()
    {
        //envoie de la sélection au serveur
        Debug.Log("Player " + pseudo + " : is choosen");
        Destroy(SelectButton);
        SelectButton = null;
    }

    public IEnumerator Vote() {
        while(true) {
            yield return new WaitForSeconds(Random.Range(2.0f, 4.0f));

			GameObject playerToVote;

            do
				playerToVote = gameManager.GetPlayers()[Random.Range(0, gameManager.GetPlayers().Count)];
			while (id == playerToVote.GetComponent<Player>().ID());

            Vector3 relativePos = playerToVote.transform.position - transform.position;
            targetRotation = Quaternion.LookRotation(relativePos);
        }
    }

    public int ID() {
        return id;
    }
}
