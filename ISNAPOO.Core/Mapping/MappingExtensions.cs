using AutoMapper.QueryableExtensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ISNAPOO.Core.Mapping
{
    public static class MappingExtensions
    {
        public static IQueryable<TDestination> To<TDestination>(
            this IQueryable source,
            params Expression<Func<TDestination, object>>[] membersToExpand)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.ProjectTo(Config.Mapper.ConfigurationProvider, null, membersToExpand);
        }

        public static IQueryable<TDestination> To<TDestination>(
            this IQueryable source,
            object parameters)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.ProjectTo<TDestination>(Config.Mapper.ConfigurationProvider, parameters);
        }

        public static TDestination To<TDestination>(this object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Config.Mapper.Map<TDestination>(source);
        }

        public static TDestination To<TSource, TDestination>(this object source, TSource dbSource, TDestination destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Config.Mapper.Map(dbSource, destination);
        }

        public static TDestination To<TSource, TDestination>(this TSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Config.Mapper.Map<TSource, TDestination>(source);
        }
    }
}
