using Character;
using Com.LuisPedroFonseca.ProCamera2D;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager
{
    private readonly Hero _character;
    private readonly Camera _gameCamera;
    private readonly InputCamera _inputCamera;
    
    public GameManager(HeroView heroView, Camera gameCamera)
    {
        var characterView = Object.Instantiate(heroView);
        _gameCamera = Object.Instantiate(gameCamera);
        _character = new Hero(characterView, _gameCamera);
        _gameCamera.GetComponent<ProCamera2D>().CameraTargets.Add(new CameraTarget(){TargetTransform = characterView.transform});
        _inputCamera = new InputCamera(_gameCamera);
        GameplayLoopAsync().Forget();
    }

    private async UniTaskVoid GameplayLoopAsync()
    {
        while (Application.isPlaying)
        {
            _character.Update();
            _inputCamera.Update();
            await UniTask.Yield();
        }
    }
}
