using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOOPipeline
{
    public class Entity 
    {
        public int id;
        public List<Component> components;

        protected Entity()
        {
            id = EntityIdHandler.GetInstance().GetId();
            components = new List<Component>();
        }

        public virtual bool Initialize()
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] != null)
                {
                    components[i].Load();
                }
            }
            return true;
        }

        public virtual bool Release()
        {

            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].needToDispose) {
                    components[i].Dispose();
                }
                components[i] = null;
                components.Clear();
            }

            return true;
        }

    }

}
