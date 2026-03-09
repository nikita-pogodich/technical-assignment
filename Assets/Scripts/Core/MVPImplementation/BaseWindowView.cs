using Core.MVP;
using UnityEngine;

namespace Core.MVPImplementation
{
    public abstract class BaseWindowView : BaseView, IWindowView
    {
        [SerializeField]
        private Canvas _canvas;

        public override void SetShown(bool isShown)
        {
            base.SetShown(isShown);

            if (isShown == true)
            {
                transform.SetAsLastSibling();
            }
        }

        protected void SetCanvasEnabled(bool isEnabled)
        {
            _canvas.enabled = isEnabled;
        }
    }
}