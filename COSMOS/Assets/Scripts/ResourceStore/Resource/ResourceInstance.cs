namespace COSMOS.ResourceStore
{
    public abstract class ResourceInstance
    {
        public Resource Resource { get; private set; }

        public bool IsFree { get; private set; }


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
            IsFree = true;

            OnFree();
        }

        protected abstract void OnFree();
    }
}