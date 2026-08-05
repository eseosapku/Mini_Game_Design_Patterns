using UnityEngine;
using AstroRush.Core;

namespace AstroRush.Patterns
{
    public enum SpawnType { Bullet, Obstacle }

    /// <summary>
    /// DESIGN PATTERN: FACTORY METHOD.
    ///
    /// Centralises "how do I build this object" behind Create(type).
    /// PlayerController and AIController just call
    ///   SpawnerFactory.Instance.Create(SpawnType.Bullet, transform, ...)
    /// without knowing anything about prefab setup or bullet initialisation.
    ///
    /// Payoff: adding a new projectile type (e.g. a freeze bomb) is one new
    /// enum value and one private Create method — call sites don't change.
    /// </summary>
    public class SpawnerFactory : MonoBehaviour
    {
        public static SpawnerFactory Instance { get; private set; }

        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject obstaclePrefab;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public GameObject Create(SpawnType type, Transform origin, Vector2 direction)
        {
            return type switch
            {
                SpawnType.Bullet   => CreateBullet(origin, direction),
                SpawnType.Obstacle => CreateObstacle(origin),
                _                  => null,
            };
        }

        private GameObject CreateBullet(Transform origin, Vector2 dir)
        {
            var go  = Instantiate(bulletPrefab, origin.position, Quaternion.identity);
            var b   = go.GetComponent<Combat.Bullet>();
            b?.Init(dir);
            return go;
        }

        private GameObject CreateObstacle(Transform origin)
        {
            return Instantiate(obstaclePrefab, origin.position, Quaternion.identity);
        }
    }
}
