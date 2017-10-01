using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public interface IMenuItemResponder
    {
        void OnButtonDown(MenuItem item);
        void OnButtonUp(MenuItem item);
    }
}
