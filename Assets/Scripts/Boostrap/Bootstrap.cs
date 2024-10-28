using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameReferences gameReferences;
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
        
        var heroView = Instantiate(gameReferences.HeroView);
        var gameCamera = Instantiate(gameReferences.GameCamera);
        Instantiate(gameReferences.Cursor);
        
        _ = new GameManager(heroView, gameCamera);
    }
}
