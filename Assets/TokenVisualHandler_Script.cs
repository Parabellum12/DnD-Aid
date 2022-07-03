using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokenVisualHandler_Script : MonoBehaviour
{
    [SerializeField] RawImage TokenImage;
    [SerializeField] SpriteRenderer TokenBackground;
    [SerializeField] Canvas TokenImageCanvas;
    [SerializeField] SpriteMask spriteMask;
    TokenHandler_Script.TokenData referenceTokenData;
    float zPos;
    int sortingOrderPos;

    public void setup(TokenHandler_Script.TokenData refTDat, Vector2 StartPos, float zPos, int sortingOrderPos)
    {
        this.referenceTokenData = refTDat;
        this.sortingOrderPos = sortingOrderPos;
        setOrdering(zPos, sortingOrderPos);

        refTDat.sortingOrder = sortingOrderPos;
        refTDat.ZPos = zPos;
    }

    public void setup(TokenHandler_Script.TokenData referenceTokenData)
    {
        this.referenceTokenData = referenceTokenData;

        SetImage(referenceTokenData.TokenImage);
        SetPosition(referenceTokenData.TokenPosition);
        setOrdering(referenceTokenData.ZPos, referenceTokenData.sortingOrder);
    }

    public void setOrdering(float zPos, int sortingOrder)
    {
        this.zPos = zPos;

        TokenBackground.sortingOrder = sortingOrderPos;
        TokenImageCanvas.sortingOrder = sortingOrderPos;
        spriteMask.frontSortingOrder = sortingOrderPos;
        spriteMask.backSortingOrder = sortingOrderPos;
    }

    public void SetPosition(Vector2 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, zPos);
        referenceTokenData.TokenPosition = pos;
    }

    public void SetImage(Texture TokenImage)
    {
        this.TokenImage.texture = TokenImage;
        referenceTokenData.TokenImage = TokenImage;
    }
}
