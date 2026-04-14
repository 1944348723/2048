using UnityEngine;

internal enum SaveLocation { File, PlayerPrefs };

internal class EasySaveSettings : MonoBehaviour
{
    [SerializeField] private SaveLocation location;

    private void Awake()
    {
        EasySave.Init(location);
    }
}