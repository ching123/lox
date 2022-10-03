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
            public R visitAssignExpr(Assign expr);
            public R visitBinaryExpr(Binary expr);
            public R visitCallExpr(Call expr);
            public R visitGetExpr(Get expr);
            public R visitGroupingExpr(Grouping expr);
            public R visitLiteralExpr(Literal expr);
            public R visitLogicalExpr(Logical expr);
            public R visitSetExpr(Set expr);
            public R visitSuperExpr(Super expr);
            public R visitThisExpr(This expr);
            public R visitUnaryExpr(Unary expr);
            public R visitVariableExpr(Variable expr);
        }
        public class Assign : Expr
        {
            public Assign(Token name, Expr value)
            {
                this.name = name;
                this.value = value;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitAssignExpr(this);
            }
            public readonly Token name;
            public readonly Expr value;
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
        public class Call : Expr
        {
            public Call(Expr callee, Token paren, List<Expr> arguments)
            {
                this.callee = callee;
                this.paren = paren;
                this.arguments = arguments;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitCallExpr(this);
            }
            public readonly Expr callee;
            public readonly Token paren;
            public readonly List<Expr> arguments;
        }
        public class Get : Expr
        {
            public Get(Expr obj, Token name)
            {
                this.obj = obj;
                this.name = name;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitGetExpr(this);
            }
            public readonly Expr obj;
            public readonly Token name;
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
            public Literal(object? value)
            {
                this.value = value;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitLiteralExpr(this);
            }
            public readonly object? value;
        }
        public class Logical : Expr
        {
            public Logical(Expr left, Token oper, Expr right)
            {
                this.left = left;
                this.oper = oper;
                this.right = right;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitLogicalExpr(this);
            }
            public readonly Expr left;
            public readonly Token oper;
            public readonly Expr right;
        }
        public class Set : Expr
        {
            public Set(Expr obj, Token name, Expr value)
            {
                this.obj = obj;
                this.name = name;
                this.value = value;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitSetExpr(this);
            }
            public readonly Expr obj;
            public readonly Token name;
            public readonly Expr value;
        }
        public class Super : Expr
        {
            public Super(Token keyword, Token method)
            {
                this.keyword = keyword;
                this.method = method;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitSuperExpr(this);
            }
            public readonly Token keyword;
            public readonly Token method;
        }
        public class This : Expr
        {
            public This(Token keyword)
            {
                this.keyword = keyword;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitThisExpr(this);
            }
            public readonly Token keyword;
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
        public class Variable : Expr
        {
            public Variable(Token name)
            {
                this.name = name;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitVariableExpr(this);
            }
            public readonly Token name;
        }
        public abstract R accept<R>(Visitor<R> visitor);
    }
}
