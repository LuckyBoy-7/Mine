namespace Lucky.Managers
{
    public partial class EventManager
    {
        public static readonly string FightStart = "FightStart";
        public static readonly string EnemyDie = "EnemyDie";
        public static readonly string PlayerDie = "PlayerDie";
        public static readonly string PlayerTurnEnd = "PlayerTurnEnd";
        public static readonly string PlayerTurnStart = "PlayerTurnStart";
        public static readonly string AllEnemiesDead = "AllEnemiesDead"; // 不一定赢了，可能同归于尽
        public static readonly string PlayerWin = "PlayerWin";
        public static readonly string PotionUsed = "PotionUsed";
    }
}