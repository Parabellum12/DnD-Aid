using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokenHandler_Script : MonoBehaviour
{
    [SerializeField] Image TokenPfp;

    public void setTokenPFP(Texture2D tex)
    {
        TokenPfp.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2)); ;
    }
}
