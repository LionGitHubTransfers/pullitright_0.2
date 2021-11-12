using com.adjust.sdk;
using GameAnalyticsSDK;
using LionStudios.Suite.Analytics;
using UnityEngine;

public class LoadLevelController : MonoBehaviour
{
    [SerializeField] private LoadLevelManager levelManager;
    [SerializeField] private string adjustToken;
    
    private void Awake()
    {
        GameAnalytics.Initialize();
        LionAnalytics.GameStart();
        InitAdjust(adjustToken);
        levelManager.LoadCurrentLevel();
    }
    
    private void InitAdjust(string adjustAppToken)
    {
        var adjustConfig = new AdjustConfig(adjustAppToken, AdjustEnvironment.Production, true);
        adjustConfig.setLogLevel(AdjustLogLevel.Info);
        adjustConfig.setSendInBackground(true);
        new GameObject("Adjust").AddComponent<Adjust>();
        Adjust.start(adjustConfig);
    }

}