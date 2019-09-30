using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedGamepad : HFTGamepad
{
    [System.Serializable]
    public class ButtonAction 
    {
        public string action0;
        public string action1;

        public ButtonAction(string action0, string action1)
        {
            this.action0 = action0;
            this.action1 = action1;
        }
    }

    [System.Serializable]
    public class MessageAction
    {
        public string message;

        public MessageAction(string message)
        {
            this.message = message;
        }
    }

    public void SetActions(string action0, string action1)
    {
        m_netPlayer.SendCmd("changeAction", new ButtonAction(action0,action1));
    }

    public void SendColor(Color color)
    {
        this.Color = color;
        SendColor();
    }

    public void DisplayMessage(string message)
    {
        m_netPlayer.SendCmd("displayMessage", new MessageAction(message));
    }

}
