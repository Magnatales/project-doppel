using Character;
using Com.LuisPedroFonseca.ProCamera2D;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager
{
    private readonly Hero _character;
    private readonly Camera _gameCamera;
    private readonly InputCamera _inputCamera;
    
    public GameManager(IHeroView heroView, Camera gameCamera)
    {
        _character = new Hero(heroView, gameCamera);
        
        var proCamera2D = gameCamera.GetComponent<ProCamera2D>();
        proCamera2D.CameraTargets.Clear();
        proCamera2D.CameraTargets.Add(new CameraTarget(){TargetTransform = heroView.Transform});
        _inputCamera = new InputCamera(gameCamera);
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
