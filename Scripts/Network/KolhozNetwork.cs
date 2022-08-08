using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class KolhozNetwork : MonoBehaviour
{
    [Header("Setup")]
    public KolhozNetworkConfiguration nc;

    int p;
    GameObject player;

    public void SetID()
    {
        nc.playerId = UnityEngine.Random.Range(0, 9999999).ToString();
    }

    public void SetName()
    {
        nc.playerName = "Player " + UnityEngine.Random.Range(0, 9999999).ToString();
    }

    public void Start()
    {
        SetID();
        SetName();
        nc.players.Clear();
        //nc.playersObjects.Clear();
        if (nc.serverUrl[nc.serverUrl.Length - 1] == '/')
            nc.serverUrl = nc.serverUrl.Remove(nc.serverUrl.Length - 1, 1);
        StartCoroutine(AddPlayer());
        StartCoroutine(GetNetworkData());
        Debug.Log("Trying to connect to the server...");
    }

    IEnumerator AddPlayer()
    {
        WWWForm form = new WWWForm();
        form.AddField("id", nc.playerId);
        form.AddField("name", nc.playerName);

        using (UnityWebRequest www = UnityWebRequest.Post(nc.serverUrl + "/AddPlayer.php", form))
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

    IEnumerator DeletePlayer()
    {
        WWWForm form = new WWWForm();
        form.AddField("id", nc.playerId);

        using (UnityWebRequest www = UnityWebRequest.Post(nc.serverUrl + "/DeletePlayer.php", form))
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

    IEnumerator GetNetworkData()
    {
        WWWForm form = new WWWForm();
        form.AddField("pass", nc.serverPassword);

        using (UnityWebRequest www = UnityWebRequest.Post(nc.serverUrl + "/GetData.php", form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string itemsDataString = www.downloadHandler.text;
                string[] splitType = new string[] { "</NetSplit>" };
                nc.networkData = itemsDataString.Split(splitType, StringSplitOptions.None);
                Array.Resize(ref nc.networkData, nc.networkData.Length - 1);
                p = 0;
                if (nc.players.Count != nc.networkData.Length)
                    nc.players.Clear();
                while (nc.networkData.Length > p)
                {
                    yield return StartCoroutine(AddPlayersList());

                    nc.players[p].playerPos = getBetween(nc.networkData[p], "<position>", "</position>");

                    p++;
                }
            }
        }

        if(nc.playerObject != null)
            StartCoroutine(UpdatePos(nc.playerObject.transform.position.x + " " + nc.playerObject.transform.position.y + " " + nc.playerObject.transform.position.z + " " + 0 + " " + nc.playerObject.transform.eulerAngles.y + " " + 0));

        yield return new WaitForSeconds(nc.updateSpeed);
        StartCoroutine(GetNetworkData());
    }

    IEnumerator AddPlayersList()
    {
        if (nc.networkData.Length > nc.players.Count)
        {
            nc.players.Add(new KolhozNetworkConfiguration.Players { playerId = getBetween(nc.networkData[p], "<id>", "</id>"), playerName = getBetween(nc.networkData[p], "<name>", "</name>"), playerPos = getBetween(nc.networkData[p], "<position>", "</position>")});
            if (GameObject.Find(nc.players[p].playerId) == null)
            {
                player = Instantiate(nc.playerPrefab);
                nc.players[p].playerObject = player;
                player.name = nc.players[p].playerId;
                if (player.name == nc.playerId)
                    nc.playerObject = player;
            }
            else
            {
                nc.players[p].playerObject = GameObject.Find(nc.players[p].playerId);
            }
        }
        yield return null;
    }

    public IEnumerator UpdatePos(string playerPos)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", nc.playerId);
        form.AddField("pos", playerPos);

        using (UnityWebRequest www = UnityWebRequest.Post(nc.serverUrl + "/UpdatePos.php", form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
        }
    }

    private static string getBetween(string strSource, string strStart, string strEnd)
    {
        if (strSource.Contains(strStart) && strSource.Contains(strEnd))
        {
            int Start, End;
            Start = strSource.IndexOf(strStart, 0) + strStart.Length;
            End = strSource.IndexOf(strEnd, Start);
            return strSource.Substring(Start, End - Start);
        }

        return "";
    }

    private void OnApplicationQuit()
    {
        StartCoroutine(DeletePlayer());
    }

    void FixedUpdate()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += StateChange;
#endif
    }
#if UNITY_EDITOR
    void StateChange(PlayModeStateChange state)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode && state.ToString() == "ExitingPlayMode")
        {
            StartCoroutine(DeletePlayer());
        }
     }
#endif
}
