using System;

namespace IotHub.Common.BasicMediator.Interfaces
{
    /// <summary>
    /// This interface is meant for the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakAction`1" /> class and can be
    /// useful if you store multiple WeakAction{T} instances but don't know in advance
    /// what type T represents.
    /// </summary>
    public interface IExecuteWithObject
    {
        /// <summary>The target of the WeakAction.</summary>
        Object Target { get; }

        /// <summary>Executes an action.</summary>
        /// <param name="parameter">A parameter passed as an object,
        /// to be casted to the appropriate type.</param>
        void ExecuteWithObject(Object parameter);

        /// <summary>
        /// Deletes all references, which notifies the cleanup method
        /// that this entry must be deleted.
        /// </summary>
        void MarkForDeletion();
    }
}