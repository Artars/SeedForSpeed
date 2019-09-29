using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuckatiel : MonoBehaviour
{
    public Animator animator;
    public int cuckatieldId = 0;
    public SeedPlayer player;
    public Texture[] textures;
    public SkinnedMeshRenderer bodyRenderer;
    public bool isScreaming = false;

    public void SetCuckatiel(int id, SeedPlayer player)
    {
        this.player = player;
        cuckatieldId = id;

        UpdateColor();
    }

    public void SetCuckatiel(SeedPlayer player)
    {
        this.player = player;
        cuckatieldId = player.cuckatID;
        player.cuckatiel = this;

        UpdateColor();
    }

    public void UpdateColor()
    {
        bodyRenderer.material.SetTexture("_MainTex",textures[cuckatieldId]);
    }

    public void SetScream(bool scream)
    {
        isScreaming = scream;
        animator.SetBool("Scream",scream);
    }

    public void OnDestroy()
    {
        if(player != null)
        {
            player.cuckatiel = null;
        }
    }


}
