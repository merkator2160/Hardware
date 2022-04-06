using IotHub.Common.BasicMediator.Interfaces;
using System.Reflection;

namespace IotHub.Common.BasicMediator
{
    /// <summary>
    /// The Messenger is a class allowing objects to exchange messages.
    /// </summary>
    public class Messenger : IMessenger
    {
        private static readonly Object CreationLock = new Object();
        private static IMessenger _defaultInstance;
        private readonly Object _registerLock = new Object();
        private Dictionary<Type, List<WeakActionAndToken>> _recipientsOfSubclassesAction;
        private Dictionary<Type, List<WeakActionAndToken>> _recipientsStrictAction;
        private readonly SynchronizationContext _context = SynchronizationContext.Current;
        private Boolean _isCleanupRegistered;

        /// <summary>
        /// Gets the Messenger's default instance, allowing
        /// to register and send messages in a static manner.
        /// </summary>
        public static IMessenger Default
        {
            get
            {
                if (_defaultInstance == null)
                {
                    lock (CreationLock)
                    {
                        if (_defaultInstance == null)
                            _defaultInstance = new Messenger();
                    }
                }
                return _defaultInstance;
            }
        }

        public virtual void Register<TMessage>(Object recipient, Action<TMessage> action) => Register(recipient, null, false, action);

        public virtual void Register<TMessage>(
          Object recipient,
          Boolean receiveDerivedMessagesToo,
          Action<TMessage> action)
        {
            Register(recipient, null, receiveDerivedMessagesToo, action);
        }

        public virtual void Register<TMessage>(Object recipient, Object token, Action<TMessage> action) => Register(recipient, token, false, action);

        public virtual void Register<TMessage>(
          Object recipient,
          Object token,
          Boolean receiveDerivedMessagesToo,
          Action<TMessage> action)
        {
            lock (_registerLock)
            {
                var key = typeof(TMessage);
                Dictionary<Type, List<WeakActionAndToken>> dictionary;
                if (receiveDerivedMessagesToo)
                {
                    if (_recipientsOfSubclassesAction == null)
                        _recipientsOfSubclassesAction = new Dictionary<Type, List<WeakActionAndToken>>();
                    dictionary = _recipientsOfSubclassesAction;
                }
                else
                {
                    if (_recipientsStrictAction == null)
                        _recipientsStrictAction = new Dictionary<Type, List<WeakActionAndToken>>();
                    dictionary = _recipientsStrictAction;
                }
                lock (dictionary)
                {
                    List<WeakActionAndToken> weakActionAndTokenList;
                    if (!dictionary.ContainsKey(key))
                    {
                        weakActionAndTokenList = new List<WeakActionAndToken>();
                        dictionary.Add(key, weakActionAndTokenList);
                    }
                    else
                        weakActionAndTokenList = dictionary[key];
                    var weakAction = new WeakAction<TMessage>(recipient, action);
                    var weakActionAndToken = new WeakActionAndToken()
                    {
                        Action = weakAction,
                        Token = token
                    };
                    weakActionAndTokenList.Add(weakActionAndToken);
                }
            }
            RequestCleanup();
        }

        /// <summary>
        /// Sends a message to registered recipients. The message will
        /// reach all recipients that registered for this message type
        /// using one of the Register methods.
        /// </summary>
        /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
        /// <param name="message">The message to send to registered recipients.</param>
        public virtual void Send<TMessage>(TMessage message) => SendToTargetOrType(message, null, null);

        /// <summary>
        /// Sends a message to registered recipients. The message will
        /// reach only recipients that registered for this message type
        /// using one of the Register methods, and that are
        /// of the targetType.
        /// </summary>
        /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
        /// <typeparam name="TTarget">The type of recipients that will receive
        /// the message. The message won't be sent to recipients of another type.</typeparam>
        /// <param name="message">The message to send to registered recipients.</param>
        public virtual void Send<TMessage, TTarget>(TMessage message) => SendToTargetOrType(message, typeof(TTarget), null);

        /// <summary>
        /// Sends a message to registered recipients. The message will
        /// reach only recipients that registered for this message type
        /// using one of the Register methods, and that are
        /// of the targetType.
        /// </summary>
        /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
        /// <param name="message">The message to send to registered recipients.</param>
        /// <param name="token">A token for a messaging channel. If a recipient registers
        /// using a token, and a sender sends a message using the same token, then this
        /// message will be delivered to the recipient. Other recipients who did not
        /// use a token when registering (or who used a different token) will not
        /// get the message. Similarly, messages sent without any token, or with a different
        /// token, will not be delivered to that recipient.</param>
        public virtual void Send<TMessage>(TMessage message, Object token) => SendToTargetOrType(message, null, token);

        /// <summary>
        /// Unregisters a messager recipient completely. After this method
        /// is executed, the recipient will not receive any messages anymore.
        /// </summary>
        /// <param name="recipient">The recipient that must be unregistered.</param>
        public virtual void Unregister(Object recipient)
        {
            UnregisterFromLists(recipient, _recipientsOfSubclassesAction);
            UnregisterFromLists(recipient, _recipientsStrictAction);
        }

        /// <summary>
        /// Unregisters a message recipient for a given type of messages only.
        /// After this method is executed, the recipient will not receive messages
        /// of type TMessage anymore, but will still receive other message types (if it
        /// registered for them previously).
        /// </summary>
        /// <param name="recipient">The recipient that must be unregistered.</param>
        /// <typeparam name="TMessage">The type of messages that the recipient wants
        /// to unregister from.</typeparam>
        public virtual void Unregister<TMessage>(Object recipient) => Unregister<TMessage>(recipient, null, null);

        /// <summary>
        /// Unregisters a message recipient for a given type of messages only and for a given token.
        /// After this method is executed, the recipient will not receive messages
        /// of type TMessage anymore with the given token, but will still receive other message types
        /// or messages with other tokens (if it registered for them previously).
        /// </summary>
        /// <param name="recipient">The recipient that must be unregistered.</param>
        /// <param name="token">The token for which the recipient must be unregistered.</param>
        /// <typeparam name="TMessage">The type of messages that the recipient wants
        /// to unregister from.</typeparam>
        public virtual void Unregister<TMessage>(Object recipient, Object token) => Unregister<TMessage>(recipient, token, null);

        public virtual void Unregister<TMessage>(Object recipient, Action<TMessage> action) => Unregister(recipient, null, action);

        public virtual void Unregister<TMessage>(
          Object recipient,
          Object token,
          Action<TMessage> action)
        {
            UnregisterFromLists(recipient, token, action, _recipientsStrictAction);
            UnregisterFromLists(recipient, token, action, _recipientsOfSubclassesAction);
            RequestCleanup();
        }

        /// <summary>
        /// Provides a way to override the Messenger.Default instance with
        /// a custom instance, for example for unit testing purposes.
        /// </summary>
        /// <param name="newMessenger">The instance that will be used as Messenger.Default.</param>
        public static void OverrideDefault(IMessenger newMessenger) => _defaultInstance = newMessenger;

        /// <summary>
        /// Sets the Messenger's default (static) instance to null.
        /// </summary>
        public static void Reset() => _defaultInstance = null;

        /// <summary>
        /// Provides a non-static access to the static <see cref="M:IotHub.Common.BasicMediator.Messenger.Reset" /> method.
        /// Sets the Messenger's default (static) instance to null.
        /// </summary>
        public void ResetAll() => Reset();

        private static void CleanupList(
          IDictionary<Type, List<WeakActionAndToken>> lists)
        {
            if (lists == null)
                return;
            lock (lists)
            {
                var typeList = new List<Type>();
                foreach (var list in lists)
                {
                    foreach (var weakActionAndToken in list.Value.Where((Func<WeakActionAndToken, Boolean>)(item => item.Action == null || !item.Action.IsAlive)).ToList())
                        list.Value.Remove(weakActionAndToken);
                    if (list.Value.Count == 0)
                        typeList.Add(list.Key);
                }
                foreach (var key in typeList)
                    lists.Remove(key);
            }
        }

        private static void SendToList<TMessage>(
          TMessage message,
          IEnumerable<WeakActionAndToken> weakActionsAndTokens,
          Type messageTargetType,
          Object token)
        {
            if (weakActionsAndTokens == null)
                return;
            var list = weakActionsAndTokens.ToList();
            foreach (var weakActionAndToken in list.Take(list.Count).ToList())
            {
                if (weakActionAndToken.Action is IExecuteWithObject action && weakActionAndToken.Action.IsAlive && weakActionAndToken.Action.Target != null && ((Object)messageTargetType == null || weakActionAndToken.Action.Target.GetType() == (Object)messageTargetType || messageTargetType.GetTypeInfo().IsAssignableFrom(weakActionAndToken.Action.Target.GetType().GetTypeInfo())) && (weakActionAndToken.Token == null && token == null || weakActionAndToken.Token != null && weakActionAndToken.Token.Equals(token)))
                    action.ExecuteWithObject(message);
            }
        }

        private static void UnregisterFromLists(
          Object recipient,
          Dictionary<Type, List<WeakActionAndToken>> lists)
        {
            if (recipient == null || lists == null || lists.Count == 0)
                return;
            lock (lists)
            {
                foreach (var key in lists.Keys)
                {
                    foreach (var weakActionAndToken in lists[key])
                    {
                        var action = (IExecuteWithObject)weakActionAndToken.Action;
                        if (action != null && recipient == action.Target)
                            action.MarkForDeletion();
                    }
                }
            }
        }

        private static void UnregisterFromLists<TMessage>(
          Object recipient,
          Object token,
          Action<TMessage> action,
          Dictionary<Type, List<WeakActionAndToken>> lists)
        {
            var key = typeof(TMessage);
            if (recipient == null || lists == null || lists.Count == 0 || !lists.ContainsKey(key))
                return;
            lock (lists)
            {
                foreach (var weakActionAndToken in lists[key])
                {
                    if (weakActionAndToken.Action is WeakAction<TMessage> action1 && recipient == action1.Target && (action == null || action.GetMethodInfo().Name == action1.MethodName) && (token == null || token.Equals(weakActionAndToken.Token)))
                        weakActionAndToken.Action.MarkForDeletion();
                }
            }
        }

        /// <summary>
        /// Notifies the Messenger that the lists of recipients should
        /// be scanned and cleaned up.
        /// Since recipients are stored as <see cref="T:System.WeakReference" />,
        /// recipients can be garbage collected even though the Messenger keeps
        /// them in a list. During the cleanup operation, all "dead"
        /// recipients are removed from the lists. Since this operation
        /// can take a moment, it is only executed when the application is
        /// idle. For this reason, a user of the Messenger class should use
        /// <see cref="M:IotHub.Common.BasicMediator.Messenger.RequestCleanup" /> instead of forcing one with the
        /// <see cref="M:IotHub.Common.BasicMediator.Messenger.Cleanup" /> method.
        /// </summary>
        public void RequestCleanup()
        {
            if (_isCleanupRegistered)
                return;
            var cleanupAction = new Action(Cleanup);
            if (_context != null)
                _context.Post((SendOrPostCallback)(_ => cleanupAction()), null);
            else
                cleanupAction();
            _isCleanupRegistered = true;
        }

        /// <summary>
        /// Scans the recipients' lists for "dead" instances and removes them.
        /// Since recipients are stored as <see cref="T:System.WeakReference" />,
        /// recipients can be garbage collected even though the Messenger keeps
        /// them in a list. During the cleanup operation, all "dead"
        /// recipients are removed from the lists. Since this operation
        /// can take a moment, it is only executed when the application is
        /// idle. For this reason, a user of the Messenger class should use
        /// <see cref="M:IotHub.Common.BasicMediator.Messenger.RequestCleanup" /> instead of forcing one with the
        /// <see cref="M:IotHub.Common.BasicMediator.Messenger.Cleanup" /> method.
        /// </summary>
        public void Cleanup()
        {
            CleanupList(_recipientsOfSubclassesAction);
            CleanupList(_recipientsStrictAction);
            _isCleanupRegistered = false;
        }

        private void SendToTargetOrType<TMessage>(
          TMessage message,
          Type messageTargetType,
          Object token)
        {
            var type1 = typeof(TMessage);
            if (_recipientsOfSubclassesAction != null)
            {
                foreach (var type2 in _recipientsOfSubclassesAction.Keys.Take(_recipientsOfSubclassesAction.Count).ToList())
                {
                    var weakActionsAndTokens = (List<WeakActionAndToken>)null;
                    if (type1 == (Object)type2 || type1.GetTypeInfo().IsSubclassOf(type2) || type2.GetTypeInfo().IsAssignableFrom(type1.GetTypeInfo()))
                    {
                        lock (_recipientsOfSubclassesAction)
                            weakActionsAndTokens = _recipientsOfSubclassesAction[type2].Take(_recipientsOfSubclassesAction[type2].Count).ToList();
                    }
                    SendToList(message, weakActionsAndTokens, messageTargetType, token);
                }
            }
            if (_recipientsStrictAction != null)
            {
                var weakActionsAndTokens = (List<WeakActionAndToken>)null;
                lock (_recipientsStrictAction)
                {
                    if (_recipientsStrictAction.ContainsKey(type1))
                        weakActionsAndTokens = _recipientsStrictAction[type1].Take(_recipientsStrictAction[type1].Count).ToList();
                }
                if (weakActionsAndTokens != null)
                    SendToList(message, weakActionsAndTokens, messageTargetType, token);
            }
            RequestCleanup();
        }

        private struct WeakActionAndToken
        {
            public WeakAction Action;
            public Object Token;
        }
    }
}
