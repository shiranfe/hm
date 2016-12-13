using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common.Helpers;

namespace Common
{
    public static class LinqHelpers
    {
        //public static Expression<Func<T, bool>> AndAlso<T>( this Expression<Func<T, bool>> left,Expression<Func<T, bool>> right)
        //{
        //    var param = Expression.Parameter(typeof(T), "x");
        //    var body = Expression.AndAlso(
        //            Expression.Invoke(left, param),
        //            Expression.Invoke(right, param)
        //        );
        //    var lambda = Expression.Lambda<Func<T, bool>>(body, param);
        //    return lambda;
        //}

        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {
            var p = a.Parameters[0];

            var visitor = new SubstExpressionVisitor {subst = {[b.Parameters[0]] = p}};

            Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {
            var p = a.Parameters[0];

            var visitor = new SubstExpressionVisitor {subst = {[b.Parameters[0]] = p}};

            Expression body = Expression.OrElse(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        internal class SubstExpressionVisitor : ExpressionVisitor
        {
            public Dictionary<Expression, Expression> subst = new Dictionary<Expression, Expression>();

            protected override Expression VisitParameter(ParameterExpression node)
            {
                Expression newValue;
                if (subst.TryGetValue(node, out newValue))
                {
                    return newValue;
                }
                return node;
            }
        }


        public static List<T> FilterByPage<T>(Pager filter, IEnumerable<T> list)
        {
            var rows = 50;

            if (filter.PageTotal == 0)
                filter.PageTotal = (list.Count() + rows - 1) / rows;

            if (filter.PageTotal > 1)
                return list.Skip( (filter.Page - 1) * rows).Take(rows).ToList();

            return list.ToList();
        }


        //public static T SingleOrDefault<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        //{
        //    return list.Where(predicate).SingleOrDefault();
        //}

        //public static T Single<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        //{
        //    var ans = list.Where(predicate).SingleOrDefault();
        //    if (ans == null)
        //        throw ErrorHelper.EntityMissing("entity doesn exist " + typeof(T).Name);

        //    return ans;
        //}


        //public static T SingleOrDefault<T>(this IEnumerable<T> enumerable)
        //{

        //    using (var e = enumerable.GetEnumerator())
        //    {
        //        /** move to 1st element and check if exist */
        //        if (!e.MoveNext())
        //            return default(T);

        //        /** move to 2nd element and check if exist */
        //        if (!e.MoveNext())
        //            return enumerable.Single();


        //        throw ErrorHelper.Conflict("more then one " + typeof(T).Name);
        //    }

        //}





    }

    //public static class  HtmlRes
    //{
    //    public static string GetString(string key)
    //    {
    //        try
    //        {
    //            return new ResourceManager("Common.Resources.Global", Assembly.GetExecutingAssembly())
    //                .GetString(key);
    //        }
    //        catch (Exception)
    //        {

    //            throw;
    //        }


    //    }


    //}
}