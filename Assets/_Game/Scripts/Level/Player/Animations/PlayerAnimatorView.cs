using NaughtyAttributes;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Player.Animations
{
  public enum EPlayerAnimation
  {
    None,
    Idle,
    InAir,
    WallSlide,
    LaunchWeak,
    LaunchStrong
  }

  /// <summary>Unity side of player animation: plays animator states by name. No "what/when" logic.</summary>
  public class PlayerAnimatorView : MonoBehaviour, IGameObjectView
  {
    [SerializeField, Required] private Animator _animator;

    [Header("States")]
    [SerializeField] private string _idleState = "Idle";
    [SerializeField, Min(0)] private float _idleFadeDuration;

    [SerializeField] private string _inAirState = "InAirBlendTree";
    [SerializeField, Min(0)] private float _inAirFadeDuration;

    [SerializeField] private string _wallSlideState = "WallSlide";
    [SerializeField, Min(0)] private float _wallSlideFadeDuration = 0.25f;

    [SerializeField] private string _launchWeakState = "PreLaunch-1";
    [SerializeField, Min(0)] private float _launchWeakFadeDuration;

    [SerializeField] private string _launchStrongState = "PreLaunch-2";
    [SerializeField, Min(0)] private float _launchStrongFadeDuration;

    [Header("Parameters")]
    [SerializeField]
    [AnimatorParam(nameof(_animator), AnimatorControllerParameterType.Float)]
    private int _inAirBlendFloatId;

    private int _idleHash, _inAirHash, _wallSlideHash, _launchWeakHash, _launchStrongHash;
    private EPlayerAnimation _current = EPlayerAnimation.None;

    private void Awake()
    {
      _idleHash = Animator.StringToHash(_idleState);
      _inAirHash = Animator.StringToHash(_inAirState);
      _wallSlideHash = Animator.StringToHash(_wallSlideState);
      _launchWeakHash = Animator.StringToHash(_launchWeakState);
      _launchStrongHash = Animator.StringToHash(_launchStrongState);
    }

    // Animator rebinds to its default state when the object is re-enabled (Die/Revive).
    private void OnDisable() =>
      _current = EPlayerAnimation.None;

    /// <summary>Repeated call for the same animation is a no-op, except launch clips which restart.</summary>
    public void Play(EPlayerAnimation animation)
    {
      if (animation == EPlayerAnimation.None)
        return;

      bool isLaunch = IsLaunch(animation);
      if (animation == _current && !isLaunch)
        return;

      _current = animation;

      (int hash, float fade) = Resolve(animation);
      // Fade durations are in seconds (same as the old "Fixed Duration" transitions).
      if (isLaunch)
        _animator.CrossFadeInFixedTime(hash, fade, 0, 0f);
      else
        _animator.CrossFadeInFixedTime(hash, fade);
    }

    /// <summary>True while a launch clip is still playing (it exits to InAir by itself).</summary>
    public bool IsPlayingOneShot
    {
      get
      {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        bool inLaunch = info.shortNameHash == _launchWeakHash || info.shortNameHash == _launchStrongHash;
        return (inLaunch && info.normalizedTime < 1f) || IsInTransitionToLaunch();
      }
    }

    public void SetInAirBlend01(float value) =>
      _animator.SetFloat(_inAirBlendFloatId, value);

    private bool IsInTransitionToLaunch()
    {
      if (!_animator.IsInTransition(0))
        return false;

      int next = _animator.GetNextAnimatorStateInfo(0).shortNameHash;
      return next == _launchWeakHash || next == _launchStrongHash;
    }

    private static bool IsLaunch(EPlayerAnimation animation) =>
      animation is EPlayerAnimation.LaunchWeak or EPlayerAnimation.LaunchStrong;

    private (int hash, float fade) Resolve(EPlayerAnimation animation) =>
      animation switch
      {
        EPlayerAnimation.Idle => (_idleHash, _idleFadeDuration),
        EPlayerAnimation.InAir => (_inAirHash, _inAirFadeDuration),
        EPlayerAnimation.WallSlide => (_wallSlideHash, _wallSlideFadeDuration),
        EPlayerAnimation.LaunchWeak => (_launchWeakHash, _launchWeakFadeDuration),
        EPlayerAnimation.LaunchStrong => (_launchStrongHash, _launchStrongFadeDuration),
        _ => (_idleHash, 0f)
      };
  }
}
