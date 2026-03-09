using System;
using System.Collections.Generic;
using Core.MVP;
using R3;

namespace Core.MVPImplementation
{
    public abstract class BasePresenter<TView, TModel> : IPresenter<TView, TModel>
        where TView : IView
        where TModel : IModel
    {
        private TView _view;
        private TModel _model;
        private bool _isShown;
        private IDisposable _reactiveDisposable;
        private readonly List<IPresenter> _childPresenters = new();

        public TView View => _view;
        public TModel Model => _model;
        public bool IsShown => _isShown;

        public void Init(TView view, TModel model)
        {
            _model = model;
            _view = view;

            DisposableBuilder disposableBuilder = Disposable.CreateBuilder();
            OnInit(ref disposableBuilder);
            _reactiveDisposable = disposableBuilder.Build();
        }

        public void Deinit()
        {
            OnDeinit();

            _reactiveDisposable.Dispose();

            foreach (IPresenter childPresenter in _childPresenters)
            {
                childPresenter.Deinit();
            }

            _childPresenters.Clear();
        }

        public virtual void SetShown(bool isShown)
        {
            _isShown = isShown;
            View.SetShown(isShown);

            if (_isShown)
            {
                OnShow();
            }
            else
            {
                OnHide();
            }

            foreach (IPresenter presenter in _childPresenters)
            {
                presenter.SetShown(isShown);
            }
        }

        protected virtual void OnInit(ref DisposableBuilder disposableBuilder)
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnDeinit()
        {
        }

        protected void AddChildPresenter(IPresenter presenter)
        {
            _childPresenters.Add(presenter);
        }

        protected void RemoveChildPresenter(IPresenter presenter)
        {
            _childPresenters.Remove(presenter);
        }
    }
}