using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "Netwok Configuration", menuName = "Network/Kolhoz Network Configuration", order = 1)]
public class KolhozNetworkConfiguration : ScriptableObject
{
    [Header("Data")]
    [Tooltip("URL of the server.")]
    public string serverUrl = "http://";
    [Tooltip("Key to access the server.")]
    public string serverPassword = "NcllaPPTPLuNKtKzPiWVtEOsF";
    [Space(5)]
    public GameObject playerPrefab;
    [Space(5)]
    [Tooltip("The speed of updating information from the server.")]
    public float updateSpeed = 0.1f;

    [Header("Player Data Debag")]
    public string playerId;
    public string playerName;
    public GameObject playerObject;

    [Header("Info")]
    public string[] networkData;
    //[SerializeField]
    //public List<GameObject> playersObjects = new List<GameObject>();
    [SerializeField]
    public List<Players> players = new List<Players>();

    public bool IsMine(GameObject player)
    {
        if (player.name == playerId)
            return true;
        else
            return false;
    }

    public int GetIndex(GameObject player)
    {
        if (players.Find(x => x.playerObject.name == player.name) != null)
            return players.FindIndex(x => x.playerObject.name == player.name);
        else
            return -1;
    }
    [Serializable]
    public class Players
    {
        public string playerName;
        public string playerId;
        public string playerPos;
        public GameObject playerObject;
    }

    public IEnumerator DeletePlayerAt(string id)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);

        using (UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/DeletePlayer.php", form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }
}