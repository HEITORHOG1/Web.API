﻿using Microsoft.EntityFrameworkCore;
using Web.Domain.Paginacao;

namespace Web.Application.Extensions
{
    public static class IQueryableExtensions
    {
        public static async Task<PagedResult<T>> GetPagedAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
        }
    }
}