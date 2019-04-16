using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
	void Update ()
	{
		if( Input.GetKeyDown(KeyCode.R) )
		{
            //Scene scene = Application.loadlevel.GetActiveScene();

            //			SceneManager.LoadScene("scene", LoadSceneMode.Additive);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}