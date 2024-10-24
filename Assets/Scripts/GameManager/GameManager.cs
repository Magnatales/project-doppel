using Character;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager
{
    private readonly Hero _character;
    private readonly Camera _gameCamera;
    
    public GameManager(HeroView heroView, Camera gameCamera)
    {
        _gameCamera = Object.Instantiate(gameCamera);
        _character = new Hero(Object.Instantiate(heroView), _gameCamera);
        GameplayLoopAsync().Forget();
    }

    private async UniTaskVoid GameplayLoopAsync()
    {
        while (Application.isPlaying)
        {
            _character.Update();
            await UniTask.Yield();
        }
    }
}
