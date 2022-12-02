namespace LoxSharp;

abstract class Expr {
   class Binary : Expr {
        Binary(Expr left, Token operaTor, Expr right) {
            this.left = left;
            this.operaTor = operaTor;
            this.right = right;
        }

        readonly Expr left;
        readonly Token operaTor;
        readonly Expr right;
    }
   class Grouping : Expr {
        Grouping(Expr expression) {
            this.expression = expression;
        }

        readonly Expr expression;
    }
   class Literal : Expr {
        Literal(Object value) {
            this.value = value;
        }

        readonly Object value;
    }
   class Unary : Expr {
        Unary(Token operaTor, Expr right) {
            this.operaTor = operaTor;
            this.right = right;
        }

        readonly Token operaTor;
        readonly Expr right;
    }
}
