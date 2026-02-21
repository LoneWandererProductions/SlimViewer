/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        UndoManager.cs
 * PURPOSE:     Simple Undo/Redo manager for image editing operations, using LinkedLists to maintain a history of states.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;

namespace SlimViews
{
    /// <summary>
    /// A generic, capacity-limited Undo/Redo manager using LinkedLists for fast insertion and deletion.
    /// </summary>
    /// <typeparam name="T">The type of state object being tracked (e.g., BitmapSource).</typeparam>
    public class UndoManager<T>
    {
        /// <summary>
        /// The limit
        /// </summary>
        private readonly int _limit;

        /// <summary>
        /// The undo stack
        /// </summary>
        private readonly LinkedList<T> _undoStack = new LinkedList<T>();

        /// <summary>
        /// The redo stack
        /// </summary>
        private readonly LinkedList<T> _redoStack = new LinkedList<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoManager{T}"/> class.
        /// </summary>
        /// <param name="limit">The limit.</param>
        public UndoManager(int limit = 5)
        {
            _limit = limit;
        }

        /// <summary>
        /// Gets a value indicating whether this instance can undo.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can undo; otherwise, <c>false</c>.
        /// </value>
        public bool CanUndo => _undoStack.Count > 0;

        /// <summary>
        /// Gets a value indicating whether this instance can redo.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can redo; otherwise, <c>false</c>.
        /// </value>
        public bool CanRedo => _redoStack.Count > 0;

        /// <summary>
        /// Call this BEFORE applying a filter or making a change.
        /// </summary>
        /// <param name="state">The state.</param>
        public void RecordState(T state)
        {
            _undoStack.AddLast(state);

            // Enforce the maximum limit by dropping the oldest state
            if (_undoStack.Count > _limit)
            {
                _undoStack.RemoveFirst();
            }

            // Any new action completely invalidates the "Redo" future
            _redoStack.Clear();
        }

        /// <summary>
        /// Moves back one step. Returns the previous state.
        /// </summary>
        /// <param name="currentState">State of the current.</param>
        /// <returns></returns>
        public T Undo(T currentState)
        {
            if (!CanUndo) return currentState;

            // Push the current state to the Redo stack
            _redoStack.AddLast(currentState);

            // Pop the previous state from the Undo stack
            var previousState = _undoStack.Last.Value;
            _undoStack.RemoveLast();

            return previousState;
        }

        /// <summary>
        /// Moves forward one step. Returns the next state.
        /// </summary>
        /// <param name="currentState">State of the current.</param>
        /// <returns></returns>
        public T Redo(T currentState)
        {
            if (!CanRedo) return currentState;

            // Push the current state back to the Undo stack
            _undoStack.AddLast(currentState);

            // Pop the next state from the Redo stack
            var nextState = _redoStack.Last.Value;
            _redoStack.RemoveLast();

            return nextState;
        }

        /// <summary>
        /// Clears all history. Call this when opening a new image.
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}