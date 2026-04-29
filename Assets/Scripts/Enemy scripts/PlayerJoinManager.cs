using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinManager : MonoBehaviour
{
    public Sprite player1Sprite;
    public Sprite player2Sprite;
    public Transform[] spawnPoints;

    private List<PlayerInput> players = new List<PlayerInput>();

    public void OnEnable()
    {
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
        }
        
            
    }

    public void OnDisable()
    {
        if (PlayerInputManager.instance != null)
        { 
        PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
        }
    }

    private void OnPlayerJoined(PlayerInput player)
    {
        int index = players.Count;

        if (index >= 2)
        {
            Destroy(player.gameObject);
            return;
        }

        players.Add(player);

       
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = player.gameObject.AddComponent<SpriteRenderer>();

       
        if (index == 0)
            sr.sprite = player1Sprite;
        else if (index == 1)
            sr.sprite = player2Sprite;


        if (spawnPoints != null && index < spawnPoints.Length)
        {
            player.transform.position = spawnPoints[index].position;
        }

        player.gameObject.name = $"Player {index + 1}";

       
    }
}