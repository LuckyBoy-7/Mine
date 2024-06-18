namespace Mine.States
{
    public interface IState
    {
        public void OnEnter();
        public void OnExit();
        public void OnUpdate();
        public void OnFixedUpdate();
    }
}