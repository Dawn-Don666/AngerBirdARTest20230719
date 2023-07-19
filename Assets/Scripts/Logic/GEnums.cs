
/// <summary>
/// 弹弓状态
/// </summary>
public enum SlingshotState
{
    //默认状态
    NORMAL,
    //闲置状态
    IDLE,
    //玩家拉动
    USER_PULLING,
    //发射
    BIRD_FLYING
}

/// <summary>
/// 游戏状态
/// </summary>
public enum GameState
{
    //开始
    START,
    //鸟移动到弹弓
    BIRD_MOVING_TO_SLINGSHOT,
    //游戏中
    PLAYING,
    //赢了
    WON,
    //输了
    LOST
}

/// <summary>
/// 小鸟的状态
/// </summary>
public enum BirdState
{
    //抛出前
    BEFORE_THROWN,
    //抛出
    THROWN,
    //运动中
    MOVE,
    //停止运动
    MOVING_STOP

}
