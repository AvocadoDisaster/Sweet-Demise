using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private HashSet<Keyboard> JoinedKeyboards = new HashSet<Keyboard>();
   
    [SerializeField] private GameObject Playerprefab;
    [SerializeField] private Transform[] spawnpoints;
    private bool Keyboardjoined = false;




    // Update is called once per frame
    void Update()
    {
        Keyboardjoined = JoinedKeyboards.Count > 0;
        if (Keyboardjoined)
        {
            if (!Keyboardjoined && Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                if (JoinedKeyboards.Count < 2)
                {
                    if (!Keyboardjoined)
                    {
                        PlayerInput.Instantiate(Playerprefab, pairWithDevice: Keyboard.current);
                    }
                    else if (JoinedKeyboards.Count == 1)
                    {
                        PlayerInput.Instantiate(Playerprefab, pairWithDevice: Keyboard.current);
                    }

                    Keyboardjoined = true;
                }
            }
        }

        foreach (var keyboard in JoinedKeyboards )
        {
            if (keyboard.backspaceKey.wasPressedThisFrame && !JoinedKeyboards.Contains(keyboard) && JoinedKeyboards.Count < 1)
            {

                PlayerInput.Instantiate(Playerprefab, controlScheme: "Keyboard", pairWithDevice: keyboard);
                JoinedKeyboards.Add(keyboard);
                Keyboardjoined = true;

            }
            if (keyboard.backspaceKey.wasPressedThisFrame && !JoinedKeyboards.Contains(keyboard) && JoinedKeyboards.Count == 1)
            {

                PlayerInput playerInput = PlayerInput.Instantiate(Playerprefab, controlScheme: "Keyboard1", pairWithDevice: keyboard);
                JoinedKeyboards.Add(keyboard);
                Keyboardjoined = true;

            }

        }

    }
}
