using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class IDraggable : MonoBehaviour
    {

        public int index;
        public static string DRAGGABLE_TAG = "Draggable";
        public IDragAndDropContainer cachedContainer = null;

        protected void Init()
        {
            tag = DRAGGABLE_TAG;
        }

        public IDragAndDropContainer GetContainer()
        {
            IDragAndDropContainer container = null;
            Transform par = transform.parent;

            while (container == null && par != null)
            {
                container = par.GetComponent<IDragAndDropContainer>();
                par = par.parent;
            }

            if (container != null)
                cachedContainer = container;
            else
                container = cachedContainer;

            return container;
        }

    }
}
