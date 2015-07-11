using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using SharpFileDB.Blocks;
using SharpFileDB.Utilities;
using System.Reflection;
using System.Linq.Expressions;

namespace SharpFileDB
{
    /// <summary>
    /// 单文件数据库上下文，代表一个单文件数据库。SharpFileDB的核心类型。
    /// </summary>
    public partial class FileDBContext : IDisposable
    {

        /// <summary>
        /// 查找数据库内的某些记录。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">符合此条件的记录会被取出。</param>
        /// <returns></returns>
        public IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : Table, new()
        {
            if (predicate == null) { throw new ArgumentNullException("predicate"); }

            // 这是没有利用索引的版本。
            Func<T, bool> func = predicate.Compile();
            foreach (T item in this.FindAll<T>())
            {
                if(func(item))
                {
                    yield return item;
                }
            }

            // TODO: 这是利用索引的版本，尚未实现。
            //List<T> result = new List<T>();

            //var body = predicate.Body as LambdaExpression;
            //this.Find(result, body);

            //return result;
        }

        /// <summary>
        /// 分析表达式，利用索引来查找。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="expr"></param>
        private void Find<T>(List<T> result, Expression expr)where T : Table, new()
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Add://
                    break;
                case ExpressionType.AddAssign://
                    break;
                case ExpressionType.AddAssignChecked://
                    break;
                case ExpressionType.AddChecked://
                    break;
                case ExpressionType.And://
                    break;
                case ExpressionType.AndAlso:
                    {
                        List<T> leftResult = new List<T>();
                        BinaryExpression exp = expr as BinaryExpression;
                        Find(leftResult, exp.Left);
                        if (leftResult.Count > 0)
                        {
                            List<T> rightResult = new List<T>();
                            Find(rightResult, exp.Right);
                            foreach (var left in leftResult)
                            {
                                foreach (var right in rightResult)
                                {
                                    if (left.Id == right.Id)
                                    {
                                        result.Add(left);
                                        break;
                                    }


                                }
                            }
                        }
                    }
                    break;
                case ExpressionType.AndAssign://
                    break;
                case ExpressionType.ArrayIndex://
                    break;
                case ExpressionType.ArrayLength:
                    break;
                case ExpressionType.Assign:
                    break;
                case ExpressionType.Block:
                    break;
                case ExpressionType.Call:
                    break;
                case ExpressionType.Coalesce:
                    break;
                case ExpressionType.Conditional:
                    break;
                case ExpressionType.Constant:
                    break;
                case ExpressionType.Convert:
                    break;
                case ExpressionType.ConvertChecked:
                    break;
                case ExpressionType.DebugInfo:
                    break;
                case ExpressionType.Decrement:
                    break;
                case ExpressionType.Default:
                    break;
                case ExpressionType.Divide:
                    break;
                case ExpressionType.DivideAssign:
                    break;
                case ExpressionType.Dynamic:
                    break;
                case ExpressionType.Equal:
                    {
                        var bin = expr as BinaryExpression;
                        //TODO:
                    }
                    break;
                case ExpressionType.ExclusiveOr:
                    break;
                case ExpressionType.ExclusiveOrAssign:
                    break;
                case ExpressionType.Extension:
                    break;
                case ExpressionType.Goto:
                    break;
                case ExpressionType.GreaterThan:
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    break;
                case ExpressionType.Increment:
                    break;
                case ExpressionType.Index:
                    break;
                case ExpressionType.Invoke:
                    break;
                case ExpressionType.IsFalse:
                    break;
                case ExpressionType.IsTrue:
                    break;
                case ExpressionType.Label:
                    break;
                case ExpressionType.Lambda:
                    break;
                case ExpressionType.LeftShift:
                    break;
                case ExpressionType.LeftShiftAssign:
                    break;
                case ExpressionType.LessThan:
                    break;
                case ExpressionType.LessThanOrEqual:
                    break;
                case ExpressionType.ListInit:
                    break;
                case ExpressionType.Loop:
                    break;
                case ExpressionType.MemberAccess:
                    break;
                case ExpressionType.MemberInit:
                    break;
                case ExpressionType.Modulo:
                    break;
                case ExpressionType.ModuloAssign:
                    break;
                case ExpressionType.Multiply:
                    break;
                case ExpressionType.MultiplyAssign:
                    break;
                case ExpressionType.MultiplyAssignChecked:
                    break;
                case ExpressionType.MultiplyChecked:
                    break;
                case ExpressionType.Negate:
                    break;
                case ExpressionType.NegateChecked:
                    break;
                case ExpressionType.New:
                    break;
                case ExpressionType.NewArrayBounds:
                    break;
                case ExpressionType.NewArrayInit:
                    break;
                case ExpressionType.Not:
                    break;
                case ExpressionType.NotEqual:
                    break;
                case ExpressionType.OnesComplement:
                    break;
                case ExpressionType.Or:
                    break;
                case ExpressionType.OrAssign:
                    break;
                case ExpressionType.OrElse:
                    {
                        BinaryExpression exp = expr as BinaryExpression;
                        Find(result, exp.Left);
                        Find(result, exp.Right);
                    }
                    break;
                case ExpressionType.Parameter:
                    break;
                case ExpressionType.PostDecrementAssign:
                    break;
                case ExpressionType.PostIncrementAssign:
                    break;
                case ExpressionType.Power:
                    break;
                case ExpressionType.PowerAssign:
                    break;
                case ExpressionType.PreDecrementAssign:
                    break;
                case ExpressionType.PreIncrementAssign:
                    break;
                case ExpressionType.Quote:
                    break;
                case ExpressionType.RightShift:
                    break;
                case ExpressionType.RightShiftAssign:
                    break;
                case ExpressionType.RuntimeVariables:
                    break;
                case ExpressionType.Subtract:
                    break;
                case ExpressionType.SubtractAssign:
                    break;
                case ExpressionType.SubtractAssignChecked:
                    break;
                case ExpressionType.SubtractChecked:
                    break;
                case ExpressionType.Switch:
                    break;
                case ExpressionType.Throw:
                    break;
                case ExpressionType.Try:
                    break;
                case ExpressionType.TypeAs:
                    break;
                case ExpressionType.TypeEqual:
                    break;
                case ExpressionType.TypeIs:
                    break;
                case ExpressionType.UnaryPlus:
                    break;
                case ExpressionType.Unbox:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 查找数据库内所有指定类型的记录。
        /// </summary>
        /// <typeparam name="T">要查找的类型。</typeparam>
        /// <returns></returns>
        public IEnumerable<T> FindAll<T>() where T:Table, new()
        {
            Type type = typeof(T);
            if (this.tableBlockDict.ContainsKey(type))
            {
                TableBlock tableBlock = this.tableBlockDict[type];
                IndexBlock firstIndex = tableBlock.IndexBlockHead.NextObj;// 第一个索引应该是Table.Id的索引。
                FileStream fs = this.fileStream;

                SkipListNodeBlock current = firstIndex.SkipListHeadNodes[0]; //currentHeadNode;

                while (current.RightPos != firstIndex.SkipListTailNodePos)
                {
                    current.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.RightObj);
                    current.RightObj.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.RightObj | SkipListNodeBlockLoadOptions.Value);
                    T item = current.RightObj.Value.GetObject<T>(this);

                    yield return item;

                    current = current.RightObj;
                }
            }
        }

        ///// <summary>
        ///// 查找数据库内所有指定类型的记录。
        ///// </summary>
        ///// <typeparam name="T">要查找的类型。</typeparam>
        ///// <returns></returns>
        //public IList<T> FindAll<T>() where T : Table, new()
        //{
        //    List<T> result = new List<T>();

        //    Type type = typeof(T);
        //    if (this.tableBlockDict.ContainsKey(type))
        //    {
        //        TableBlock tableBlock = this.tableBlockDict[type];
        //        IndexBlock firstIndex = tableBlock.IndexBlockHead.NextObj;// 第一个索引应该是Table.Id的索引。
        //        //long currentHeadNodePos = firstIndex.SkipListHeadNodePos;
        //        //if (currentHeadNodePos <= 0)
        //        //{ throw new Exception("DB Error: There is no skip list node head stored!"); }
        //        FileStream fs = this.fileStream;
        //        //SkipListNodeBlock currentHeadNode = fs.ReadBlock<SkipListNodeBlock>(currentHeadNodePos);
        //        //currentHeadNode.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.DownObj);
        //        //while (currentHeadNode.DownObj != null)
        //        //{
        //        //    currentHeadNode.DownObj.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.DownObj);
        //        //    currentHeadNode = currentHeadNode.DownObj;
        //        //}

        //        SkipListNodeBlock current = firstIndex.SkipListHeadNodes[0]; //currentHeadNode;

        //        while (current.RightPos != firstIndex.SkipListTailNodePos)
        //        {
        //            current.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.RightObj);
        //            //if (current.RightObj.RightPos == 0)
        //            //{ break; }
        //            current.RightObj.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.RightObj | SkipListNodeBlockLoadOptions.Value);
        //            T item = current.RightObj.Value.GetObject<T>(this);
        //            result.Add(item);

        //            current = current.RightObj;
        //        }
        //    }

        //    return result;
        //}

    }
}
