using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lox
{
    public abstract class Expr
    {
        public interface Visitor<R>
        {
            public R visitBinaryExpr(Binary expr);
            public R visitGroupingExpr(Grouping expr);
            public R visitLiteralExpr(Literal expr);
            public R visitUnaryExpr(Unary expr);
        }
        public class Binary : Expr
        {
            public Binary(Expr left, Token oper, Expr right)
            {
                this.left = left;
                this.oper = oper;
                this.right = right;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitBinaryExpr(this);
            }
            public readonly Expr left;
            public readonly Token oper;
            public readonly Expr right;
        }
        public class Grouping : Expr
        {
            public Grouping(Expr expression)
            {
                this.expression = expression;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitGroupingExpr(this);
            }
            public readonly Expr expression;
        }
        public class Literal : Expr
        {
            public Literal(object value)
            {
                this.value = value;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitLiteralExpr(this);
            }
            public readonly object value;
        }
        public class Unary : Expr
        {
            public Unary(Token oper, Expr right)
            {
                this.oper = oper;
                this.right = right;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitUnaryExpr(this);
            }
            public readonly Token oper;
            public readonly Expr right;
        }
        public abstract R accept<R>(Visitor<R> visitor);
    }
}
