﻿using System.Linq;

using Bet.CleanArchitecture.Core.Entities;
using Bet.CleanArchitecture.Core.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Bet.Blog.Data
{
    public static class SpecificationEvaluator<T, TKey> where T : BaseEntity<TKey>
    {
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
#pragma warning restore CA1000 // Do not declare static members on generic types
        {
            var query = inputQuery;

            // modify the IQueryable using the specification's criteria expression
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Includes all expression-based includes
            query = specification.Includes.Aggregate(
                query,
                (current, include) => current.Include(include));

            // Include any string-based include statements
            query = specification.IncludeStrings.Aggregate(
                query,
                (current, include) => current.Include(include));

            // Apply ordering if expressions are set
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            // Apply paging if enabled
            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip)
                             .Take(specification.Take);
            }

            return query;
        }
    }
}
