﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace System.Option
{
    /// <summary>
    /// A generic Option type that allows for an explicit difference
    /// between an intentionally set value, and a default value of None.
    /// </summary>
    /// <typeparam name="T">The type to create an option for.</typeparam>
    [DebuggerDisplay("HasValue = {_hasValue}, Value = {_value}")]
    public struct Option<T> : IEquatable<Option<T>>
    {
        /// <summary>
        /// The value of the option.
        /// </summary>
        private T _value;

        /// <summary>
        /// The bool indicating whether the option has a value.
        /// </summary>
        private readonly bool _hasValue;

        private static Option<T> _none = new Option<T>();

        /// <summary>
        /// Creates a new option from a specified value.
        /// </summary>
        /// <typeparam name="T">The type to create an option for.</typeparam>
        /// <param name="value">The value to create an option for.</param>
        /// <returns>
        /// A new Option&lt;T&gt; whose value is
        /// set to <paramref name="value"/>.
        /// </returns>
        public static Option<T> Some(T value)
        {
            return new Option<T>(value);
        }

        /// <summary>
        /// The Option indication there is no value.
        /// </summary>
        public static Option<T> None
        {
            get { return Option<T>._none; }
        }

        /// <summary>
        /// True if the option has a value, false otherwise.
        /// </summary>
        public bool HasValue
        {
            get { return this._hasValue; }
        }

        /// <summary>
        /// Gets the value of the option
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the option does not have a value.
        /// </exception>
        [DebuggerDisplay("{_value}")]
        public T Value
        {
            get
            {
                if (!this.HasValue)
                {
                    throw new InvalidOperationException(
                        "The option does not have a value");
                }

                return this._value;
            }
        }

        /// <summary>
        /// Gets the value of the option if present,
        /// and the default value otherwise.
        /// </summary>
        public T ValueOrDefault
        {
            get { return this._hasValue ? this._value : default(T); }
        }

        /// <summary>
        /// Tries to get the value of an option and place
        /// it in the referenced result.
        /// </summary>
        /// <param name="result">
        /// The location to store the option's value.
        /// </param>
        /// <returns>true if the option has a value, false otherwise.</returns>
        public bool TryGetValue(out T result)
        {
            result = this.ValueOrDefault;

            return this.HasValue;
        }

        public T ValueOr(T val)
        {
            return this._hasValue ? this._value : val;
        }

        internal Option(T value)
        {
            this._hasValue = true;
            this._value = value;
        }

        #region Pattern Matching

        /// <summary>
        /// Creates and returns an OptionPatternMatcher&lt;T&gt;
        /// made from the current option.
        /// </summary>
        /// <returns>
        /// An OptionPatternMatcher&lt;T&gt; made from the current option.
        /// </returns>
        public OptionPatternMatcher<T> Match()
        {
            return new OptionPatternMatcher<T>(this);
        }

        /// <summary>
        /// Runs the appropriate action on the option depending on
        /// whether or not the option has a value.
        /// </summary>
        /// <param name="None">
        /// The action to run if the option doesn't have a value.
        /// </param>
        /// <param name="Some">
        /// The action to run if the option does have a value.
        /// </param>
        public void Match(Action None = null, Action<T> Some = null)
        {
            if (!this._hasValue)
            {
                if (null != None) None();
            }
            else
            {
                if (null != Some) Some(this._value);
            }
        }

        /// <summary>
        /// Runs the appropriate function on the option depending on
        /// whether or not the option has a value.
        /// </summary>
        /// <param name="None">
        /// The function to run if the option doesn't have a value.
        /// </param>
        /// <param name="Some">
        /// The function to run if the option does have a value.
        /// </param>
        public TOut Match<TOut>(Func<TOut> None, Func<T, TOut> Some)
        {
            if (!this._hasValue)
            {
                return None();
            }
            else
            {
                return Some(this._value);
            }
        }

        /// <summary>
        /// Creates and returns an OptionPatternMatcher&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// An OptionPatternMatcher&lt;T&gt;.
        /// </returns>
        public static OptionPatternMatcher<T> PatternMatch()
        {
            return new OptionPatternMatcher<T>();
        }

        /// <summary>
        /// Creates and returns an OptionPatternMatcher&lt;T,TOut&gt;
        /// made from the current option.
        /// </summary>
        /// <typeparam name="TOut">
        /// The type which the OptionPatternMatcher&lt;T,TOut&gt; will return.
        /// </typeparam>
        /// <returns>
        /// An OptionPatternMatcher&lt;T,TOut&gt; made from the current option.
        /// </returns>
        public OptionPatternMatcher<T, TOut> Match<TOut>()
        {
            return new OptionPatternMatcher<T, TOut>(this);
        }

        /// <summary>
        /// Creates and returns an OptionPatternMatcher&lt;T,TOut&gt;.
        /// </summary>
        /// <typeparam name="TOut">
        /// The type which the OptionPatternMatcher&lt;T,TOut&gt; will return.
        /// </typeparam>
        /// <returns>
        /// An OptionPatternMatcher&lt;T,TOut&gt;.
        /// </returns>
        public static OptionPatternMatcher<T, TOut> PatternMatch<TOut>()
        {
            return new OptionPatternMatcher<T, TOut>();
        }

        #endregion


        #region Operators

        /// <summary>
        /// Implicitly converts an Option to an Option&lt;T&gt;.
        /// </summary>
        /// <param name="option">The option to convert.</param>
        /// <returns>Option&lt;T&gt;.None</returns>
        public static implicit operator Option<T>(Option option)
        {
            return Option<T>.None;
        }

        /// <summary>
        /// Implicitly converts a value to an Option&lt;T&gt;.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>
        /// Option&lt;T&gt;.None if value is null, otherwise an
        /// Option&lt;T&gt; whose value is set to <paramref name="value"/>.
        /// </returns>
        public static implicit operator Option<T>(T value)
        {
            return null == value
                ? Option<T>.None
                : new Option<T>(value);
        }

        /// <summary>
        /// Implicitly converts an Option&lt;T&gt; to a T.
        /// </summary>
        /// <param name="option">The option to convert.</param>
        /// <returns>
        /// Option&lt;T&gt;.Value, which will throw InvalidOperationException
        /// if the option does not have a value.
        /// </returns>
        [Obsolete("This is not a safe operation! "
            +"You should use Option.ValueOr(val) "
            +"or Option.ValueOrDefault instead.")]
        public static implicit operator T(Option<T> option)
        {
            return option.Value;
        }

        /// <summary>
        /// Comapares two options for equality.
        /// </summary>
        /// <param name="lhs">The option on the left hand side.</param>
        /// <param name="rhs">The option on the right hand side.</param>
        /// <returns>
        /// true if the options' values are equal
        /// or both options are Option&lt;T&gt;.None,
        /// and false if the options' values are not
        /// equal or only one option is Option&lt;T&gt;.None
        /// </returns>
        public static bool operator ==(Option<T> lhs, Option<T> rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Comapares two options for inequality.
        /// </summary>
        /// <param name="lhs">The option on the left hand side.</param>
        /// <param name="rhs">The option on the right hand side.</param>
        /// <returns>
        /// true if the options' values are not
        /// equal or only one option is Option&lt;T&gt;.None,
        /// and false if the options' values are equal
        /// or both options are Option&lt;T&gt;.None.
        /// </returns>
        public static bool operator !=(Option<T> lhs, Option<T> rhs)
        {
            if (null == (object)lhs)
            {
                if (null == (object)rhs)
                {
                    return false;
                }
                return true;
            }
            return !lhs.Equals(rhs);
        }

        #endregion

        #region IEquatable<T> Members

        /// <summary>
        /// Compares the option to another option for equality.
        /// </summary>
        /// <param name="other">The option to compare to.</param>
        /// <returns>
        /// true if the options' values are equal
        /// or both options are Option&lt;T&gt;.None,
        /// and false if the options' values are not
        /// equal or only one option is Option&lt;T&gt;.None
        /// </returns>
        public bool Equals(Option<T> other)
        {
            if (this.HasValue != other.HasValue)
            {
                return false;
            }

            // Both are Option<T>.None
            if (!this.HasValue)
            {
                return true;
            }

            return EqualityComparer<T>.Default.Equals(
                this._value,
                other.Value);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Compares the option to another object for equality.
        /// </summary>
        /// <param name="other">The object to compare to.</param>
        /// <returns>
        /// true if the object is an Option&lt;T&gt; and
        /// the options' values are equal
        /// or both options are Option&lt;T&gt;.None,
        /// and false if the object is not an Option&lt;T&gt; or
        /// the options' values are not equal
        /// or only one option is Option&lt;T&gt;.None
        /// </returns>
        public override bool Equals(object other)
        {
            if (other is Option<T>)
            {
                return Equals((Option<T>)other);
            }

            return false;
        }

        /// <summary>
        /// Gets the HashCode for the Option&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// 0 if the Option is Option.None, otherwise
        /// returns the hash code of the value.
        /// </returns>
        public override int GetHashCode()
        {
            if (!this._hasValue)
            {
                return 0;
            }

            return EqualityComparer<T>.Default.GetHashCode(this._value);
        }

        #endregion
    }

    /// <summary>
    /// An Option type that allows the use of Option.None
    /// as well as the creation of Options.
    /// </summary>
    public sealed class Option
    {
        private static Option _none = null;

        private Option()
        {
        }

        /// <summary>
        /// Creates a new option from a specified value.
        /// </summary>
        /// <typeparam name="T">The type to create an option for.</typeparam>
        /// <param name="value">The value to create an option for.</param>
        /// <returns>
        /// A new Option&lt;T&gt; whose value is
        /// set to <paramref name="value"/>.
        /// </returns>
        public static Option<T> Some<T>(T value)
        {
            if (null == value)
            {
                throw new ArgumentNullException("value");
            }

            return Option<T>.Some(value);
        }

        /// <summary>
        /// The default Option type specifying there is no value.
        /// </summary>
        public static Option None
        {
            get { return Option._none; }
        }
    }

    /// <summary>
    /// A class used solely for pattern matching
    /// </summary>
    /// <typeparam name="T">The type of option</typeparam>
    public sealed class None<T>
    {
        private None() { }
    }

    /// <summary>
    /// A class used solely for pattern matching
    /// </summary>
    /// <typeparam name="T">The type of option</typeparam>
    public sealed class Some<T>
    {
        private Some() { }
    }
}
