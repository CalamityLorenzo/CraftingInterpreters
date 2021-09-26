using System;
using System.Collections.Generic;
namespace CsLoxInterpreter.Expressions {
internal abstract class Expr {
internal abstract T Accept<T>(IVisitor<T> visitor);
interface IVisitor<T>{
		T VisitAssignExpr(Assign expr);
		T VisitBinaryExpr(Binary expr);
		T VisitConditionalExpr(Conditional expr);
		T VisitCallExpr(Call expr);
		T VisitGetExpr(Get expr);
		T VisitGroupingExpr(Grouping expr);
		T VisitLiteralExpr(Literal expr);
		T VisitLogicalExpr(Logical expr);
		T VisitSetExpr(Set expr);
		T VisitThisExpr(This expr);
		T VisitUnaryExpr(Unary expr);
		T VisitVariableExpr(Variable expr);
	}

internal class Assign : Expr {
	internal Assign(Token name, Expr value){
	this.Name=name;
	this.Value=value;
	}


internal override T Accept<T>(IVisitor<T> visitor){
		return visitor.VisitAssignExpr(this);
}
	public Token Name{get;}
	public Expr Value{get;}
}

internal class Binary : Expr {
	internal Binary(Expr left, Token @operator, Expr right){
	this.Left=left;
	this.@operator=@operator;
	this.Right=right;
	}


internal override T Accept<T>(IVisitor<T> visitor){
		return visitor.VisitBinaryExpr(this);
}
	public Expr Left{get;}
	public Token @operator{get;}
	public Expr Right{get;}
}

internal class Conditional : Expr {
	internal Conditional(Expr ifThen, Expr ifElse){
	this.IfThen=ifThen;
	this.IfElse=ifElse;
	}


internal override T Accept<T>(IVisitor<T> visitor){
		return visitor.VisitConditionalExpr(this);
}
	public Expr IfThen{get;}
	public Expr IfElse{get;}
}

internal class Call : Expr {
	internal Call(Expr callee, Token paren, List<Expr> arguments){
	this.Callee=callee;
	this.Paren=paren;
	this.Arguments=arguments;
	}


internal override T Accept<T>(IVisitor<T> visitor){
		return visitor.VisitCallExpr(this);
}
	public Expr Callee{get;}
	public Token Paren{get;}
	public List<Expr> Arguments{get;}
}

internal class Get : Expr {
	internal Get(Expr @object, Token name){
	this.@object=@object;
	this.Name=name;
	}


internal override T Accept<T>(IVisitor<T> visitor){
		return visitor.VisitGetExpr(this);
}
	public Expr @object{get;}
	public Token Name{get;}
}

internal class Grouping : Expr {
	internal Grouping(Expr expression){
	this.Expression=expression;
	}


internal override T Accept<T>(IVisitor<T> visitor){
		return visitor.VisitGroupingExpr(this);
}
	public Expr Expression{get;}
}

internal class Literal : Expr {
	internal Literal(Object value){
	this.Value=value;
	}


internal override T Accept<T>(IVisitor<T> visitor){
		return visitor.VisitLiteralExpr(this);
}
	public Object Value{get;}
}

internal class Logical : Expr {
	internal Logical(Expr left, Token @operator, Expr right){
	this.Left=left;
	this.@operator=@operator;
	this.Right=right;
	}


internal override T Accept<T>(IVisitor<T> visitor){
		return visitor.VisitLogicalExpr(this);
}
	public Expr Left{get;}
	public Token @operator{get;}
	public Expr Right{get;}
}

internal class Set : Expr {
	internal Set(Expr @object, Token name, Expr value){
	this.@object=@object;
	this.Name=name;
	this.Value=value;
	}


internal override T Accept<T>(IVisitor<T> visitor){
		return visitor.VisitSetExpr(this);
}
	public Expr @object{get;}
	public Token Name{get;}
	public Expr Value{get;}
}

internal class This : Expr {
	internal This(Token keyword){
	this.Keyword=keyword;
	}


internal override T Accept<T>(IVisitor<T> visitor){
		return visitor.VisitThisExpr(this);
}
	public Token Keyword{get;}
}

internal class Unary : Expr {
	internal Unary(Token @operator, Expr right){
	this.@operator=@operator;
	this.Right=right;
	}


internal override T Accept<T>(IVisitor<T> visitor){
		return visitor.VisitUnaryExpr(this);
}
	public Token @operator{get;}
	public Expr Right{get;}
}

internal class Variable : Expr {
	internal Variable(Token name){
	this.Name=name;
	}


internal override T Accept<T>(IVisitor<T> visitor){
		return visitor.VisitVariableExpr(this);
}
	public Token Name{get;}
}


}
}
