using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncVariables : MonoBehaviour
{
    [Header("Network Setup")]
    public KolhozNetworkConfiguration networkConfiguration;
    [Header("Base")]
    public int index;
    public string[] variablesData;

    private void Start()
    {
        StartCoroutine(DeleteIfIsNotMoving());
    }

    void Update()
    {
        index = networkConfiguration.GetIndex(gameObject);
        if (index != -1 && networkConfiguration.players[index].playerPos != string.Empty)
            variablesData = networkConfiguration.players[index].playerPos.Split(' ');
        else
            StartCoroutine(GiveChange());
        if (variablesData.Length > 0)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(float.Parse(variablesData[0]), float.Parse(variablesData[1]), float.Parse(variablesData[2])), Time.deltaTime * 2);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, float.Parse(variablesData[4]), 0), Time.deltaTime * 2);
        }
    }

    IEnumerator GiveChange()
    {
        yield return new WaitForSeconds(5);
        if (index == -1)
            Destroy(gameObject);
    }

    IEnumerator DeleteIfIsNotMoving()
    {
        Vector3 startPos = transform.position;
        yield return new WaitForSeconds(15);
        float moving = (transform.position - startPos).magnitude / Time.deltaTime;
        if(moving <= 0.1f)
        {
            Debug.Log("Player with id: \"" + networkConfiguration.players[index].playerId + "\" was kicked because of AFK");
            StartCoroutine(networkConfiguration.DeletePlayerAt(networkConfiguration.players[index].playerId));
        }
        yield return new WaitForSeconds(1);
        StartCoroutine(DeleteIfIsNotMoving());
    }
}
