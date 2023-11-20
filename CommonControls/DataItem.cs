﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     CommonControls
 * FILE:        CommonControls/DataItem.cs
 * PURPOSE:     Basic Object needed for DataList
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable ArrangeBraces_foreach

using System;
using ViewModel;

namespace CommonControls
{
    /// <inheritdoc cref="ObservableObject" />
    /// <summary>
    ///     Data Object
    /// </summary>
    public sealed class DataItem : ObservableObject, IEquatable<DataItem>
    {
        /// <summary>
        ///     The id.
        /// </summary>
        private readonly int _id;

        /// <summary>
        ///     The name.
        /// </summary>
        private readonly string _name;

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        public int Id
        {
            get => _id;
            init
            {
                _id = value;
                RaisePropertyChangedEvent(nameof(Id));
            }
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public string Name
        {
            get => _name;
            init
            {
                _name = value;
                RaisePropertyChangedEvent(nameof(Name));
            }
        }

        public bool Equals(DataItem other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return _id == other._id && _name == other._name;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || (obj is DataItem other && Equals(other));
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(_id, _name);
        }
    }
}