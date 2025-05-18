using AutoMapper;
using System;

namespace Peritos.Common.Mapping
{
    /// <summary>
    /// Mapper class that will handle almost any type of object mapping
    /// </summary>
    public class CommonMapper : ICommonMapper
    {
        /// <summary>
        /// Mapper Configuration Object
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonMapper"/> class.
        /// </summary>
        /// <param name="profiles">Profiles to map</param>
        public CommonMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Maps an object to an Object type
        /// </summary>
        /// <typeparam name="TOut">TypeObject output</typeparam>
        /// <param name="o">Object to map</param>
        /// <returns>Mapped Object</returns>
        public TOut Map<TOut>(object o)
        {
            return this._mapper.Map<TOut>(o);
        }


        /// <summary>
        /// Maps an object to an Object type with a Resolution Context. 
        /// </summary>
        /// <typeparam name="TOut">TypeObject output</typeparam>
        /// <param name="o">Object to map</param>
        /// <param name="o">Context</param>
        /// <returns>Mapped Object</returns>
        public TOut Map<TOut>(object o, object context)
        {
            return this._mapper.Map<TOut>(o, opts =>
            {
                foreach (var item in context.GetType().GetProperties())
                {
                    opts.Items[item.Name] = item.GetValue(context);
                }
            });
        }

        /// <summary>
        /// Maps an object of Type to an Object type
        /// </summary>
        /// <typeparam name="TIn">TypeObject input</typeparam>
        /// <typeparam name="TOut">TypeObject output</typeparam>
        /// <param name="o">Object to map</param>
        /// <returns>Mapped object</returns>
        public TOut Map<TIn, TOut>(TIn o)
        {
            return this._mapper.Map<TIn, TOut>(o);
        }

        /// <summary>
        /// Mapping method using two objects as parameters
        /// </summary>
        /// <typeparam name="TIn">TypeObject input</typeparam>
        /// <typeparam name="TOut">TypeObject output</typeparam>
        /// <param name="source">Map Source</param>
        /// <param name="destination">Mapping target</param>
        /// <returns>Returns a Mapped object</returns>
        public TOut Map<TIn, TOut>(TIn source, TOut destination)
        {
            return this._mapper.Map(source, destination);
        }

        /// <summary>
        /// Maps Object to Object based on object type to object type
        /// </summary>
        /// <param name="source">Source object to map</param>
        /// <param name="sourceType">Source type</param>
        /// <param name="destinationType">Destination object type</param>
        /// <returns>Mapped Object</returns>
        public object Map(object source, Type sourceType, Type destinationType)
        {
            return this._mapper.Map(source, sourceType, destinationType);
        }
    }
}
