using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lox
{
    public abstract class Stmt
    {
        public interface Visitor<R>
        {
            public R visitBlockStmt(Block expr);
            public R visitExpressionStmt(Expression expr);
            public R visitIfStmt(If expr);
            public R visitPrintStmt(Print expr);
            public R visitVarStmt(Var expr);
            public R visitWhileStmt(While expr);
        }
        public class Block : Stmt
        {
            public Block(List<Stmt> statements)
            {
                this.statements = statements;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitBlockStmt(this);
            }
            public readonly List<Stmt> statements;
        }
        public class Expression : Stmt
        {
            public Expression(Expr expression)
            {
                this.expression = expression;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitExpressionStmt(this);
            }
            public readonly Expr expression;
        }
        public class If : Stmt
        {
            public If(Expr condition, Stmt thenBrance, Stmt? elseBranch)
            {
                this.condition = condition;
                this.thenBrance = thenBrance;
                this.elseBranch = elseBranch;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitIfStmt(this);
            }
            public readonly Expr condition;
            public readonly Stmt thenBrance;
            public readonly Stmt? elseBranch;
        }
        public class Print : Stmt
        {
            public Print(Expr expression)
            {
                this.expression = expression;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitPrintStmt(this);
            }
            public readonly Expr expression;
        }
        public class Var : Stmt
        {
            public Var(Token name, Expr? initalizer)
            {
                this.name = name;
                this.initalizer = initalizer;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitVarStmt(this);
            }
            public readonly Token name;
            public readonly Expr? initalizer;
        }
        public class While : Stmt
        {
            public While(Expr condition, Stmt body)
            {
                this.condition = condition;
                this.body = body;
            }
            public override R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitWhileStmt(this);
            }
            public readonly Expr condition;
            public readonly Stmt body;
        }
        public abstract R accept<R>(Visitor<R> visitor);
    }
}
