using Godot;
using System;
using System.Collections.Generic;

public class Cell
{
	public String name;
	public Sheet parent;

	public String input;

	public Ast? parsed;
	public float? computed;

	public List<Cell> depends_on;
	public HashSet<Cell> depended_on;

	public Cell(String name, Sheet parent) {
		this.name = name;
		this.parent = parent;

		this.input = "";
		this.parsed = null;
		this.computed = null;

		this.depends_on = new List<Cell>();
		this.depended_on = new HashSet<Cell>();
	}

	public void update_input(String new_input) {
		if (this.input == new_input) return;

		this.input = new_input;

		if (new_input.StartsWith('=')) {
			var lexer = new Tokenizer(new_input.Substring(1));
			var parser = new Parser(lexer, parent);
			this.parsed = parser.pratt();
		}

		this.reset_dependencies();
		this.compute();
	}

	void reset_dependencies() {
		foreach (var cell in depends_on) {
			cell.depended_on.Remove(this);
		}

		this.depends_on.Clear();
		if (this.parsed != null) {
			this.find_dependency(this.parsed);
		}
	}

	void find_dependency(Ast ast) {
		switch (ast) {
			case Ast.Cell c:
				this.depends_on.Add(c.cell);
				c.cell.depended_on.Add(this);
				break;
			case Ast.BinaryOp op:
				this.find_dependency(op.lhs);
				this.find_dependency(op.rhs);
				break;
		}
	}

	public void compute() {
		this.computed = null;
		if (this.parsed == null) return;

		this.computed = this.eval(this.parsed);
	}

	float? eval_binary_op(Ast.BinaryOp op) {
		var lhs = this.eval(op.lhs);
		if (lhs == null) return null;
		var rhs = this.eval(op.rhs);
		if (rhs == null) return null;

		return op.op switch {
			BinaryOperation.ADD => lhs + rhs,
			BinaryOperation.SUB => lhs - rhs,
			BinaryOperation.MUL => lhs * rhs,
			BinaryOperation.DIV => lhs / rhs,
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	float? eval(Ast ast) {
		return ast switch {
			Ast.Number n => n.value,
			Ast.Cell c => c.cell.computed,
			Ast.BinaryOp op => this.eval_binary_op(op),
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}
