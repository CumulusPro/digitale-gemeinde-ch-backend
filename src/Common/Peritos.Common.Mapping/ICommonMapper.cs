using System;

namespace Peritos.Common.Mapping
{
    /// <summary>
    /// Object Mapping interface
    /// </summary>
    public interface ICommonMapper
    {
        /// <summary>
        /// Maps object to Type parameter
        /// </summary>
        /// <typeparam name="TOut">Mapped Typed object</typeparam>
        /// <param name="o">Object to map</param>
        /// <returns>Mapped Object</returns>
        TOut Map<TOut>(object o);

        /// <summary>
        /// Maps Type parameter to Type parameter
        /// </summary>
        /// <typeparam name="TIn">Typed object to map</typeparam>
        /// <typeparam name="TOut">Mapped Typed object</typeparam>
        /// <param name="o">Object To map</param>
        /// <returns>Mapped Object</returns>
        TOut Map<TIn, TOut>(TIn o);

        /// <summary>
        /// Maps an object to an Object type with a Resolution Context. 
        /// </summary>
        /// <typeparam name="TOut">TypeObject output</typeparam>
        /// <param name="o">Object to map</param>
        /// <param name="o">Context</param>
        /// <returns>Mapped Object</returns>
        TOut Map<TOut>(object o, object context);

        /// <summary>
        /// Maps Type parameter to Type Parameter
        /// </summary>
        /// <typeparam name="TIn">Typed object to map</typeparam>
        /// <typeparam name="TOut">Mapped Typed object</typeparam>
        /// <param name="source">Source object to map</param>
        /// <param name="destination">Target object map</param>
        /// <returns>Mapped Object</returns>
        TOut Map<TIn, TOut>(TIn source, TOut destination);

        /// <summary>
        /// Maps Object to Object based on object type to object type
        /// </summary>
        /// <param name="source">Source object to map</param>
        /// <param name="sourceType">Source type</param>
        /// <param name="destinationType">Destination object type</param>
        /// <returns>Mapped Object</returns>
        object Map(object source, Type sourceType, Type destinationType);
    }
}
