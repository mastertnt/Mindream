using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindream
{
    public delegate void MethodEnd();
    public abstract class AComponent : IComponent
    {
        public IComponentDescriptor Descriptor
        {
            get { throw new NotImplementedException(); }
        }

        public Dictionary<string, object> Inputs
        {
            get { throw new NotImplementedException(); }
        }

        public Dictionary<string, object> Outputs
        {
            get { throw new NotImplementedException(); }
        }

        public event Action<IComponent> Started;

        public event Action<IComponent, string> Ended;

        public event Action<IComponent> Failed;

        public object this[string pParameterName]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Start()
        {
            if (this.Started != null)
            {
                this.Started(this);
            }
            this.StartProtected();
        }

        public void Stop()
        {
            this.StopProtected();
        }

        public virtual void StartProtected()
        {
            // Nothing to do.
        }

        public virtual void StopProtected()
        {
            // Nothing to do.
        }

        public void NotifyEnd(string pResult)
        {
            
        }
    }
}
