using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PoolControl : MonoBehaviour{

	Queue<GameObject> mapGridCells;
	Queue<GameObject> mapGridBorders;
	Queue<GameObject> mapGridEdges;
	Queue<GameObject> floorSlots;
	Queue<GameObject> roomGridCells;
	Queue<GameObject> roomGridBorders;
	Queue<GameObject> roomGridEdges;
	Queue<GameObject> menuSlots;

	public static PoolControl Instance;

	public void Init(){
		//ensure uniqueness
		if(Instance != null && Instance != this){
			Destroy(gameObject);
			return;
		}
		Instance = this;

		mapGridCells = new Queue<GameObject>();
		mapGridBorders = new Queue<GameObject>();
		mapGridEdges = new Queue<GameObject>();
		floorSlots = new Queue<GameObject>();
		roomGridCells = new Queue<GameObject>();
		roomGridBorders = new Queue<GameObject>();
		roomGridEdges = new Queue<GameObject>();
		menuSlots = new Queue<GameObject>();
	}



	public static void DepositMapGridCell(GameObject go){
		go.transform.SetParent(Instance.transform);
		go.transform.localPosition = Vector3.zero;
		go.SetActive(false);
		Instance.mapGridCells.Enqueue(go);
	}

	public static GameObject WithdrawMapGridCell(){
		GameObject go;
		if(Instance.mapGridCells.Count == 0){
			go = Instantiate(PrefabManager.GetUIElement(0));
		}else{
			go = Instance.mapGridCells.Dequeue();
			go.SetActive(true);
		}
		return go;
	}

	public static void DepositMapGridBorder(GameObject go){
		go.transform.SetParent(Instance.transform);
		go.transform.localPosition = Vector3.zero;
		go.SetActive(false);
		Instance.mapGridBorders.Enqueue(go);
	}

	public static GameObject WithdrawMapGridBorder(){
		GameObject go;
		if(Instance.mapGridBorders.Count == 0){
			go = Instantiate(PrefabManager.GetUIElement(1));
		}else{
			go = Instance.mapGridBorders.Dequeue();
			go.SetActive(true);
		}
		return go;
	}

	public static void DepositMapGridEdge(GameObject go){
		go.transform.SetParent(Instance.transform);
		go.transform.localPosition = Vector3.zero;
		go.SetActive(false);
		Instance.mapGridEdges.Enqueue(go);
	}

	public static GameObject WithdrawMapGridEdge(){
		GameObject go;
		if(Instance.mapGridEdges.Count == 0){
			go = Instantiate(PrefabManager.GetUIElement(2));
		}else{
			go = Instance.mapGridEdges.Dequeue();
			go.SetActive(true);
		}
		return go;
	}

	public static void DepositFloorSlot(GameObject go){
		go.transform.SetParent(Instance.transform);
		go.transform.localPosition = Vector3.zero;
		go.SetActive(false);
		Instance.floorSlots.Enqueue(go);
	}

	public static GameObject WithdrawFloorSlot(){
		GameObject go;
		if(Instance.floorSlots.Count == 0){
			go = Instantiate(PrefabManager.GetUIElement(3));
		}else{
			go = Instance.floorSlots.Dequeue();
			go.SetActive(true);
		}
		return go;
	}

	public static void DepositRoomGridCell(GameObject go){
		go.transform.SetParent(Instance.transform);
		go.transform.localPosition = Vector3.zero;
		go.SetActive(false);
		Instance.roomGridCells.Enqueue(go);
	}

	public static GameObject WithdrawRoomGridCell(){
		GameObject go;
		if(Instance.roomGridCells.Count == 0){
			go = Instantiate(PrefabManager.GetUIElement(4));
		}else{
			go = Instance.roomGridCells.Dequeue();
			go.SetActive(true);
		}
		return go;
	}

	public static void DepositRoomGridBorder(GameObject go){
		go.transform.SetParent(Instance.transform);
		go.transform.localPosition = Vector3.zero;
		go.SetActive(false);
		Instance.roomGridBorders.Enqueue(go);
	}

	public static GameObject WithdrawRoomGridBorder(){
		GameObject go;
		if(Instance.roomGridBorders.Count == 0){
			go = Instantiate(PrefabManager.GetUIElement(5));
		}else{
			go = Instance.roomGridBorders.Dequeue();
			go.SetActive(true);
		}
		return go;
	}

	public static void DepositRoomGridEdge(GameObject go){
		go.transform.SetParent(Instance.transform);
		go.transform.localPosition = Vector3.zero;
		go.SetActive(false);
		Instance.roomGridEdges.Enqueue(go);
	}

	public static GameObject WithdrawRoomGridEdge(){
		GameObject go;
		if(Instance.roomGridEdges.Count == 0){
			go = Instantiate(PrefabManager.GetUIElement(6));
		}else{
			go = Instance.roomGridEdges.Dequeue();
			go.SetActive(true);
		}
		return go;
	}

	public static void DepositMenuSlot(GameObject go){
		go.transform.SetParent(Instance.transform);
		go.transform.localPosition = Vector3.zero;
		go.SetActive(false);
		Instance.menuSlots.Enqueue(go);
	}

	public static GameObject WithdrawMenuSlot(){
		GameObject go;
		if(Instance.menuSlots.Count == 0){
			go = Instantiate(PrefabManager.GetUIElement(7));
		}else{
			go = Instance.menuSlots.Dequeue();
			go.SetActive(true);
		}
		return go;
	}

}