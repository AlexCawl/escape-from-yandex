namespace GameMaster.State
{
    public abstract class BaseState<T>
    {
        protected T Value;

        public T Get => Value;

        public virtual void Set(T newValue) => Value = newValue;
    }
}