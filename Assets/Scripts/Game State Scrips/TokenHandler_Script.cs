using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenHandler_Script : MonoBehaviour
{
    [SerializeField] bool NetworkMode = false;
    [SerializeField] GameObject TokenVisualPrefab;
    List<TokenData> Tokens = new List<TokenData>();
    long currentActiveNumber = 0;





    

    public void CreateToken(Vector2 Pos)
    {
        TokenData TDat = new TokenData(Pos, getCurrentActiveNumber());
        Tokens.Add(TDat);
        CreateToken(TDat);
    }

    public void CreateToken(TokenData TDat)
    {
        //add local/network tokenHandler
    }


    public List<TokenData> GetSaveData()
    {
        return Tokens;
    }

    public void LoadSaveData(List<TokenData> tokenData)
    {
        Tokens = tokenData;
        currentActiveNumber = 0;
        foreach (TokenData tDat in Tokens)
        {
            if (tDat.TokenIdNumber >= currentActiveNumber)
            {
                currentActiveNumber = tDat.TokenIdNumber + 1;
            }

            CreateToken(tDat);
        }
    }

    long getCurrentActiveNumber()
    {
        currentActiveNumber++;
        return currentActiveNumber - 1;
    }


    public class TokenData
    {
        public string TokenName;
        public long TokenIdNumber;
        public Texture TokenImage;

        public Vector2 TokenPosition;

        public int sortingOrder;
        public float ZPos;

        public int InitiativeNumber;

        public TokenData(Vector2 TokenPosition, long tokenNumber)
        {
            this.TokenPosition = TokenPosition;
            TokenIdNumber = tokenNumber;
        }
    }
}
