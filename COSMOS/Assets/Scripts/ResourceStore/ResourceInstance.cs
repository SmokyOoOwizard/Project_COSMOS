namespace COSMOS.ResourceStore
{
    public abstract class ResourceInstance
    {
        public Resource Resource { get; private set; }

        public bool IsFree { get; private set; }

        // ссылка на ресурс готовый к работе

        public void Free()
        {
            InternalFree();
            if (Resource != null)
            {
                Resource.ReturnInstance(this);
                Resource = null;
            }
        }

        internal void InternalFree()
        {
            if (IsFree)
            {
                return;
            }

            OnFree();
        }

        protected abstract void OnFree();
    }
}