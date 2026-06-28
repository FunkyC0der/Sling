namespace Sling.Infrastructure
{
  public static class UnityServicesOverrides
  {
#if UGS_ENV_PRODUCTION
    public const string Name = "production";
#elif UGS_ENV_PLAYTEST
    public const string Name = "play-test";
#else
    public const string Name = "";
#endif
  }
}
