namespace COSMOS.ResourceStore
{
    public abstract class ResourceInstance
    {
        public Resource Resource { get; private set; }

        public bool IsFree { get; private set; }

        // ссылка на ресурс готовый к работе

        public void Free()
        {
            if (IsFree)
            {
                return;
            }


            if (Resource != null)
            {
                Resource.ReturnInstance(this);
            }
            OnFree();
        }

        protected abstract void OnFree();
    }
}
