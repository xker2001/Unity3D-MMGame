namespace MMGame
{
    public class UnitTestPoolManagerEntity : PoolBehaviour
    {
        private bool onSpawnCalled;
        private bool onDespawnCalled;

        public bool OnSpawnCalled
        {
            get { return onSpawnCalled; }
        }

        public bool OnDespawnCalled
        {
            get { return onDespawnCalled; }
        }

        private void OnEnable()
        {
            onDespawnCalled = false;
        }

        private void OnDisable()
        {
            onSpawnCalled = false;
        }

        public override void OnSpawn()
        {
            onSpawnCalled = true;
        }

        public override void OnDespawn()
        {
            onDespawnCalled = true;
        }
    }
}