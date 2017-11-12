using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public interface IDragAndDropContainer
    {
        bool CanDrag(IDraggable obj);
        bool CanDrop(IDraggable dragged, IDraggable drop);
        void Drop(IDraggable dragged, IDraggable drop, int replacedIndex, bool final);
        bool DoesSwap();
    }
}
