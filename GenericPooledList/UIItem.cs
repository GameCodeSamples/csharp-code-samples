using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wonderland
{
    public abstract class UIItem<TViewModel> : MonoBehaviour
    {
        public abstract void Repaint(TViewModel viewModel);
    }
}