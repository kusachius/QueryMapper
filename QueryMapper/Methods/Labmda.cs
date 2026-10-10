using System.Linq.Expressions;
using System.Reflection;

namespace Kusachius.QueryMapper
{
    public partial class QueryMapper<TSource> where TSource : new()
    {
        private string LambdaToSql(Expression expr, out string memberName, bool unsafeSql = false)
        {
            memberName = null;

            string sql;
            switch (expr.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.LessThan:
                    var be = expr as BinaryExpression;
                    var beLeft = GetConvertedIfNeeded(be.Left);
                    var beRight = GetConvertedIfNeeded(be.Right);
                    var left = LambdaToSql(beLeft, out _);
                    var right = LambdaToSql(beRight, out _);
                    var op = LambdaExprSql(be.NodeType);
                    if (right == "NULL")
                    {
                        switch (op.Trim())
                        {
                            case "=":
                                op = " IS ";
                                break;

                            case "<>":
                                op = " IS NOT ";
                                break;

                            default:
                                throw new InvalidOperationException($"Invalid opetation for null: {op}");
                        }
                    }
                    if (expr.NodeType == ExpressionType.OrElse)
                    {
                        sql = $"({left}{op}{right})";
                    }
                    else
                    {
                        sql = $"{left}{op}{right}";
                    }
                    break;

                case ExpressionType.MemberAccess:
                    var me = expr as MemberExpression;
                    switch (me.Member.MemberType)
                    {
                        case MemberTypes.Property:
                            if (me.Expression.NodeType == ExpressionType.Parameter || me.Expression.NodeType == ExpressionType.TypeAs)
                            {
                                var mee = GetConvertedIfNeeded(me.Expression);
                                var alias = LambdaToSql(mee, out _);
                                var col = GetTableInfo(mee.Type).GetColumnName(me.Member.Name);
                                memberName = col;
                                sql = $"{alias}.{col}";
                            }
                            else if (me.Expression.NodeType == ExpressionType.Constant)
                            {
                                var pi = me.Member as PropertyInfo;
                                var mval = pi.GetValue((me.Expression as ConstantExpression).Value);
                                var mSql = LambdaGetConstantSql(mval);
                                sql = $"{mSql}";
                            }
                            else
                            {
                                throw new ArgumentException($"Member expression type {me.Expression.NodeType} is not supported");
                            }
                            break;

                        case MemberTypes.Field:
                            var fi = me.Member as FieldInfo;
                            object val;
                            if (me.Expression is ConstantExpression mec)
                            {
                                val = fi.GetValue(mec.Value);
                            }
                            else if (me.Expression is MemberExpression mef && mef.Expression is ConstantExpression mefc)
                            {
                                var fi2 = mef.Member as FieldInfo;
                                var obj2 = fi2.GetValue(mefc.Value);
                                val = fi.GetValue(obj2);
                            }
                            else
                            {
                                throw new ArgumentException($"Member type {me.Member.MemberType} is not supported");
                            }
                            sql = LambdaGetConstantSql(val, unsafeSql);
                            break;

                        default:
                            throw new ArgumentException($"Member type {me.Member.MemberType} is not supported");
                    }
                    break;

                case ExpressionType.Parameter:
                    var pe = expr as ParameterExpression;
                    sql = pe.Name;
                    break;

                case ExpressionType.Constant:
                    var ce = expr as ConstantExpression;
                    sql = LambdaGetConstantSql(ce.Value, unsafeSql);
                    break;

                case ExpressionType.Convert:
                    var ue = expr as UnaryExpression;
                    if (ue.Type == typeof(RawSql))
                    {
                        sql = LambdaToSql(ue.Operand, out _, true);
                    }
                    else
                    {
                        sql = LambdaToSql(ue.Operand, out _, true);
                        // throw new ArgumentException($"Node type {expr.NodeType} is not supported (convert)");
                    }
                    break;

                case ExpressionType.Negate:
                    var ue1 = expr as UnaryExpression;
                    sql = $"-({LambdaToSql(ue1.Operand, out _, true)})";
                    break;

                case ExpressionType.Add:
                case ExpressionType.Subtract:
                case ExpressionType.Multiply:
                case ExpressionType.Divide:
                    var be1 = expr as BinaryExpression;
                    var beLeft1 = GetConvertedIfNeeded(be1.Left);
                    var beRight1 = GetConvertedIfNeeded(be1.Right);
                    var left1 = LambdaToSql(beLeft1, out _);
                    var right1 = LambdaToSql(beRight1, out _);
                    var op1 = LambdaExprSql(be1.NodeType);
                    if (beLeft1.Type == typeof(string))
                    {
                        if (expr.NodeType == ExpressionType.Add)
                        {
                            sql = $"CONCAT({left1}, {right1})";
                        }
                        else
                        {
                            throw new ArgumentException($"Node type {expr.NodeType} is not supported for string");
                        }
                    }
                    else
                    {
                        sql = $"({left1}{op1}{right1})";
                    }
                    break;

                case ExpressionType.Call:
                    var mce = expr as MethodCallExpression;
                    if (mce.Method.Name == "GetValueOrDefault")
                    {
                        if (mce.Arguments.Count() > 0)
                        {
                            throw new ArgumentException($"GetValueOrDefault supports only 0 arguments");
                        }
                        if (mce.Object.Type == typeof(Nullable<int>) || mce.Object.Type == typeof(Nullable<decimal>))
                        {
                            sql = $"COALESCE({LambdaToSql(mce.Object, out _)}, 0)";
                        }
                        else
                        {
                            throw new ArgumentException($"GetValueOrDefault not supported for type {mce.Object.Type}");
                        }
                    }
                    else if (mce.Method.Name == "ToString")
                    {
                        if (mce.Arguments.Count() == 0)
                        {
                            sql = $"({LambdaToSql(mce.Object, out _)})::text";
                        }
                        else if (mce.Arguments.Count() == 1)
                        {
                            var fmt = LambdaToSql(mce.Arguments[0], out _, true);
                            if (fmt == "0" || fmt == "00" || fmt == "000" || fmt == "0000" || fmt == "00000" || fmt == "000000" || fmt == "0000000")
                            {
                                sql = $"to_char({LambdaToSql(mce.Object, out _)}, 'fm{fmt}')";
                            }
                            else
                            {
                                throw new ArgumentException($"ToString supports only format with zeroes");
                            }
                        }
                        else
                        {
                            throw new ArgumentException($"ToString supports only 0 or 1 arguments");
                        }
                    }
                    else if (mce.Method.Name == "Replace")
                    {
                        sql = $"REPLACE({LambdaToSql(mce.Object, out _)}, {LambdaToSql(mce.Arguments[0], out _)}, {LambdaToSql(mce.Arguments[1], out _)})";
                    }
                    else if (mce.Method.Name == "AddDays")
                    {
                        sql = $"({LambdaToSql(mce.Object, out _)} + INTERVAL '{LambdaToSql(mce.Arguments[0], out _)} DAY')";
                    }
                    else
                    {
                        var aggExpr = mce.Method.Name switch
                        {
                            "Sum" => "SUM",
                            "Count" => "COUNT",
                            "Min" => "MIN",
                            "Max" => "MAX",
                            "Round" => "ROUND",
                            _ => throw new ArgumentException($"Unsupported function: {mce.Method.Name}")
                        };
                        if (mce.Arguments.Count() == 1)
                        {
                            sql = $"{aggExpr}({LambdaToSql(mce.Arguments[0], out _)})";
                        }
                        else if (mce.Arguments.Count() == 2)
                        {
                            sql = $"{aggExpr}({LambdaToSql(mce.Arguments[0], out _)}, {LambdaToSql(mce.Arguments[1], out _)})";
                        }
                        else
                        {
                            throw new ArgumentException($"Unsupported number of arguments: {mce.Arguments.Count()} for function {mce.Method.Name}");
                        }
                    }
                    break;

                case ExpressionType.Conditional:
                    var cnde = expr as ConditionalExpression;
                    var cndeTest = LambdaToSql(cnde.Test, out _);
                    var cndeTrue = LambdaToSql(cnde.IfTrue, out _);
                    var cndeFalse = LambdaToSql(cnde.IfFalse, out _);

                    sql = $"CASE WHEN {cndeTest} THEN {cndeTrue} ELSE {cndeFalse} END";
                    break;

                case ExpressionType.Not:
                    var nue = expr as UnaryExpression;
                    sql = $"NOT ({LambdaToSql(nue.Operand, out _, true)})";
                    break;

                case ExpressionType.Coalesce:
                    var cue = expr as BinaryExpression;
                    sql = $"COALESCE({LambdaToSql(cue.Left, out _, true)}, {LambdaToSql(cue.Right, out _)})";
                    break;

                default:
                    throw new ArgumentException($"Node type {expr.NodeType} is not supported");
            }

            return sql;
        }

        private string LambdaGetConstantSql(object obj, bool unsafeSql = false)
        {
            string sql;
            if (obj is null)
            {
                sql = "NULL";
            }
            else if (obj is int || obj is decimal || obj is float)
            {
                sql = $"{obj}";
            }
            else if (obj is bool)
            {
                sql = $"{obj}";
            }
            else if (obj.GetType().IsEnum)
            {
                sql = $"{(int)obj}";
            }
            else if (unsafeSql)
            {
                sql = $"{obj}";
            }
            else
            {
                _params.Add(obj);
                sql = $"{{{_params.Count() - 1}}}";
            }

            return sql;
        }

        private string LambdaExprSql(ExpressionType exprType)
        {
            switch (exprType)
            {
                case ExpressionType.Equal:
                    return " = ";

                case ExpressionType.NotEqual:
                    return " <> ";

                case ExpressionType.AndAlso:
                    return "\n\tAND ";

                case ExpressionType.OrElse:
                    return " OR ";

                case ExpressionType.GreaterThanOrEqual:
                    return " >= ";

                case ExpressionType.GreaterThan:
                    return " > ";

                case ExpressionType.LessThanOrEqual:
                    return " <= ";

                case ExpressionType.LessThan:
                    return " < ";

                case ExpressionType.Add:
                    return " + ";

                case ExpressionType.Subtract:
                    return " - ";

                case ExpressionType.Multiply:
                    return " * ";

                case ExpressionType.Divide:
                    return " / ";
            }

            throw new ArgumentException($"Unsupported expresision type: {exprType}");
        }

        private Expression GetConvertedIfNeeded(Expression expr)
        {
            if (expr is UnaryExpression ue && (ue.NodeType == ExpressionType.Convert || ue.NodeType == ExpressionType.TypeAs))
            {
                return ue.Operand as Expression;
            }

            return expr;
        }
    }
}
