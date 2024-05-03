using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel : MonoBehaviour
{
    // Start is called before the first frame update
    public string nextScreen;
   
    public void Exit(){
        SceneManager.LoadScene(nextScreen);
    }
}
