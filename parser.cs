using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SheetCell = Cell;

public enum Operation {
	PLUS,
	MINUS,
	TIMES,
	DIV
}

public abstract record Token {
	public record Number(float value) : Token{}
	public record Cell(string position) : Token{}
	public record Op(Operation op) : Token{}
	public record LParen() : Token{}
	public record RParen() : Token{}
	public record Eof() : Token{}
}

public class Tokenizer {
	private Queue<Token> queue;

	public Tokenizer(string str) {
		this.queue = new Queue<Token>();
		Regex isalphanum = new Regex("[a-zA-Z0-9.]");
		string acc = "";

		var push_token = () => {
			if (acc.Length == 0) return;

			if (acc.IsValidFloat()) {
				this.queue.Enqueue(new Token.Number(acc.ToFloat()));
			} else {
				this.queue.Enqueue(new Token.Cell(acc));
			}
			acc = "";
		};

		foreach (var c in str) {
			if (isalphanum.IsMatch(c.ToString())) {
				acc += c;
			} else {
				push_token();

				Token? op = c switch {
					'+' => new Token.Op(Operation.PLUS),
					'-' => new Token.Op(Operation.MINUS),
					'*' => new Token.Op(Operation.TIMES),
					'/' => new Token.Op(Operation.DIV),
					'(' => new Token.LParen(),
					')' => new Token.RParen(),
					_ => null
				};

				if (op != null) {
					this.queue.Enqueue(op);
				}
			}
		}

		push_token();
	}

	public Token next() {
		if (this.queue.Count == 0) {
			return new Token.Eof();
		}
		return this.queue.Dequeue();
	}

	public Token peek() {
		if (this.queue.Count == 0) {
			return new Token.Eof();
		}
		return this.queue.Peek();
	}
}

public enum UnaryOperation {
	NEGATE
}

public enum BinaryOperation {
	ADD,
	SUB,
	MUL,
	DIV
}

public abstract record Ast {
	public record Number(float value) : Ast{}
	public record Cell(SheetCell cell) : Ast{}
	public record BinaryOp(BinaryOperation op, Ast lhs, Ast rhs) : Ast{}
}

public class Parser {
	private Tokenizer lexer;
	private Sheet parent;

	public Parser(Tokenizer lexer, Sheet parent) {
		this.lexer = lexer;
		this.parent = parent;
	}

	public int? lbp(Token token) {
		return token switch {
			Token.Op op => op.op switch {
				Operation.PLUS or Operation.MINUS => 20,
				Operation.TIMES or Operation.DIV => 30,
				_ => throw new ArgumentException()
			},
			Token.RParen or Token.Eof => 0,
			_ => null,
		};
	}

	public Ast? nud(Token token) {
		switch (token) {
			case Token.Number num:
				return new Ast.Number(num.value);
			case Token.Cell c:
				var cell = this.parent.get_cell(c.position);
				if (cell == null) return null;
				return new Ast.Cell(cell);
			case Token.LParen:
				var inner = this.pratt(0);
				var next = this.lexer.next();
				if (next is not Token.RParen) {
					return null;
				}
				return inner; 
			default:
				return null;
		};
	}

	private Ast.BinaryOp? led_operation(Token.Op op, Ast left) {
		var (type, power) = op.op switch {
			Operation.PLUS => (BinaryOperation.ADD, 21),
			Operation.MINUS => (BinaryOperation.SUB, 21),
			Operation.TIMES => (BinaryOperation.MUL, 31),
			Operation.DIV => (BinaryOperation.DIV, 31),
			_ => throw new ArgumentException()
		};

		var right = this.pratt(power);
		if (right == null) return null;

		return new Ast.BinaryOp(type, left, right);
	}

	public Ast? led(Token token, Ast left) {
		return token switch {
			Token.Op op => led_operation(op, left),
			_ => null
		};
	}

	public Ast? pratt(int power = 0) {
		var t = this.lexer.next();
		var left = this.nud(t);
		while (this.lbp(this.lexer.peek()) > power) {
			if (left == null) return null;

			t = this.lexer.next();
			left = this.led(t, left);
		}

		return left;
	}
}

public class Evaluator {
	private Sheet sheet;
	public Evaluator(Sheet sheet) {
		this.sheet = sheet;
	}

}
