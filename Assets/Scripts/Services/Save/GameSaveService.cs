
// 目前实现是每次都从本地读取或者保存到本地
// 后续如果要优化，可以在游戏开始时从本地加载一次数据
// 然后在游戏过程中维护内存中的数据，在关键节点/主动调用/游戏退出时再写入本地
// 这样可以降低IO频率
// 不过对于本项目，以及目前的情况来看是无所谓
public class GameSaveService
{
    private const string HIGH_SCORE_KEY = "HighScore";
    private const string SAVE_FILE_NAME = "2048save.json";

    public int GetHighScore()
    {
        return EasySave.Load<int>(HIGH_SCORE_KEY, 0, SAVE_FILE_NAME);
    }

    public bool TryUpdateHighScore(int score)
    {
        int highScore = GetHighScore();
        if (score > highScore)
        {
            EasySave.Save<int>(HIGH_SCORE_KEY, score, SAVE_FILE_NAME);
            return true;
        }
        return false;
    }
}
