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

        public override void ResetForSpawn()
        {
            onSpawnCalled = true;
        }

        public override void ReleaseForDespawn()
        {
            onDespawnCalled = true;
        }
    }
}