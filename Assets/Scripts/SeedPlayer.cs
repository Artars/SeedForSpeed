using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedPlayer : MonoBehaviour
{
    [System.Serializable]
    public enum PlayerActions {
        Left = 0, Right = 1, Accelerator = 2, Breake = 3, Scream = 4, None = 5, Start = 6
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
        new CurrentControls("Jump")
    };


    protected HFTInput _htfInput;
    protected SeedGamepad _seedGamepad;

    public void Awake()
    {
        _seedGamepad = GetComponent<SeedGamepad>();
        _htfInput = GetComponent<HFTInput>();
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
    }

    public void SetActions(PlayerActions action0)
    {
        actions[0].action = action0;
        actions[0].state = false;

        actions[1].action = PlayerActions.None;
        actions[1].state = false;

        _seedGamepad.controllerOptions.controllerType = HFTGamepad.ControllerType.c_1button;
        _seedGamepad.SetActions(actions[0].action.ToString(),actions[1].action.ToString());
    }

    public void SetColor(Color color)
    {
        _seedGamepad.SendColor(color);
    }



    public void Update()
    {
        ProcessAction(actions[0]);
        ProcessAction(actions[1]);
    }

    public void ProcessAction(CurrentControls action)
    {
        bool state = _htfInput.GetKey(action.inputName);
        action.state = state;
        if((int) action.action < 4 && carController != null)
        {
            
        }
        else if(action.action == PlayerActions.Start)
        {
            SeedManager.instance.StartGame();
        }
    }
}
