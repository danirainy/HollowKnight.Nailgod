namespace Nailgod;
internal static class EnemyHPBar
{
    [ModImportName(nameof(EnemyHPBar))]
    private static class EnemyHPBarImport
    {
        public static Action<GameObject>? RefreshHPBar;
    }
    static EnemyHPBar() => typeof(EnemyHPBarImport).ModInterop();
    public static void RefreshHPBar(this GameObject go)
    {
        EnemyHPBarImport.RefreshHPBar?.Invoke(go);
    }

}