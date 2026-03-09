using System;
using Core.MVP;
using R3;
using UnityEngine;

namespace Core.MVPImplementation
{
    public abstract class BaseView : MonoBehaviour, IView
    {
        private IDisposable _reactiveDisposable;

        public string ViewName { get; private set; }

        public void Init(string viewName)
        {
            ViewName = viewName;
            DisposableBuilder disposableBuilder = Disposable.CreateBuilder();
            OnInit(ref disposableBuilder);
            _reactiveDisposable = disposableBuilder.Build();
        }

        public void Deinit()
        {
            OnDeinit();
            _reactiveDisposable.Dispose();
        }

        public virtual void SetShown(bool isShown)
        {
        }

        protected virtual void OnInit(ref DisposableBuilder disposableBuilder)
        {
        }

        protected virtual void OnDeinit()
        {
        }
    }
}