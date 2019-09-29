using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedPlayer : MonoBehaviour
{
    [System.Serializable]
    public enum PlayerActions {
        Left = 0, Right = 1, Accelerator = 2, Brake = 3, Scream = 4, None = 5, Start = 6
    }

    [System.Serializable]
    public class CurrentControls {
        public PlayerActions action;
        public bool state;
        public string inputName;

        public CurrentControls(string inputName, PlayerActions action = PlayerActions.None, bool state = false)
        {
            this.inputName = inputName;
            this.action = action;
            this.state = state;
        }
    }

    public PlayerController carController;

    public CurrentControls[] actions = new CurrentControls[]{
        new CurrentControls("Fire1"),
        new CurrentControls("Fire2")
    };

    public AudioClip[] screamClips;
    protected AudioSource audioSource;
    protected HFTInput _htfInput;
    protected SeedGamepad _seedGamepad;
    bool isScreaming = false;

    public void Awake()
    {
        _seedGamepad = GetComponent<SeedGamepad>();
        _htfInput = GetComponent<HFTInput>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Start()
    {
        _seedGamepad.OnDisconnect += OnDisconnect;
        SeedManager.instance.AddPlayer(this);
    }

    protected void OnDisconnect()
    {
        SeedManager.instance.RemovePlayer(this);
        Destroy(this);
    }

    public void SetActions(PlayerActions action0, PlayerActions action1)
    {
        actions[0].action = action0;
        actions[0].state = false;

        actions[1].action = action1;
        actions[1].state = false;

        _seedGamepad.controllerOptions.controllerType = HFTGamepad.ControllerType.c_2button;
        _seedGamepad.SetActions(actions[0].action.ToString(),actions[1].action.ToString());

        isScreaming = false;

    }

    public void SetActions(PlayerActions action0)
    {
        actions[0].action = action0;
        actions[0].state = false;

        actions[1].action = PlayerActions.None;
        actions[1].state = false;

        _seedGamepad.controllerOptions.controllerType = HFTGamepad.ControllerType.c_1button;
        _seedGamepad.SetActions(actions[0].action.ToString(),actions[1].action.ToString());
    
        isScreaming = false;

    }

    public void SetColor(Color color)
    {
        _seedGamepad.SendColor(color);
    }



    public void Update()
    {
        ProcessAction(actions[0]);
        ProcessAction(actions[1]);
        UpdateScream();
    }

    public void ProcessAction(CurrentControls action)
    {
        bool state = _htfInput.GetButton(action.inputName);

        Debug.Log(action.action.ToString() + " :" + _htfInput.GetButton(action.inputName));
        action.state = state;
        if((int) action.action < 4 && carController != null)
        {
            if(action.action == PlayerActions.Accelerator)
            {
                carController.inputReceiveAccel(state);
            }
            else if(action.action == PlayerActions.Brake)
            {
                carController.inputReceiveBrak(state);
            }
            else if(action.action == PlayerActions.Right)
            {
                carController.inputReceiveRight(state);
            }
            else if(action.action == PlayerActions.Left)
            {
                carController.inputReceiveLeft(state);
            }
        }
        else if(action.action == PlayerActions.Start && state)
        {
            SeedManager.instance.StartGame();
        }
        else if(action.action == PlayerActions.Scream)
        {
            isScreaming = state;
        }
    }

    protected void UpdateScream()
    {
        if(isScreaming)
        {
            if(!audioSource.isPlaying)
            {
                AudioClip randomClip = screamClips[Random.Range(0,screamClips.Length)];
                audioSource.PlayOneShot(randomClip);
            }
        }
    }

    
}
