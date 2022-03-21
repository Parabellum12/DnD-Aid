using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenHandler : MonoBehaviour
{

    public void toMapCreator()
    {
        SceneManager.LoadScene("MapCreator", LoadSceneMode.Single);
    }

    public void toRoomConnect()
    {

    }
}
