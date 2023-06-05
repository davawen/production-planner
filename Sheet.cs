using System;
using System.Collections.Generic;

public class Sheet {
	private int width;
	private int height;
	public Dictionary<string, Cell> cells { get; private set; }

    public Sheet(int width, int height) {
		this.width = width;
		this.height = height;
		this.cells = new Dictionary<string, Cell>();

		for (int j = 0; j < height; j++) {
			for (int i = 0; i < width; i++) {
				var column = (char)(65 + i);
				var row = (j + 1).ToString();

				var cell = new Cell(column + row, this);
				cells[cell.name] = cell;
			}
		}
	}

	public Cell? get_cell(string position) {
		if (!cells.ContainsKey(position)) return null;

		return cells[position];
	}
}
