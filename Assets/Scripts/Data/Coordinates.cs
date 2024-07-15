using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Coordinates {
	public int x;
	public int y;

	public Coordinates(int a, int b){
		x = a;
		y = b;
	}

	public override string ToString() => $"({x},{y})";

	public static Coordinates operator +(Coordinates a, Coordinates b)
	=> new Coordinates(a.x + b.x, a.y + b.y);

	public static Coordinates operator -(Coordinates a, Coordinates b)
	=> new Coordinates(a.x - b.x, a.y - b.y);

	public static bool operator ==(Coordinates a, Coordinates b) => (a.x == b.x && a.y == b.y);
	public static bool operator !=(Coordinates a, Coordinates b) => (a.x != b.x || a.y != b.y);

	public override bool Equals(object o) {
		if(!(o is Coordinates)){
			return false;
		}
		Coordinates c = (Coordinates) o;
		return c == this;
	}

	public override int GetHashCode(){
		int hash = 17;
		hash = hash * 23 + x.GetHashCode();
		hash = hash * 23 + y.GetHashCode();
		return hash;
	}

	public Coordinates[] OrthogonalNeighbors(){
		Coordinates[] output = new Coordinates[4];
		output[0] = new Coordinates(x+1, y);
		output[1] = new Coordinates(x, y-1);
		output[2] = new Coordinates(x-1, y);
		output[3] = new Coordinates(x, y+1);
		return output;
	}

	public Coordinates[] AllNeighbors(){
		Coordinates[] output = new Coordinates[8];
		output[0] = new Coordinates(x+1, y);
		output[1] = new Coordinates(x+1, y-1);
		output[2] = new Coordinates(x, y-1);
		output[3] = new Coordinates(x-1, y-1);
		output[4] = new Coordinates(x-1, y);
		output[5] = new Coordinates(x-1, y+1);
		output[6] = new Coordinates(x, y+1);
		output[7] = new Coordinates(x+1, y+1);
		return output;
	}

	public bool InBounds(int w, int h){
		return x >= 0 && x < w && y >= 0 && y < h;
	}
}
