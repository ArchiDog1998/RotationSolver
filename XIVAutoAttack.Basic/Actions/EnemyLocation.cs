namespace XIVAutoAttack.Actions
{
    public enum EnemyLocation : byte
    {
        None,
        Back,
        Side,
        Front,
    }
    public static class EnemyLocationExtensions
    {
        public static string ToName(this EnemyLocation value)
            => value switch
            {
                EnemyLocation.None  => "无",
                EnemyLocation.Back  => "背面",
                EnemyLocation.Side  => "侧面",
                EnemyLocation.Front => "正面",
                _ => "错误",
            };
    }
}
