using System;
using AstroRush.Core;

namespace AstroRush.Core
{
    /// <summary>
    /// DESIGN PATTERN: OBSERVER (event hub / subject).
    ///
    /// Gameplay code raises events here. UI, audio, and race logic
    /// subscribe independently — they share zero direct references
    /// with the code that fires the events.
    ///
    /// Payoff demonstrated: RaceManager was added after PlayerController
    /// was already working. It subscribes to OnRacerFinished without
    /// touching PlayerController, AIController, or Bullet.
    /// </summary>
    public static class GameEvents
    {
        public static event Action<Racer>   OnRacerHit;
        public static event Action<Racer>   OnRacerFinished;   // reached finish line
        public static event Action<int>     OnCountdownTick;   // 3-2-1-GO
        public static event Action          OnRaceStart;
        public static event Action          OnRaceEnd;

        public static void RaiseRacerHit(Racer r)          => OnRacerHit?.Invoke(r);
        public static void RaiseRacerFinished(Racer r)     => OnRacerFinished?.Invoke(r);
        public static void RaiseCountdownTick(int n)        => OnCountdownTick?.Invoke(n);
        public static void RaiseRaceStart()                 => OnRaceStart?.Invoke();
        public static void RaiseRaceEnd()                   => OnRaceEnd?.Invoke();
    }
}
