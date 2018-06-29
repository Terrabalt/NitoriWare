﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoomGame_Player : MonoBehaviour
{
    [SerializeField]
    public static int hp = 1;
    [HideInInspector]
    public static float bloodfx = 0;
    [SerializeField]
    Material blit;

    void Start()
    {
        bloodfx = 0;
        hp = 1;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, Input.GetAxis("Mouse X"));
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit, 100f, 1 << LayerMask.NameToLayer("MicrogameLayer1")))
                hit.collider.GetComponent<DoomGame_Enemy>().DamageSelf();
        }
        CheckEnemies();
    }

    void CheckEnemies()
    {
        DoomGame_UI.rightArrow = DoomGame_UI.leftArrow = false;
        for(int i = 0; i < DoomGame_Enemy.enemies.Count; i++)
        {
            Vector3 vec = Camera.main.WorldToViewportPoint(
                DoomGame_Enemy.enemies[i].transform.position);
            if(vec.z < Camera.main.nearClipPlane)
            {
                if(vec.x > 0.5f)
                    DoomGame_UI.leftArrow = true;
                else
                    DoomGame_UI.rightArrow = true;
            }
            else
            {
                if(vec.x < 0.25f)
                    DoomGame_UI.leftArrow = true;
                if(vec.x > 0.75f)
                    DoomGame_UI.rightArrow = true;
            }
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        blit.SetFloat("_Amount", bloodfx -= Time.deltaTime);
        if(bloodfx < 0)
            bloodfx = 0;
        Graphics.Blit(source, destination, blit);
    }
}
