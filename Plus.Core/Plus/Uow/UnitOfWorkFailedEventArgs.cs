﻿using JetBrains.Annotations;
using System;

namespace Plus.Uow
{
    /// <summary>
    /// Used as event arguments on <see cref="IUnitOfWork.Failed"/> event.
    /// </summary>
    public class UnitOfWorkFailedEventArgs : UnitOfWorkEventArgs
    {
        /// <summary>
        /// Exception that caused failure. This is set only if an error occurred during <see cref="IUnitOfWork.Complete"/>.
        /// Can be null if there is no exception, but <see cref="IUnitOfWork.Complete"/> is not called. 
        /// Can be null if another exception occurred during the UOW.
        /// </summary>
        [CanBeNull]
        public Exception Exception { get; }

        /// <summary>
        /// True, if the unit of work is manually rolled back.
        /// </summary>
        public bool IsRolledback { get; }

        /// <summary>
        /// Creates a new <see cref="UnitOfWorkFailedEventArgs"/> object.
        /// </summary>
        public UnitOfWorkFailedEventArgs([NotNull] IUnitOfWork unitOfWork, [CanBeNull] Exception exception, bool isRolledback)
            : base(unitOfWork)
        {
            Exception = exception;
            IsRolledback = isRolledback;
        }
    }
}