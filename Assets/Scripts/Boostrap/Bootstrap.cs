using Character;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private HeroView heroView;
    [SerializeField] private Camera gameCamera;
    private const string GAME_SCENE = "Game";
    private const string UI_SCENE = "UI";
    
    private void Awake()
    {
        LoadGame().Forget();
    }

    private async UniTaskVoid LoadGame()
    {
        await UniTask.WhenAll(
            SceneManager.LoadSceneAsync(GAME_SCENE, LoadSceneMode.Additive).ToUniTask(),
            SceneManager.LoadSceneAsync(UI_SCENE, LoadSceneMode.Additive).ToUniTask());
        
        var gameScene = SceneManager.GetSceneByName(GAME_SCENE);
        SceneManager.SetActiveScene(gameScene);
        
        var gameManager = new GameManager(heroView, gameCamera);
    }
}
