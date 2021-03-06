﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Ubiquity.ArgValidators
{
    /// <summary>Marker Attribute to inform CodeAnalysis that a parameter is validated as non-null in a method</summary>
    [SuppressMessage( "", "SA1402", Justification = "Closely related to the static validators" )]
    [AttributeUsage( AttributeTargets.Parameter, Inherited = true, AllowMultiple = false )]
    public sealed class ValidatedNotNullAttribute
        : Attribute
    {
    }

    /// <summary>Parameter validation extension set</summary>
    public static class Validators
    {
        /// <summary>Verifies a that a value is not equal to the default for the type</summary>
        /// <typeparam name="T">Type of value to test for</typeparam>
        /// <param name="value">Value to test</param>
        /// <param name="paramName">Name of the parameter for the argument exception generated</param>
        /// <returns><paramref name="value"/> for fluent design usage</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is the default value for value types or <see lang="null"/> for reference types</exception>
        [DebuggerStepThrough]
        public static T ValidateNotDefault<T>( [ValidatedNotNull] this T value, [InvokerParameterName] string paramName )
        {
            if( EqualityComparer<T>.Default.Equals( value, default ) )
            {
                throw new ArgumentNullException( paramName );
            }

            return value;
        }

        /// <summary>Verifies an object isn't null</summary>
        /// <param name="value">Object instance to check</param>
        /// <param name="paramName">Name of the parameter for the exception if <paramref name="value"/> is null</param>
        [ContractAnnotation( "obj:null => halt" )]
        [DebuggerStepThrough]
        public static T ValidateNotNull<T>( [ValidatedNotNull] this T value, [InvokerParameterName] string paramName )
            where T : class
        {
            if( value == null )
            {
                throw new ArgumentNullException( paramName );
            }

            return value;
        }

        /// <summary>Verifies an IntPtr isn't null</summary>
        /// <param name="value">value to check</param>
        /// <param name="paramName">Name of the parameter for the exception if <paramref name="obj"/> is null</param>
        [ContractAnnotation( "obj:null => halt" )]
        [DebuggerStepThrough]
        public static IntPtr ValidateNotNull( this IntPtr value, [InvokerParameterName] string paramName )
        {
            if( value == IntPtr.Zero )
            {
                throw new ArgumentNullException( paramName );
            }

            return value;
        }

        /// <summary>Verifies an UIntPtr isn't null</summary>
        /// <param name="value">value to check</param>
        /// <param name="paramName">Name of the parameter for the exception if <paramref name="obj"/> is null</param>
        [ContractAnnotation( "obj:null => halt" )]
        [DebuggerStepThrough]
        public static UIntPtr ValidateNotNull( this UIntPtr value, [InvokerParameterName] string paramName )
        {
            if( value == UIntPtr.Zero )
            {
                throw new ArgumentNullException( paramName );
            }

            return value;
        }

        /// <summary>Validates a parameter string is not null or white space</summary>
        /// <param name="value">string to test</param>
        /// <param name="paramName">Name of the parameter for the exception if the <paramref name="value"/> is null or whitespace</param>
        [ContractAnnotation( "str:null => halt" )]
        [DebuggerStepThrough]
        public static void ValidateNotNullOrWhiteSpace( [ValidatedNotNull] this string value, [InvokerParameterName] string paramName )
        {
            if( value == null )
            {
                throw new ArgumentNullException( paramName );
            }

            if( value.Length == 0 || ( ( value.Length == 1 ) && ( value[0]=='\0' ) ) )
            {
                throw new ArgumentException( "Must not be empty", paramName );
            }

            if( string.IsNullOrWhiteSpace( value ) )
            {
                throw new ArgumentException( "Must not be whitespace", paramName );
            }
        }

        /// <summary>Verifies a parameter is within a valid range</summary>
        /// <typeparam name="T">Type of the parameter (must be primitive type supporting comparisons)</typeparam>
        /// <param name="i">Value of the parameter to test</param>
        /// <param name="min">Minimum allowed value</param>
        /// <param name="max">Maximum allowed value</param>
        /// <param name="paramName">Name of the parameter for the exception if the <paramref name="i"/> is outside the specified range</param>
        [DebuggerStepThrough]
        public static void ValidateRange<T>( this T i, T min, T max, [InvokerParameterName] string paramName )
            where T : struct, IComparable<T>
        {
            if( min.CompareTo( i ) > 0 || i.CompareTo( max ) > 0 )
            {
                throw new ArgumentOutOfRangeException( paramName, i, $"Accepted range: [{min}, {max}]" );
            }
        }

        /// <summary>Verifies that a string matches a Regular expression pattern</summary>
        /// <param name="value">String value to validate</param>
        /// <param name="pattern">Pattern to use for testing <paramref name="value"/></param>
        /// <param name="paramName">Name of the parameter for the exception message if <paramref name="value"/> does not match the pattern</param>
        [ContractAnnotation( "value:null => halt" )]
        [DebuggerStepThrough]
        public static Match ValidatePattern( [ValidatedNotNull] this string value, [RegexPattern] string pattern, [InvokerParameterName] string paramName )
        {
            if( string.IsNullOrWhiteSpace( value ) )
            {
                throw new ArgumentException( "Must not be null or whitespace", paramName );
            }

            if( string.IsNullOrWhiteSpace( pattern ) )
            {
                throw new ArgumentException( "Must not be null or whitespace", nameof( pattern ) );
            }

            var regEx = new Regex( pattern );
            var match = regEx.Match( value );
            if( !match.Success )
            {
                throw new ArgumentException( $"Value does not conform to required format: {pattern}", paramName );
            }

            return match;
        }

        /// <summary>Verifies that a string matches a Regular expression pattern</summary>
        /// <param name="value">String value to validate</param>
        /// <param name="pattern">Pattern to use for testing <paramref name="value"/></param>
        /// <param name="paramName">Name of the parameter for the exception message if <paramref name="value"/> does not match the pattern</param>
        [ContractAnnotation( "value:null => halt" )]
        [DebuggerStepThrough]
        public static Match ValidatePattern( [ValidatedNotNull] this string value, Regex pattern, [InvokerParameterName] string paramName )
        {
            if( string.IsNullOrWhiteSpace( value ) )
            {
                throw new ArgumentException( "Must not be null or whitespace", paramName );
            }

            if( pattern == null )
            {
                throw new ArgumentNullException( nameof( pattern ) );
            }

            var match = pattern.Match( value );
            if( !match.Success )
            {
                throw new ArgumentException( $"Value does not conform to required format: {pattern}", paramName );
            }

            return match;
        }

        /// <summary>Validates a string length falls within a specific range</summary>
        /// <param name="value">String to validate</param>
        /// <param name="min">Minimum allowed number of characters in the string</param>
        /// <param name="max">Maximum allowed number of characters in the string</param>
        /// <param name="paramName">Name of the parameter for the exception message if <paramref name="value"/> length is outside the specified range</param>
        [ContractAnnotation( "value:null => halt" )]
        [DebuggerStepThrough]
        public static string ValidateLength( [ValidatedNotNull] this string value, int min, int max, [InvokerParameterName] string paramName )
        {
            if( value == null )
            {
                throw new ArgumentNullException( nameof( value ) );
            }

            if( value.Length < min || value.Length > max )
            {
                throw new ArgumentException( $"Expected string with length in the range [{min}, {max}]", paramName );
            }

            return value;
        }

        /// <summary>Validates that a value is defined on the Enum type</summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="value">Value to check is defined</param>
        /// <param name="paramName">name of the parameter owning the value</param>
        /// <returns></returns>
        public static T ValidateDefined<T>( this T value, [InvokerParameterName]string paramName )
            where T : struct, IConvertible, IComparable, IFormattable
        {
            if( !Enum.IsDefined( typeof( T ), value ) )
            {
                throw new ArgumentException( $"Type {typeof( T ).FullName} does not define an enumerated value for '{value}'", paramName );
            }

            return value;
        }
    }
}
