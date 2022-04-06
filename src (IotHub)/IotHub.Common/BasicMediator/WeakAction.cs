using IotHub.Common.BasicMediator.Interfaces;
using System.Reflection;

namespace IotHub.Common.BasicMediator
{
    /// <summary>
    /// Stores an <see cref="T:System.Action" /> without causing a hard reference
    /// to be created to the Action's owner. The owner can be garbage collected at any time.
    /// </summary>
    public class WeakAction
    {
        private Action _staticAction;

        /// <summary>
        /// Gets or sets the <see cref="T:System.Reflection.MethodInfo" /> corresponding to this WeakAction's
        /// method passed in the constructor.
        /// </summary>
        protected MethodInfo Method { get; set; }

        /// <summary>
        /// Gets the name of the method that this WeakAction represents.
        /// </summary>
        public virtual String MethodName => _staticAction != null ? _staticAction.GetMethodInfo().Name : Method.Name;

        /// <summary>
        /// Gets or sets a WeakReference to this WeakAction's action's target.
        /// This is not necessarily the same as
        /// <see cref="P:IotHub.Common.BasicMediator.WeakAction.Reference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference ActionReference { get; set; }

        /// <summary>
        /// Gets or sets a WeakReference to the target passed when constructing
        /// the WeakAction. This is not necessarily the same as
        /// <see cref="P:IotHub.Common.BasicMediator.WeakAction.ActionReference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference Reference { get; set; }

        /// <summary>
        /// Gets a value indicating whether the WeakAction is static or not.
        /// </summary>
        public Boolean IsStatic => _staticAction != null;

        /// <summary>
        /// Initializes an empty instance of the <see cref="T:IotHub.Common.BasicMediator.WeakAction" /> class.
        /// </summary>
        protected WeakAction()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:IotHub.Common.BasicMediator.WeakAction" /> class.
        /// </summary>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(Action action)
          : this(action == null ? null : action.Target, action)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:IotHub.Common.BasicMediator.WeakAction" /> class.
        /// </summary>
        /// <param name="target">The action's owner.</param>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(Object target, Action action)
        {
            if (action.GetMethodInfo().IsStatic)
            {
                _staticAction = action;
                if (target == null)
                    return;
                Reference = new WeakReference(target);
            }
            else
            {
                Method = action.GetMethodInfo();
                ActionReference = new WeakReference(action.Target);
                Reference = new WeakReference(target);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public virtual Boolean IsAlive
        {
            get
            {
                if (_staticAction == null && Reference == null)
                    return false;
                return _staticAction != null && Reference == null || Reference.IsAlive;
            }
        }

        /// <summary>
        /// Gets the Action's owner. This object is stored as a
        /// <see cref="T:System.WeakReference" />.
        /// </summary>
        public Object Target => Reference == null ? null : Reference.Target;

        /// <summary>The target of the weak reference.</summary>
        protected Object ActionTarget => ActionReference == null ? null : ActionReference.Target;

        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive.
        /// </summary>
        public void Execute()
        {
            if (_staticAction != null)
            {
                _staticAction();
            }
            else
            {
                var actionTarget = ActionTarget;
                if (!IsAlive || (Object)Method == null || ActionReference == null || actionTarget == null)
                    return;
                Method.Invoke(actionTarget, null);
            }
        }

        /// <summary>Sets the reference that this instance stores to null.</summary>
        public void MarkForDeletion()
        {
            Reference = null;
            ActionReference = null;
            Method = null;
            _staticAction = null;
        }
    }

    /// <summary>
    /// Stores an Action without causing a hard reference
    /// to be created to the Action's owner. The owner can be garbage collected at any time.
    /// </summary>
    /// <typeparam name="T">The type of the Action's parameter.</typeparam>
    public class WeakAction<T> : WeakAction, IExecuteWithObject
    {
        private Action<T> _staticAction;

        /// <summary>
        /// Gets the name of the method that this WeakAction represents.
        /// </summary>
        public override String MethodName => _staticAction != null ? _staticAction.GetMethodInfo().Name : Method.Name;

        /// <summary>
        /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public override Boolean IsAlive
        {
            get
            {
                if (_staticAction == null && Reference == null)
                    return false;
                if (_staticAction == null)
                    return Reference.IsAlive;
                return Reference == null || Reference.IsAlive;
            }
        }

        public WeakAction(Action<T> action)
          : this(action == null ? null : action.Target, action)
        {
        }

        public WeakAction(Object target, Action<T> action)
        {
            if (action.GetMethodInfo().IsStatic)
            {
                _staticAction = action;
                if (target == null)
                    return;
                Reference = new WeakReference(target);
            }
            else
            {
                Method = action.GetMethodInfo();
                ActionReference = new WeakReference(action.Target);
                Reference = new WeakReference(target);
            }
        }

        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive. The action's parameter is set to default(T).
        /// </summary>
        public new void Execute() => Execute(default(T));

        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive.
        /// </summary>
        /// <param name="parameter">A parameter to be passed to the action.</param>
        public void Execute(T parameter)
        {
            if (_staticAction != null)
            {
                _staticAction(parameter);
            }
            else
            {
                var actionTarget = ActionTarget;
                if (!IsAlive || (Object)Method == null || ActionReference == null || actionTarget == null)
                    return;
                Method.Invoke(actionTarget, new Object[1]
                {
          parameter
                });
            }
        }

        /// <summary>
        /// Executes the action with a parameter of type object. This parameter
        /// will be casted to T. This method implements <see cref="M:GalaSoft.MvvmLight.Helpers.IExecuteWithObject.ExecuteWithObject(System.Object)" />
        /// and can be useful if you store multiple WeakAction{T} instances but don't know in advance
        /// what type T represents.
        /// </summary>
        /// <param name="parameter">The parameter that will be passed to the action after
        /// being casted to T.</param>
        public void ExecuteWithObject(Object parameter) => Execute((T)parameter);

        /// <summary>
        /// Sets all the actions that this WeakAction contains to null,
        /// which is a signal for containing objects that this WeakAction
        /// should be deleted.
        /// </summary>
        public new void MarkForDeletion()
        {
            _staticAction = null;
            base.MarkForDeletion();
        }
    }
}
