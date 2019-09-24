Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks

Namespace eShopDashboard.Queries
	Public Module OrderingQueriesText
		Public Function ProductHistory(productId As String) As String
'INSTANT VB WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: var sqlCommandText = $"TangibleTempVerbatimOpenTagTangibleTempVerbatimStringLiteralLineJoinselect p.productId, p.[year], p.[month], p.units, p.[avg], p.[count], p.[max], p.[min],TangibleTempVerbatimStringLiteralLineJoin    LAG (units, 1) OVER (PARTITION BY p.productId ORDER BY p.productId, p.date) as prev,TangibleTempVerbatimStringLiteralLineJoin    LEAD (units, 1) OVER (PARTITION BY p.productId ORDER BY p.productId, p.date) as [next]TangibleTempVerbatimStringLiteralLineJoinfrom (TangibleTempVerbatimStringLiteralLineJoin    select oi.ProductId as productId, TangibleTempVerbatimStringLiteralLineJoin        YEAR(CAST(oi.OrderDate as datetime)) as [year], TangibleTempVerbatimStringLiteralLineJoin        MONTH(CAST(oi.OrderDate as datetime)) as [month], TangibleTempVerbatimStringLiteralLineJoin        MIN(CAST(oi.OrderDate as datetime)) as date,TangibleTempVerbatimStringLiteralLineJoin        sum(oi.Units) as units,TangibleTempVerbatimStringLiteralLineJoin        avg(oi.Units) as [avg],TangibleTempVerbatimStringLiteralLineJoin        count(oi.Units) as [count],TangibleTempVerbatimStringLiteralLineJoin        max(oi.Units) as [max],TangibleTempVerbatimStringLiteralLineJoin        min(oi.Units) as [min]TangibleTempVerbatimStringLiteralLineJoin    from (TangibleTempVerbatimStringLiteralLineJoin        select CONVERT(date, oo.OrderDate) as OrderDate, oi.ProductId, sum(oi.Units) as unitsTangibleTempVerbatimStringLiteralLineJoin        from [ordering].[orderItems] oiTangibleTempVerbatimStringLiteralLineJoin        inner join [ordering].[orders] oo on oi.OrderId = oo.IdTangibleTempVerbatimStringLiteralLineJoin        {(string.IsNullOrEmpty(productId) ? string.Empty : TangibleTempVerbatimCloseTag"where oi.ProductId = @productId")} group by CONVERT(date, oo.OrderDate), oi.ProductId) as oi group by oi.ProductId, YEAR(CAST(oi.OrderDate as datetime)), MONTH(CAST(oi.OrderDate as datetime)) ) as p";
			$"
select p.productId, p.[year], p.[month], p.units, p.[avg], p.[count], p.[max], p.[min],
    LAG (units, 1) OVER (PARTITION BY p.productId ORDER BY p.productId, p.date) as prev,
    LEAD (units, 1) OVER (PARTITION BY p.productId ORDER BY p.productId, p.date) as [next]
from (
    select oi.ProductId as productId, 
        YEAR(CAST(oi.OrderDate as datetime)) as [year], 
        MONTH(CAST(oi.OrderDate as datetime)) as [month], 
        MIN(CAST(oi.OrderDate as datetime)) as date,
        sum(oi.Units) as units,
        avg(oi.Units) as [avg],
        count(oi.Units) as [count],
        max(oi.Units) as [max],
        min(oi.Units) as [min]
    from (
        select CONVERT(date, oo.OrderDate) as OrderDate, oi.ProductId, sum(oi.Units) as units
        from [ordering].[orderItems] oi
        inner join [ordering].[orders] oo on oi.OrderId = oo.Id
        {(string.IsNullOrEmpty(productId) ? string.Empty : "where oi.ProductId = productId")} group by CONVERT(date, oo.OrderDate), oi.ProductId) as oi group by oi.ProductId, YEAR(CAST(oi.OrderDate as datetime)), MONTH(CAST(oi.OrderDate as datetime)) ) as p"
			Dim sqlCommandText As Dim = $"
select p.productId, p.[year], p.[month], p.units, p.[avg], p.[count], p.[max], p.[min],
    LAG (units, 1) OVER (PARTITION BY p.productId ORDER BY p.productId, p.date) as prev,
    LEAD (units, 1) OVER (PARTITION BY p.productId ORDER BY p.productId, p.date) as [next]
from (
    select oi.ProductId as productId, 
        YEAR(CAST(oi.OrderDate as datetime)) as [year], 
        MONTH(CAST(oi.OrderDate as datetime)) as [month], 
        MIN(CAST(oi.OrderDate as datetime)) as date,
        sum(oi.Units) as units,
        avg(oi.Units) as [avg],
        count(oi.Units) as [count],
        max(oi.Units) as [max],
        min(oi.Units) as [min]
    from (
        select CONVERT(date, oo.OrderDate) as OrderDate, oi.ProductId, sum(oi.Units) as units
        from [ordering].[orderItems] oi
        inner join [ordering].[orders] oo on oi.OrderId = oo.Id
        {(string.IsNullOrEmpty(productId) ? string.Empty : "where oi.ProductId

			Return sqlCommandText
		End Function

		Public Function CountryHistory(country As String) As String
'INSTANT VB WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: var sqlCommandText = $"TangibleTempVerbatimOpenTagTangibleTempVerbatimStringLiteralLineJoinselect TangibleTempVerbatimStringLiteralLineJoin    LEAD (log10(sum(R.sale)), 1) OVER (PARTITION BY R.country ORDER BY R.[year], R.[month]) as [next],TangibleTempVerbatimStringLiteralLineJoin	R.country, R.year, R.month, sum(R.sale) as sales, count(R.sale) as count, TangibleTempVerbatimStringLiteralLineJoin    max(R.p_max) as [max], min(R.p_med) as [med], min(R.p_min) as [min], stdevp(R.sale) as std,TangibleTempVerbatimStringLiteralLineJoin    LAG (sum(R.sale), 1) OVER (PARTITION BY R.country ORDER BY R.[year], R.[month]) as prevTangibleTempVerbatimStringLiteralLineJoinfrom (TangibleTempVerbatimStringLiteralLineJoin    select S.country, S.[month], S.[year], S.sale,TangibleTempVerbatimStringLiteralLineJoin        PERCENTILE_CONT(0.20) WITHIN GROUP (ORDER BY S.sale) OVER (PARTITION BY S.country, S.[year], S.[month]) as p_min,TangibleTempVerbatimStringLiteralLineJoin        PERCENTILE_CONT(0.50) WITHIN GROUP (ORDER BY S.sale) OVER (PARTITION BY S.country, S.[year], S.[month]) as p_med,TangibleTempVerbatimStringLiteralLineJoin        PERCENTILE_CONT(0.80) WITHIN GROUP (ORDER BY S.sale) OVER (PARTITION BY S.country, S.[year], S.[month]) as p_maxTangibleTempVerbatimStringLiteralLineJoin        from TangibleTempVerbatimStringLiteralLineJoin        (select min(T.country) as country, min(T.year) as [year], min(T.month) as [month], sum(T.sale) as saleTangibleTempVerbatimStringLiteralLineJoin            from (TangibleTempVerbatimStringLiteralLineJoin            select oo.Address_Country as country, oo.Id as id, YEAR(oo.OrderDate) as [year], MONTH(oo.OrderDate) as [month], oi.UnitPrice * oi.Units as saleTangibleTempVerbatimStringLiteralLineJoin            from [ordering].[orderItems] oiTangibleTempVerbatimStringLiteralLineJoin            inner join [ordering].[orders] oo on oi.OrderId = oo.Id {(string.IsNullOrEmpty(country) ? string.Empty : TangibleTempVerbatimCloseTag"and oo.Address_Country = (@country)")} ) as T group by T.id ) as S ) as R group by R.country, R.year, R.month order by R.country, R.year, R.month";
			$"
select 
    LEAD (log10(sum(R.sale)), 1) OVER (PARTITION BY R.country ORDER BY R.[year], R.[month]) as [next],
	R.country, R.year, R.month, sum(R.sale) as sales, count(R.sale) as count, 
    max(R.p_max) as [max], min(R.p_med) as [med], min(R.p_min) as [min], stdevp(R.sale) as std,
    LAG (sum(R.sale), 1) OVER (PARTITION BY R.country ORDER BY R.[year], R.[month]) as prev
from (
    select S.country, S.[month], S.[year], S.sale,
        PERCENTILE_CONT(0.20) WITHIN GROUP (ORDER BY S.sale) OVER (PARTITION BY S.country, S.[year], S.[month]) as p_min,
        PERCENTILE_CONT(0.50) WITHIN GROUP (ORDER BY S.sale) OVER (PARTITION BY S.country, S.[year], S.[month]) as p_med,
        PERCENTILE_CONT(0.80) WITHIN GROUP (ORDER BY S.sale) OVER (PARTITION BY S.country, S.[year], S.[month]) as p_max
        from 
        (select min(T.country) as country, min(T.year) as [year], min(T.month) as [month], sum(T.sale) as sale
            from (
            select oo.Address_Country as country, oo.Id as id, YEAR(oo.OrderDate) as [year], MONTH(oo.OrderDate) as [month], oi.UnitPrice * oi.Units as sale
            from [ordering].[orderItems] oi
            inner join [ordering].[orders] oo on oi.OrderId = oo.Id {(string.IsNullOrEmpty(country) ? string.Empty : "[and] oo.Address_Country = CType(")} ) as T group by T.id ) as S ) as R group by R.country, R.year, R.month order by R.country, R.year, R.month", country)
			Dim sqlCommandText As Dim = $"
select 
    LEAD (log10(sum(R.sale)), 1) OVER (PARTITION BY R.country ORDER BY R.[year], R.[month]) as [next],
	R.country, R.year, R.month, sum(R.sale) as sales, count(R.sale) as count, 
    max(R.p_max) as [max], min(R.p_med) as [med], min(R.p_min) as [min], stdevp(R.sale) as std,
    LAG (sum(R.sale), 1) OVER (PARTITION BY R.country ORDER BY R.[year], R.[month]) as prev
from (
    select S.country, S.[month], S.[year], S.sale,
        PERCENTILE_CONT(0.20) WITHIN GROUP (ORDER BY S.sale) OVER (PARTITION BY S.country, S.[year], S.[month]) as p_min,
        PERCENTILE_CONT(0.50) WITHIN GROUP (ORDER BY S.sale) OVER (PARTITION BY S.country, S.[year], S.[month]) as p_med,
        PERCENTILE_CONT(0.80) WITHIN GROUP (ORDER BY S.sale) OVER (PARTITION BY S.country, S.[year], S.[month]) as p_max
        from 
        (select min(T.country) as country, min(T.year) as [year], min(T.month) as [month], sum(T.sale) as sale
            from (
            select oo.Address_Country as country, oo.Id as id, YEAR(oo.OrderDate) as [year], MONTH(oo.OrderDate) as [month], oi.UnitPrice * oi.Units as sale
            from [ordering].[orderItems] oi
            inner join [ordering].[orders] oo on oi.OrderId = oo.Id {(string.IsNullOrEmpty(country) ? string.Empty : "[and] oo.Address_Country

			Return sqlCommandText
		End Function
	End Module
End Namespace
